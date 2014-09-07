using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Conclave.Collections;
using Conclave.Data.Store;
using Conclave.Map.Model;

namespace Conclave.Map.Store {
	public abstract class SqlTopicStore: SqlStore, ITopicStore {

		protected SqlTopicStore(DbProviderFactory instance, string connStr) : base(instance, connStr) {}

		// move to extension
		protected IEnumerable<Topic.Builder> ProcessTopicQuery(string sql, params IDbDataParameter[] parms) {
			string topicId, assocId, occurParent, topicMetaParent, assocMetaParent;
			Topic.Builder currentTopic = null;
			Association.Builder currentAssoc = null;
			Occurrence.Builder currentOccur;
			Metadata.Builder currentTopicMeta;
			Metadata.Builder currentAssocMeta;
			DataCollection<Topic.Builder> topics = new DataCollection<Topic.Builder>();

			using (IDataReader reader = this.Read(sql, parms)) {
				while (reader.Read()) {
					// get the topic id for the current record
					topicId = reader.ReadString("topic_id");
					// then check if this is a new topic
					if (currentTopic == null || currentTopic.Id != topicId) {
						// we have a new topic
						currentTopic = new Topic.Builder(topicId);
						topics.Add(currentTopic);
						currentAssoc = null; // the current assoc can't remain current with a new topic
					}
					// now check if we have a new assoc
					assocId = reader.ReadString("assoc_id");
					if (currentAssoc == null && assocId != null || currentAssoc != null && currentAssoc.Id != assocId) {
						// we have a new assoc
						currentAssoc = new Association.Builder()
						{
							Id = assocId,
							Parent = reader.ReadString("assoc_parent"),
							Type = reader.ReadString("assoc_type"),
							Scope = reader.ReadString("assoc_scope"),
							Role = reader.ReadString("assoc_role"),
							Reference = reader.ReadString("assoc_ref")
						};
						currentTopic.Associations.Add(currentAssoc);
					}
					// check for occurence data
					occurParent = reader.ReadString("occur_parent");
					if (occurParent != null) { // test that this is an adequate guard
						currentOccur = new Occurrence.Builder()
						{
							Parent = reader.ReadString("occur_parent"),
							Scope = reader.ReadString("occur_scope"),
							Role = reader.ReadString("occur_role"),
							Behaviour = reader.ReadString("occur_behaviour"),
							Reference = reader.ReadString("occur_ref"),
							Data = reader.ReadBinaryData("occur_data")
						};
						currentTopic.Occurrences.Add(currentOccur);
					}
					// check for topic metadata
					topicMetaParent = reader.ReadString("topic_meta_parent");
					if (topicMetaParent != null) { // test that this is an adequate guard
						// we have topic metadata
						currentTopicMeta = new Metadata.Builder()
						{
							Parent = topicMetaParent,
							Scope = reader.ReadString("topic_meta_scope"),
							Name = reader.ReadString("topic_meta_name"),
							Value = reader.ReadString("topic_meta_value"),
						};
						currentTopic.AddMetadata(currentTopicMeta);
					}
					// check for assoc metadata
					assocMetaParent = reader.ReadString("assoc_meta_parent");
					if (assocMetaParent != null && currentAssoc != null) {
						// we have topic metadata
						currentAssocMeta = new Metadata.Builder()
						{
							Parent = assocMetaParent,
							Scope = reader.ReadString("assoc_meta_scope"),
							Name = reader.ReadString("assoc_meta_name"),
							Value = reader.ReadString("assoc_meta_value"),
						};
						currentAssoc.AddMetadata(currentAssocMeta);
					}
				}
			}

			//return topics.Cast<Topic>();
			return topics;
		}

		public abstract void RemoveMetadataFor(string parent);
		public abstract void RemoveMetadata(string parent, string scope, string name);

		public virtual void AddMetadata(IEnumerable<Metadata> metadata) {
			this.AddMetadata(metadata, true);
		}

		protected virtual void AddMetadata(IEnumerable<Metadata> metadata, bool check) {
			foreach (Metadata meta in metadata) {
				this.AddMetadata(meta, check);
			}
		}

		public virtual void AddMetadata(Metadata meta) {
			this.AddMetadata(meta, true);
		}

		protected virtual void AddMetadata(Metadata meta, bool check) {
			this.SetMetadata(meta.Parent, meta.Scope, meta.Name, meta.Value, check);
		}

		public virtual void SetMetdadata(string parent, string name, string value) {
			this.SetMetadata(parent, "default", name, value, true);
		}

		public virtual void SetMetadata(string parent, string scope, string name, string value) {
			this.SetMetadata(parent, scope, name, value, true);
		}

		// consider making protected protected?
		public abstract void SetMetadata(string parent, string scope, string name, string value, bool check);

		public abstract void UpdateMetadataForPointingAssociations(
			string id,
			string assocType, string assocRole,
			string metaScope, string metaName, string metaValue
			);

		public virtual Metadata GetMetadata(string parent, string name) {
			return this.GetMetadata(parent, "default", name);
		}

		public abstract Metadata GetMetadata(string parent, string scope, string name);
		public abstract IEnumerable<Metadata> GetMetadataFor(string parent);
		public abstract void RemoveOccurrencesFor(string parent);
		public abstract void RemoveOccurrence(string parent, string scope, string role, string behaviour, string reference);

		public virtual void AddOccurrences(IEnumerable<Occurrence> occurrences) {
			this.AddOccurrences(occurrences, true);
		}

		protected virtual void AddOccurrences(IEnumerable<Occurrence> occurrences, bool check) {
			foreach (Occurrence occurrence in occurrences) {
				this.AddOccurrence(occurrence, check);
			}
		}

		public virtual void AddOccurrence(Occurrence occurrence) {
			this.AddOccurrence(occurrence, true);
		}

		protected virtual void AddOccurrence(Occurrence occurrence, bool check) {
			this.SetOccurrence(occurrence.Parent, occurrence.Scope, occurrence.Role, occurrence.Behaviour, occurrence.Reference, occurrence.Data, check);
		}

		public virtual void SetOccurrence(string parent, string role, string behaviour, string reference) {
			this.SetOccurrence(parent, "default", role, behaviour, reference);
		}

		protected abstract void SetOccurrence(string parent, string scope, string role, string behaviour, string reference,
		                             byte[] data = null, bool check = true);

		public virtual Occurrence GetOccurrence(string parent, string role, string behaviour, string reference) {
			return this.GetOccurrence(parent, "default", role, behaviour, reference);
		}

		/// <summary>
		/// Returns any occurrence that matches the provided parameters; or, returns a blank occurrence.
		/// </summary>
		/// <param name="parent">The identity of the parent Topic to which the occurrence belongs.</param>
		/// <param name="scope">The scope of the occurrence.</param>
		/// <param name="role">The role that the occurrence performs.</param>
		/// <param name="behaviour">The behaviour cited to resolve the occurrence.</param>
		/// <param name="reference">The reference to the occurrence backing data to be resolved by the cited behaviour.</param>
		/// <returns>Returns either the occurrence matching the provided parameters; or, returns a blank occurrence.</returns>
		/// <remarks>It is API breaking for this method to return a null reference.</remarks>
		public abstract Occurrence GetOccurrence(string parent, string scope, string role, string behaviour, string reference);

		public abstract IEnumerable<Occurrence> GetOccurrencesFor(string parent);

		public virtual bool OccurrenceExists(string parent, string scope, string role, string behaviour, string reference) {
			return (this.GetOccurrence(parent, scope, role, behaviour, reference) != Occurrence.Blank);
		}

		/// <summary>
		/// Gets all the association ids in the store.
		/// </summary>
		/// <returns>
		/// Returns an an enumeration of all the association ids.
		/// </returns>
		public virtual IEnumerable<string> GetAssociationIds() {
			List<string> result = new List<string>();
			using (IDataReader reader = this.Read(@"select id from association")) {
				while (reader.Read()) {
					result.Add(reader.ReadString("id"));
				}
			}
			return result;
		}

		/// <summary>
		/// Checks whether the specified association exists in the store.
		/// </summary>
		/// <param name="id">The identity of the association to check for.</param>
		/// <returns>Returns <b>true</b> if the association exists; otherwise returns <b>false</b>.</returns>
		public abstract bool AssociationExists(string id);

		public abstract Association GetAssociation(string id);
		public abstract void RemoveAssociationsFor(string parent);

		public abstract void RemovePointingAssociations(string reference);
		public abstract void RemoveAssociation(string id);

		public virtual void AddAssociations(IEnumerable<Association> associations) {
			this.AddAssociations(associations, true);
		}

		protected virtual void AddAssociations(IEnumerable<Association> associations, bool check) {
			foreach (Association association in associations) {
				this.AddAssociation(association, check);
			}
		}

		public virtual void AddAssociation(Association association) {
			this.AddAssociation(association, true);
		}

		protected virtual void AddAssociation(Association association, bool check) {
			if (check) {
				this.RemoveAssociation(association.Id);
			}
			this.CreateAssociation(association.Id, association.Scope, association.Type, association.Role, association.Parent, association.Reference);
			this.AddMetadata(association.Metadata);
		}

		public abstract void CreateAssociation(string id, string scope, string type, string role, string parent, string reference);

		/// <summary>
		/// Checks whether the specified topic exists in the store.
		/// </summary>
		/// <param name="topicId">The identity of the topic to check for.</param>
		/// <returns>Returns <b>true</b> if the topic exists; otherwise returns <b>false</b>.</returns>
		public abstract bool TopicExists(string topicId);

		/// <summary>
		/// Gets all the topic ids in the store.
		/// </summary>
		/// <returns>
		/// Returns an an enumerable of all the topic ids.
		/// </returns>
		public virtual IEnumerable<string> GetTopicIds() {
			List<string> result = new List<string>();
			using (IDataReader reader = this.Read(@"select id from topic")) {
				while (reader.Read()) {
					result.Add(reader.ReadString("id"));
				}
				return result;
			}
		}

		/// <summary>
		/// Creates a topic with the specified Id.
		/// </summary>
		/// <param name="id">The identity of the topic to create.</param>
		/// <returns>Returns the newly created <see cref="Topic"/></returns>
		/// <exception cref="TopicStoreException">
		/// It is an error to attempt to create a topic that already exists.
		/// </exception>
		/// <remarks>
		/// This is a leaky abstraction and needs to be removed. It relies
		/// on knowledge of the relational model of the sql stores
		/// and that the topic table is a discrete table of topic ids
		/// alone.
		/// </remarks>
		/// TODO: Breaking. Remove, or add Topic return type for the topic created.
		public abstract void CreateTopic(string id);

		/// <summary>
		/// Removes the specified topic from the store.
		/// </summary>
		/// <param name="topicId">The identity of the topic to remove.</param>
		public abstract void RemoveTopic(string topicId);

		/// <summary>
		/// Adds a topic to the store.
		/// </summary>
		/// <param name="topic">The topic to add to the store.</param>

		public void AddTopic(Topic topic) {
			this.RemoveTopic(topic.Id);
			this.CreateTopic(topic.Id);
			this.AddMetadata(topic.Metadata, false);
			this.AddOccurrences(topic.Occurrences, false);
			this.AddAssociations(topic.Associations, false);
		}

		public abstract Topic GetTopic(string id);
		public abstract Topic GetTopic(string id, string scope);

	}
}
