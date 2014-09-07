using System;
using System.Collections.Generic;
using System.Linq;

using MongoDB.Bson;
using MongoDB.Driver;

using Conclave.Data.Store;
using Conclave.Map.Model;

namespace Conclave.Map.Store {

	/// <summary>
	/// An <see cref="ITopicStore"/> for MongoDB.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Mongo topic store behaves slightly differently
	/// that topic stores implemented against relational
	/// databases. This difference is largely around adding
	/// metadata, occurrences, and associations by themselves
	/// directly when there may be no parent topic in the store.
	/// See implementation notes.
	/// </para>
	/// <para>
	/// You can only add child members to parents that exist
	/// in the Mongo store.
	/// </para>
	/// </remarks>

	public class MongoTopicStore: MongoStore, ITopicStore {

		// check ./readme.md for schema, index and performance notes

		// TODO: read and digest http://docs.mongodb.org/manual/tutorial/perform-two-phase-commits/
		// see if this can be used to implement transaction as per the sql stores
		// AddTopic isn't atomic, the cited document might help with that, as might FindAndUpdate
		// there's a hole between finding the candidate document and then saving the newer version of the document
		// for most applications this is a purely theoretical concern... really it is... it would still be nice to have atomic updates

		// because updating of topic members is done by getting the topic, performing the member
		// update, and then replacing the old document with the new one in Mongo, it relies
		// on the topic model correctly dealing with duplicate member keys... adding new metadata for example
		// there's not unit tests in place for the topic model yet, so bugs in this area are likely
		// the chief mechanism for this in the model are the hashcodes produced by the model
		// and member collections being hash sets... this is where any problems are likely to be found

		// the advantage of the get topic, modify topic, add/replace topic back into Mongo
		// is that significantly fewer indexes need to be maintained

		// this store will change over time as experienced is gained
		// the current implementation strives to be "not terrible"

		public MongoTopicStore(string connStr, string dbName) : base(connStr, dbName) {}

		public void RemoveMetadataFor(string parent) {
			// leaky abstraction, this betrays the relation implementation of this API
			// upon which it is based, this has no real use-case for a document or
			// object backing store... it's a refactoring of topic/assoc removal in the sql stores
			// should be made protected to the sql stores
			Topic.Builder topic = _getTopicBuilder(parent);
			if (topic == null) {
				throw new TopicStoreException("The topic you are trying to remove metadata from does not exist.");
			} else {
				topic.Metadata.Clear();
				this.AddTopic(topic);
			}
		}

		public void RemoveMetadata(string parent, string scope, string name) {
			// make atomic
			Topic.Builder topic = _getTopicBuilder(parent);
			if (topic == null) {
				throw new TopicStoreException("The topic you are trying to remove metadata from does not exist.");
			} else {
				topic.Metadata.RemoveWhere(m => m.Scope == scope && m.Name == name);
				this.AddTopic(topic);
			}
		}

		public void AddMetadata(IEnumerable<Metadata> metadata) {
			// this gets the parent topic builder
			// adds all the associations to the builder
			// and then adds the topic builder to the store
			// make atomic
			Topic.Builder topic = null;
			foreach (Metadata meta in metadata) {
				if (topic == null) {
					topic = _getTopicBuilder(meta.Parent);
					if (topic == null) throw new TopicStoreException("The topic you are attempting to add metadata to does not exist.");
				}
				if (meta.Parent != topic.Id) {
					throw new TopicStoreException("The metadata you are attempting to add does not belong to the topic you are trying to add it to.");
				} else {
					topic.Metadata.Add(meta);
				}
			}
			this.AddTopic(topic);
		}

		public void AddMetadata(Metadata meta) {
			// consider adding an index for the metadata
			// so that it can be queried for directly

			// first try to add metadata to a topic
			Topic.Builder topic = _getTopicBuilder(meta.Parent);
			if (topic == null) {
				// then try to add it as an association
				Association.Builder assoc = _getAssociationBuilder(meta.Parent);
				if (assoc == null) {
					throw new TopicStoreException("The topic you wish to add the metadata to does not exist.");
				} else {
					assoc.Metadata.RemoveWhere(m => m.Name == meta.Name && m.Scope == meta.Scope);
					assoc.Metadata.Add(meta);
					this.AddAssociation(assoc);	
				}
			} else {
				topic.Metadata.RemoveWhere(m => m.Name == meta.Name && m.Scope == meta.Scope);
				topic.Metadata.Add(meta);
				this.AddTopic(topic);
			}
		}

		public void SetMetdadata(string parent, string name, string value) {
			this.SetMetadata(parent, "default", name, value);
		}

		public void SetMetadata(string parent, string scope, string name, string value) {
			this.AddMetadata(new Metadata(parent, scope, name, value));
		}

		public void SetMetadata(string parent, string scope, string name, string value, bool check) {
			// this is beyond leaky and has no place here
			// kill anything to do with bool: check on the public api
			// this is a relational optimisation
			throw new NotImplementedException();
		}

		/// <summary>
		/// Updates the metadata for all associations pointing
		/// at the provided id.
		/// </summary>
		/// <remarks>This is useful for updating the labels of pointing assocs.</remarks>
		/// <param name="id"></param>
		/// <param name="assocType">The type of assocs to consider.</param>
		/// <param name="assocRole">The role of assocs to consider.</param>
		/// <param name="metaScope">The scope of metadata to consider.</param>
		/// <param name="metaName">The name of metadata to consider.</param>
		/// <param name="metaValue">The value to set for the metadata.</param>
		public void UpdateMetadataForPointingAssociations(string id, string assocType, string assocRole, string metaScope,
			string metaName, string metaValue) {
			throw new NotImplementedException();
		}

		public Metadata GetMetadata(string parent, string name) {
			return this.GetMetadata(parent, "default", name);
		}

		public Metadata GetMetadata(string parent, string scope, string name) {
			return this.GetTopic(parent).Metadata.FirstOrDefault(m => m.Scope == scope && m.Name == name) ?? Metadata.Blank;
		}

		public IEnumerable<Metadata> GetMetadataFor(string parent) {
			return this.GetTopic(parent).Metadata;
		}

		public void RemoveOccurrencesFor(string parent) {
			// leaky abstraction, see comments for .RemoveMetadataFor()
			// this method has no use-case and is a relational concern
			// make protected on the sql store and kill on public api
			Topic.Builder topic = _getTopicBuilder(parent);
			if (topic == null) {
				throw new TopicStoreException("The topic you are trying to remove metadata from does not exist.");
			} else {
				topic.Occurrences.Clear();
				this.AddTopic(topic);
			}
		}

		public void RemoveOccurrence(string parent, string scope, string role, string behaviour, string reference) {
			// make atomic
			// not sure reference belongs here, confirm index and implementation on relational stores
			Topic.Builder topic = _getTopicBuilder(parent);
			if (topic == null) {
				throw new TopicStoreException("The topic you are trying to remove metadata from does not exist.");
			} else {
				topic.Occurrences.RemoveWhere(o => o.Scope == scope && o.Role == role && o.Behaviour == behaviour && o.Reference == reference);
				this.AddTopic(topic);
			}
		}

		public void AddOccurrences(IEnumerable<Occurrence> occurrences) {
			// make atomic
			Topic.Builder topic = null;
			foreach (Occurrence occur in occurrences) {
				if (topic == null) {
					topic = _getTopicBuilder(occur.Parent);
					if (topic == null) throw new TopicStoreException("The topic you are attempting to add occurrences to does not exist.");
				}
				if (occur.Parent != topic.Id) {
					throw new TopicStoreException(
						"The occurrence you are attempting to add does not belong to the topic you are trying to add it to.");
				} else {
					topic.Occurrences.Add(occur);
				}
			}
			this.AddTopic(topic);
		}

		public void AddOccurrence(Occurrence occurrence) {
			// make atomic
			Topic.Builder topic = _getTopicBuilder(occurrence.Parent);
			if (topic == null) {
				throw new TopicStoreException("The topic you wish to add the metadata to does not exist.");
			} else {
				topic.Occurrences.RemoveWhere(o => o == occurrence);
				topic.Occurrences.Add(occurrence);
				this.AddTopic(topic);
			}
		}

		public void SetOccurrence(string parent, string role, string behaviour, string reference) {
			// this is incomplete and lacks SetOccurrence(parent, scope, role, behaviour, reference)
			this.AddOccurrence(new Occurrence(parent, role, behaviour, reference));
		}

		public Occurrence GetOccurrence(string parent, string role, string behaviour, string reference) {
			return this.GetOccurrence(parent, "default", role, behaviour, reference);
		}

		public Occurrence GetOccurrence(string parent, string scope, string role, string behaviour, string reference) {
			Topic topic = this.GetTopic(parent);
			if (topic == null) {
				throw new TopicStoreException("The parent of the occurence you are trying to get does not exist.");
			} else {
				return topic.Occurrences.FirstOrDefault(o =>
				                               o.Scope == scope &&
				                               o.Role == role &&
				                               o.Behaviour == behaviour &&
				                               o.Reference == reference
					       ) ?? Occurrence.Blank;
			}
		}

		public bool OccurrenceExists(string parent, string scope, string role, string behaviour, string reference) {
			// what is the use case for this?
			return (this.GetOccurrence(parent, scope, role, behaviour, reference) != Occurrence.Blank);
		}

		public IEnumerable<Occurrence> GetOccurrencesFor(string parent) {
			return this.GetTopic(parent).Occurrences;
		}

		public IEnumerable<string> GetAssociationIds() {
			// this is a suicide bomber of a method on a large dataset
			// move to IExtendedTopicStore, dont even implement here
			// this would be a non-supported feature on most 
			// non-trivial mongo datasets
			throw new NotImplementedException();
		}

		public bool AssociationExists(string id) {
			return (this.GetAssociation(id) != Association.Blank);
		}

		private Topic.Builder _getTopicBuilderForAssociation(string id) {
			// we have association.id indexed, but it's still the topic document that will be fetched
			BsonDocument doc = this["topic"].FindOne(new QueryDocument("associations.id", id));
			return _readTopicDocument(doc);
		}

		private Association.Builder _getAssociationBuilder(string id) {
			Topic.Builder topic = _getTopicBuilderForAssociation(id);
			if (topic != null) {
				return topic.Associations.FirstOrDefault(a => a.Id == id);
			}
			return null;
		}

		public Association GetAssociation(string id) {
			return _getAssociationBuilder(id) ?? Association.Blank;
		}

		public Association GetAssociation(string id, string scope) {
			return _getAssociationBuilder(id) ?? Association.Blank;
		}

		public void RemoveAssociationsFor(string parent) {
			// this again is one of those methods that makes sense
			// as a refactoring of a relational store, but
			// doesn't make much sense on a document store
			// this should be moved to sql store and made protected
			// it has no public use, leaks knowledge of underlying schema
			Topic.Builder topic = _getTopicBuilder(parent);
			if (topic == null) {
				throw new TopicStoreException("The topic you are trying to remove metadata from does not exist.");
			} else {
				topic.Associations.Clear();
				this.AddTopic(topic);
			}
		}

		public void RemovePointingAssociations(string reference) {
			throw new NotImplementedException();
		}

		public void RemoveAssociation(string id) {
			// this can be done as an atomic update rather
			// than like this which is at best cheesy
			BsonDocument assoc = this["topic"].FindOne(new QueryDocument("associations.id", id));
			if (assoc != null) {
				string topicId = assoc["parent"].AsString;
				Topic.Builder topic = _getTopicBuilder(topicId);
				if (topic != null) {
					topic.Associations.RemoveWhere(a => a.Id == id);
					this.AddTopic(topic);
				}
			}
		}

		public void AddAssociations(IEnumerable<Association> associations) {
			// this gets the parent topic builder
			// adds all the associations to the builder
			// and then adds the topic builder to the store
			Topic.Builder topic = null;
			foreach (Association assoc in associations) {
				if (topic == null) {
					topic = _getTopicBuilder(assoc.Parent);
					if (topic == null) throw new TopicStoreException("The topic you are attempting to add associations to does not exist.");
				}
				if (assoc.Parent != topic.Id) {
					throw new TopicStoreException(
						"The association you are attempting to add does not belong to the topic you are trying to add it to.");
				} else {
					topic.Associations.Add(assoc);
				}
			}
			this.AddTopic(topic);
		}

		public void AddAssociation(Association association) {
			// currently this is actually a full topic document update
			// investigate whether updated the association directly
			// might be better... the concern there is that such an
			// update would require more round trips to the server
			Topic.Builder topic = _getTopicBuilder(association.Parent);
			if (topic == null) {
				throw new TopicStoreException("The topic you are attempting to add associations to does not exist.");
			} else {
				topic.Associations.RemoveWhere(a => a.Id == association.Id);
				topic.Associations.Add(association);
				this.AddTopic(topic);
			}
		}

		public void CreateAssociation(string id, string scope, string type, string role, string parent, string reference) {
			// this is a pretty useless method with no label
			// check its usage
			Topic.Builder topic = _getTopicBuilder(parent);
			if (topic == null) {
				throw new TopicStoreException("The topic you are attempting to add associations to does not exist.");
			} else {
				Association assoc = new Association(parent, type, scope, reference, role);
				topic.Associations.Add(assoc);
				this.AddTopic(topic);
			}
		}

		public bool TopicExists(string id) {
			return (this["topic"].Count(new QueryDocument("id", id)) > 0);
		}

		public IEnumerable<string> GetTopicIds() {
			// another lemming method
			// move to IExtendedTopicStore implementation
			// on a large dataset this method could suck,
			// although not as much as GetAssociationIds
			throw new NotImplementedException();
		}

		public void CreateTopic(string id) {
			if (this.TopicExists(id)) throw new TopicStoreException("The topic you are trying to create already exists.");

			this["topic"].Insert(new BsonDocument("id", Guid.NewGuid().ToString()));
		}

		public void RemoveTopic(string id) {
			this["topic"].Remove(new QueryDocument("id", id));
		}

		public void AddTopic(Topic topic) {
			// this is pretty much it for updates to the db
			// while more fine-grained updates are possible, and will checked for the future
			// this is it for now... all the updating methods go through here and perform
			// a full topic swap with the topic passed replacing the old one in
			// its entirety... there's no actual updating taking place in this store
			BsonDocument existing = this["topic"].FindOne(new QueryDocument("id", topic.Id));
			BsonDocument created = BsonDocument.Parse(topic.ToJson());
			if (existing != null) {
				created["_id"] = existing["_id"];
			}
			this["topic"].Save<BsonDocument>(created);
		}

		private Topic.Builder _getTopicBuilder(string id, string scope = null) {
			BsonDocument doc = this["topic"].FindOne(new QueryDocument("id", id));
			return _readTopicDocument(doc, scope);
		}
		
		private Topic.Builder _readTopicDocument(BsonDocument doc, string scope = null) {
			if (doc != null && doc["_type"] == "topic") {
				Topic.Builder topic = new Topic.Builder(doc["id"].AsString);
				BsonArray metadata = doc["metadata"].AsBsonArray;
				foreach (BsonValue meta in metadata) {
					if (scope == null || meta["scope"] == scope) {
						topic.AddMetadata(meta["scope"].AsString, meta["name"].AsString, meta["value"].AsString);
					}
				}
				BsonArray occurrences = doc["occurrences"].AsBsonArray;
				foreach (BsonValue occur in occurrences) {
					if (scope == null || occur["scope"] == scope) {
						topic.AddOccurrence(occur["scope"].AsString, occur["role"].AsString, occur["behaviour"].AsString,
						                    occur["reference"].AsString, occur["string-data"].AsString);
					}
				}
				BsonArray assocs = doc["associations"].AsBsonArray;
				foreach (BsonValue assoc in assocs) {
					Association.Builder association = new Association.Builder(assoc["id"].AsString, assoc["parent"].AsString, assoc["type"].AsString, assoc["scope"].AsString, assoc["reference"].AsString, assoc["role"].AsString);
					foreach (BsonValue assocMeta in assoc["metadata"].AsBsonArray) {
						if (scope == null || assocMeta["scope"] == scope) {
							association.AddMetadata(assocMeta["scope"].AsString, assocMeta["name"].AsString, assocMeta["value"].AsString);
						}
					}
					topic.Associations.Add(association);
				}
				return topic;
			}
			return null;
		}

		public Topic GetTopic(string id) {
			return this.GetTopic(id, null);
		}

		public Topic GetTopic(string id, string scope) {
			return _getTopicBuilder(id, scope) ?? Topic.Blank;
		}

	}
}
