using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using MySql.Data.MySqlClient;

using Conclave.Collections;
using Conclave.Data.Store;
using Conclave.Map.Model;

namespace Conclave.Map.Store.MySql {
	public class _MySqlTopicStore : SqlStore, ITopicStore {

		private static Metadata.Builder _readMeta(IDataReader reader, Node.Builder node) {
			Metadata.Builder metadata = _readMeta(reader);
			if (metadata != null) {
				node.AddMetadata(metadata);
			}
			return metadata;
		}

		private static Metadata.Builder _readMeta(IDataReader reader) {
			Metadata.Builder builder = null;
			string parent = reader.ReadString("meta_parent");
			if (!String.IsNullOrWhiteSpace(parent)) {
				builder = new Metadata.Builder()
				{
					Parent = reader.ReadString("meta_parent"),
					Scope = reader.ReadString("meta_scope"),
					Name = reader.ReadString("meta_name"),
					Value = reader.ReadString("meta_value")
				};
			}
			return builder;
		}

		private static Association.Builder _readAssoc(IDataReader reader) {
			Association.Builder builder = new Association.Builder()
			{
				Id = reader.ReadString("assoc_id"),
				Parent = reader.ReadString("assoc_parent"),
				Scope = reader.ReadString("assoc_scope"),
				Type =  reader.ReadString("assoc_type"),
				Role = reader.ReadString("assoc_role"),
				Reference = reader.ReadString("assoc_ref")
			};
			return builder;
		}

		//public MySqlTopicStore() : this(null) { }

		public _MySqlTopicStore(string connStr)
			: base(MySqlClientFactory.Instance, connStr) {
		}

		public void RemoveMetadataFor(string parent) {
			this.Exec("delete from metadata where parent=?parent", _parameter("?parent", parent));
		}

		public void RemoveMetadata(string parent, string scope, string name) {
			this.Exec("delete from metadata where parent=?parent and scope=?scope and name=?name", _parameter("?parent", parent), _parameter("?scope", scope), _parameter("?name", name));
		}

		public void AddMetadata(IEnumerable<Metadata> metadata) {
			this.AddMetadata(metadata, true);
		}

		protected void AddMetadata(IEnumerable<Metadata> metadata, bool check) {
			foreach (Metadata meta in metadata) {
				this.AddMetadata(meta, check);
			}
		}

		public void AddMetadata(Metadata meta) {
			this.AddMetadata(meta, true);
		}

		protected void AddMetadata(Metadata meta, bool check) {
			this.SetMetadata(meta.Parent, meta.Scope, meta.Name, meta.Value, check);
		}

		public void SetMetdadata(string parent, string name, string value) {
			this.SetMetadata(parent, "default", name, value, true);
		}

		public void SetMetadata(string parent, string scope, string name, string value) {
			this.SetMetadata(parent, scope, name, value, true);
		}

		public void SetMetadata(string parent, string scope, string name, string value, bool check) {
			const string insertSql = @"insert into metadata (parent, name, value, scope) values (?parent, ?name, ?value, ?scope)";
			const string updateSql = @"update metadata set value=?value where parent=?parent and name=?name and scope=?scope";

			IDbDataParameter[] parameters = new[] {
				_parameter("?parent", parent),
				_parameter("?name", name),
				_parameter("?value", value),
				_parameter("?scope", scope)
			};

			if (check) {
				// we'll check if we need to do and update or insert
				Metadata meta = this.GetMetadata(parent, scope, name);
				if (meta == Metadata.Blank) {
					this.Exec(insertSql, parameters);
				} else if (meta.Value != value) {
					this.Exec(updateSql, parameters);
				}
			} else {
				// we'll assume that if the method is being called with no check
				// that the calling code will have already cleared a path for us
				this.Exec(insertSql, parameters);
			}
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
			const string sql = "select value from metadata where parent=?parent and name=?name and scope=?scope";

			using (IDataReader reader = this.Read(sql, _parameter("?parent", parent), _parameter("?name", name), _parameter("?scope", scope))) {
				while (reader.Read()) {
					return new Metadata(parent, scope, name, reader.ReadString("value"));
				}
			}
			return Metadata.Blank;
		}

		public IEnumerable<Metadata> GetMetadataFor(string parent) {
			// TODO: refactor to use _readMeta
			const string sql = "select * from metadata where parent=?parent";

			List<Metadata> result = new List<Metadata>();
			using (IDataReader reader = this.Read(sql, _parameter("?parent", parent))) {
				while (reader.Read()) {
					result.Add(new Metadata(parent, reader.ReadString("scope"), reader.ReadString("name"), reader.ReadString("value")));
				}
			}
			return result;
		}

		public void RemoveOccurrencesFor(string parent) {
			this.Exec("delete from occurrence where parent=?parent", _parameter("?parent", parent));
		}

		public void RemoveOccurrence(string parent, string scope, string role, string behaviour, string reference) {
			this.Exec(
				"delete from occurrence where parent=?parent and scope=?scope and role=?role and behaviour=?behaviour and reference=?reference",
				_parameter("?parent", parent),
				_parameter("?scope", scope),
				_parameter("?role", role),
				_parameter("?behaviour", behaviour),
				_parameter("?reference", reference)
			);
		}

		public void AddOccurrences(IEnumerable<Occurrence> occurrences) {
			this.AddOccurrences(occurrences, true);
		}

		protected void AddOccurrences(IEnumerable<Occurrence> occurrences, bool check) {
			foreach (Occurrence occurrence in occurrences) {
				this.AddOccurrence(occurrence, check);
			}
		}

		public void AddOccurrence(Occurrence occurrence) {
			this.AddOccurrence(occurrence, true);
		}

		protected void AddOccurrence(Occurrence occurrence, bool check) {
			this.SetOccurrence(occurrence.Parent, occurrence.Scope, occurrence.Role, occurrence.Behaviour, occurrence.Reference, occurrence.Data, check);
		}

		public void SetOccurrence(string parent, string role, string behaviour, string reference) {
			this.SetOccurrence(parent, "default", role, behaviour, reference);
		}

		//public void SetOccurrence(string parent, string scope, string role, string behaviour, string reference) {
		//	this.SetOccurrence(parent, scope, role, behaviour, reference);
		//}

		protected void SetOccurrence(string parent, string scope, string role, string behaviour, string reference, byte[] data = null, bool check = true) {
			const string insertSql = @"insert into occurrence (parent, scope, role, behaviour, reference, data) values (?parent, ?scope, ?role, ?behaviour, ?reference, ?data)";
			const string updateSql = @"update occurrence set reference=?reference, data=?data where parent=?parent and scope=?scope and role=?role and behaviour=?behaviour";

			IDbDataParameter[] parameters = new[] {
				 _parameter("?parent", parent),
				 _parameter("?scope", scope),
				 _parameter("?role", role),
				 _parameter("?behaviour", behaviour),
				 _parameter("?reference", reference),
				 _parameter("?data", data)
			};

			if (check) {
				// we'll need to check is an update or an insert is required
				Occurrence occurrence = this.GetOccurrence(parent, scope, role, behaviour, reference);
				if (occurrence == Occurrence.Blank) {
					this.Exec(insertSql, parameters);
				} else {
					this.Exec(updateSql, parameters);
				}
			} else {
				// we'll assume that is the method is being called with no check
				// the the calling code has already cleared a path for us
				this.Exec(insertSql, parameters);
			}
		}

		public Occurrence GetOccurrence(string parent, string role, string behaviour, string reference) {
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
		
		public Occurrence GetOccurrence(string parent, string scope, string role, string behaviour, string reference) {
			const string sql = @"select * from occurrence where parent=?parent and scope=?scope and role=?role and behaviour=?behaviour and reference=?reference";

			using (IDataReader reader = this.Read(sql, _parameter("?parent", parent), _parameter("?scope", scope), _parameter("?role", role), _parameter("?behaviour", behaviour), _parameter("?reference", reference))) {
				while (reader.Read()) {
					return new Occurrence(parent, scope, role, behaviour, reference, reader.ReadBinaryData("data"));
				}
			}
			return Occurrence.Blank;
		}

		public bool OccurrenceExists(string parent, string scope, string role, string behaviour, string reference) {
			return (this.GetOccurrence(parent, scope, role, behaviour, reference) != Occurrence.Blank);
		}

		public IEnumerable<Occurrence> GetOccurrencesFor(string parent) {
			const string sql = "select * from occurrence where parent=?parent";

			List<Occurrence> result = new List<Occurrence>();
			using (IDataReader reader = this.Read(sql, _parameter("?parent", parent))) {
				while (reader.Read()) {
					result.Add(new Occurrence(parent, reader.ReadString("scope"), reader.ReadString("role"), reader.ReadString("behaviour"), reader.ReadString("reference"), reader.ReadBinaryData("data")));
				}
			}
			return result;
		}

		/// <summary>
		/// Gets all the association ids in the store.
		/// </summary>
		/// <returns>
		/// Returns an an enumerable of all the association ids.
		/// </returns>
		public IEnumerable<string> GetAssociationIds() {
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

		public bool AssociationExists(string id) {
			const string sql = @"select id from association where id=?id";

			using (IDataReader reader = this.Read(sql, _parameter("?id", id))) {
				return reader.Read();
			}
		}

		public Association GetAssociation(string id) {
			const string assocQuery = @"
select 
	A.id as assoc_id,
	A.parent as assoc_parent,
	A.scope as assoc_scope,
	A.type as assoc_type,
	A.role as assoc_role,
	A.reference as assoc_ref,
	M.parent as meta_parent,
	M.scope as meta_scope,
	M.name as meta_name,
	M.value as meta_value
from 
	association as A
left join 
	metadata as M
on
	M.parent = A.id
where {0}
";

			// and M.scope=A.scope
			string whereClause = String.Format("A.id=?assocId");
			string sql = String.Format(assocQuery, whereClause);

			Association.Builder builder = null;
			using (IDataReader reader = this.Read(sql, _parameter("?assocId", id))) {
				while (reader.Read()) {
					if (builder == null) {
						// the first record can be read for assoc details
						builder = _readAssoc(reader);
					}
					_readMeta(reader, builder);
				}
			}

			return builder ?? Association.Blank;
		}

		public void RemoveAssociationsFor(string parent) {
			IDbDataParameter parameter = _parameter("?parent", parent);
			this.Exec(@"delete from metadata where parent in (select id from association where parent=?parent)", parameter);
			this.Exec(@"delete from association where parent=?parent", parameter);
		}

		public void RemovePointingAssociations(string reference) {
			throw new NotImplementedException();
		}

		public void RemoveAssociation(string id) {
			IDbDataParameter parameter = _parameter("?id", id);
			this.Exec(@"delete from metadata where parent=?id", parameter);
			this.Exec(@"delete from association where id=?id", parameter);
		}

		public void AddAssociations(IEnumerable<Association> associations) {
			this.AddAssociations(associations, true);
		}

		protected void AddAssociations(IEnumerable<Association> associations, bool check) {
			foreach (Association association in associations) {
				this.AddAssociation(association, check);
			}
		}

		public void AddAssociation(Association association) {
			this.AddAssociation(association, true);
		}

		protected void AddAssociation(Association association, bool check) {
			if (check) {
				this.RemoveAssociation(association.Id);
			}
			this.CreateAssociation(association.Id, association.Scope, association.Type, association.Role, association.Parent, association.Reference);
			this.AddMetadata(association.Metadata);
		}

		public void CreateAssociation(string id, string scope, string type, string role, string parent, string reference) {
			const string sql = @"insert into association (id, scope, type, role, parent, reference) values (?id, ?scope, ?type, ?role, ?parent, ?reference)";
			this.Exec(sql, _parameter("?id", id), _parameter("?scope", scope), _parameter("?type", type), _parameter("?role", role), _parameter("?parent", parent), _parameter("?reference", reference));
		}

		//public IEnumerable<Association> GetAssociationsFor(string parent) {

		//}

		/// <summary>
		/// Checks whether the specified topic exists in the store.
		/// </summary>
		/// <param name="topicId">The identity of the topic to check for.</param>
		/// <returns>Returns <b>true</b> if the topic exists; otherwise returns <b>false</b>.</returns>

		public bool TopicExists(string topicId) {
			//const string sql = @"select id from topic where id=?id";

			//using (IDataReader reader = this.Read(sql, _parameter("?id", topicId))) {
			//	return reader.Read();
			//}
			return this.Exists("select id from topic where id=?id", _parameter("?id", topicId));
		}

		/// <summary>
		/// Gets all the topic ids in the store.
		/// </summary>
		/// <returns>
		/// Returns an an enumerable of all the topic ids.
		/// </returns>
		public IEnumerable<string> GetTopicIds() {
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

		public void CreateTopic(string id) {
			const string sql = @"insert into topic (id) values (?id)";
			this.Exec(sql, _parameter("?id", id));
		}

		/// <summary>
		/// Removes the specified topic from the store.
		/// </summary>
		/// <param name="topicId">The identity of the topic to remove.</param>

		public void RemoveTopic(string topicId) {
			if (this.TopicExists(topicId)) {
				this.RemoveMetadataFor(topicId);
				this.RemoveOccurrencesFor(topicId);
				this.RemoveAssociationsFor(topicId);
				this.Exec("delete from topic where id=?topicId", _parameter("?topicId", topicId));
			}
		}

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

		private const string _topicQuerySqlTemplate = @"
/* finally add the metadata for the associations */
select
	topic_id, topic_meta_parent, topic_meta_scope, topic_meta_name, topic_meta_value,
	occur_parent, occur_scope, occur_role, occur_behaviour, occur_ref, occur_data,
	assoc_id, assoc_parent, assoc_type, assoc_scope, assoc_role, assoc_ref,
	AM.parent as assoc_meta_parent,
	AM.scope as assoc_meta_scope,
	AM.name as assoc_meta_name,
	AM.value as assoc_meta_value
from
	(/* 3.. add the assoc data to the topic/meta/occur data */
		select
			topic_id, topic_meta_parent, topic_meta_scope, topic_meta_name, topic_meta_value,
			occur_parent, occur_scope, occur_role, occur_behaviour, occur_ref, occur_data,
			A.id as assoc_id,
			A.parent as assoc_parent,
			A.type as assoc_type,
			A.scope as assoc_scope,
			A.role as assoc_role,
			A.reference as assoc_ref
		from
			(/* 2.. add the topic occurrence data to the topic/meta */
				select
					topic_id, topic_meta_parent, topic_meta_scope, topic_meta_name, topic_meta_value,
					O.parent as occur_parent,
					O.scope as occur_scope,
					O.role as occur_role,
					O.behaviour as occur_behaviour,
					O.reference as occur_ref,
					O.data as occur_data
				from
					(/* 1.. topic and metadata */
						select
							T.id as topic_id,
							M.parent as topic_meta_parent,
							M.scope as topic_meta_scope,
							M.name as topic_meta_name,
							M.value as topic_meta_value
						from topic as T
						left join metadata as M
						on T.id = M.parent {1}
						{0}
					) as TM
				left join occurrence as O
				on TM.topic_id = O.parent {1}
			) as TMO
		left join association as A
		on TMO.topic_id = A.parent {1}
	) as TMOA
left join metadata as AM
on TMOA.assoc_id = AM.parent {1}
order by topic_id
";

		private const string _topicQuerySqlTemplateAlt = @"
select 
	T.id as topic_id,
	TM.parent as topic_meta_parent,
	TM.scope as topic_meta_scope,
	TM.name as topic_meta_name,
	TM.value as topic_meta_value,
	O.parent as occur_parent,
	O.scope as occur_scope,
	O.role as occur_role,
	O.behaviour as occur_behaviour,
	O.reference as occur_ref,
	O.data as occur_data,
	A.id as assoc_id,
	A.parent as assoc_parent,
	A.type as assoc_type,
	A.scope as assoc_scope,
	A.role as assoc_role,
	A.reference as assoc_ref,
	AM.parent as assoc_meta_parent,
	AM.scope as assoc_meta_scope,
	AM.name as assoc_meta_name,
	AM.value as assoc_meta_value
from
	topic as T
left join
	metadata as TM
on
	T.id = TM.parent {1}
left join
	occurrence as O
on
	T.id = O.parent {1}
left join
	association as A
on
	T.id = A.parent {1}
left join
	metadata as AM
on
	A.id = AM.parent {1}
{0}
";

		protected IEnumerable<Topic.Builder> ProcessTopicQuery(string sql, params IDbDataParameter[] parms) {
			string topicId, assocId, occurParent, topicMetaParent, assocMetaParent;
			Topic.Builder currentTopic = null;
			Association.Builder currentAssoc = null;
			Occurrence.Builder currentOccur = null;
			Metadata.Builder currentTopicMeta = null;
			Metadata.Builder currentAssocMeta = null;
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

		public Topic GetTopic(string id) {
			string sql = String.Format(_topicQuerySqlTemplate, "where T.id=?id", "");
			IEnumerable<Topic.Builder> topics = this.ProcessTopicQuery(sql, _parameter("?id", id));
			return topics.FirstOrDefault() ?? Topic.Blank;
		}

		public Topic GetTopic(string id, string scope) {
			string sql = String.Format(_topicQuerySqlTemplate, "where T.id=?id", "and scope=?scope");
			IEnumerable<Topic.Builder> topics = this.ProcessTopicQuery(sql, _parameter("?id", id), _parameter("?scope", scope));
			return topics.FirstOrDefault() ?? Topic.Blank;
		}

	}
}
