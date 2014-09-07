using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

using Conclave.Data.Store;
using Conclave.Map.Model;

namespace Conclave.Map.Store {

	/// <summary>
	/// An <see cref="ITopicStore"/> for Sql Server.
	/// </summary>

	public class SqlServerTopicStore: SqlTopicStore {

		public SqlServerTopicStore(string connStr)
			: base(SqlClientFactory.Instance, connStr) {
		}

		public override void RemoveMetadataFor(string parent) {
			this.Exec("delete from metadata where parent=@parent", _parameter("@parent", parent));
		}

		public override void RemoveMetadata(string parent, string scope, string name) {
			this.Exec("delete from metadata where parent=@parent and scope=@scope and name=@name", _parameter("@parent", parent), _parameter("@scope", scope), _parameter("@name", name));
		}

		public override void SetMetadata(string parent, string scope, string name, string value, bool check) {
			const string insertSql = @"insert into metadata (parent, name, value, scope) values (@parent, @name, @value, @scope)";
			const string updateSql = @"update metadata set value=@value where parent=@parent and name=@name and scope=@scope";

			IDbDataParameter[] parameters = {
				_parameter("@parent", parent),
				_parameter("@name", name),
				_parameter("@value", value),
				_parameter("@scope", scope)
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

		public override Metadata GetMetadata(string parent, string scope, string name) {
			const string sql = "select value from metadata where parent=@parent and name=@name and scope=@scope";
			// move to extension
			using (IDataReader reader = this.Read(sql, _parameter("@parent", parent), _parameter("@name", name), _parameter("@scope", scope))) {
				while (reader.Read()) {
					return new Metadata(parent, scope, name, reader.ReadString("value"));
				}
			}
			return Metadata.Blank;
		}

		public override IEnumerable<Metadata> GetMetadataFor(string parent) {
			// TODO: refactor to use _readMeta
			const string sql = "select * from metadata where parent=@parent";
			// move to extension
			List<Metadata> result = new List<Metadata>();
			using (IDataReader reader = this.Read(sql, _parameter("@parent", parent))) {
				while (reader.Read()) {
					result.Add(new Metadata(parent, reader.ReadString("scope"), reader.ReadString("name"), reader.ReadString("value")));
				}
			}
			return result;
		}

		public override void RemoveOccurrencesFor(string parent) {
			this.Exec("delete from occurrence where parent=@parent", _parameter("@parent", parent));
		}

		public override void RemoveOccurrence(string parent, string scope, string role, string behaviour, string reference) {
			this.Exec(
				"delete from occurrence where parent=@parent and scope=@scope and role=@role and behaviour=@behaviour and reference=@reference",
				_parameter("@parent", parent),
				_parameter("@scope", scope),
				_parameter("@role", role),
				_parameter("@behaviour", behaviour),
				_parameter("@reference", reference)
			);
		}

		protected override void SetOccurrence(string parent, string scope, string role, string behaviour, string reference, byte[] data = null, bool check = true) {
			const string insertSql = @"insert into occurrence (parent, scope, role, behaviour, reference, data) values (@parent, @scope, @role, @behaviour, @reference, @data)";
			const string updateSql = @"update occurrence set reference=@reference, data=@data where parent=@parent and scope=@scope and role=@role and behaviour=@behaviour";

			IDbDataParameter[] parameters = {
				 _parameter("@parent", parent),
				 _parameter("@scope", scope),
				 _parameter("@role", role),
				 _parameter("@behaviour", behaviour),
				 _parameter("@reference", reference),
				 _parameter("@data", data)
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

		public override Occurrence GetOccurrence(string parent, string scope, string role, string behaviour, string reference) {
			const string sql = @"select * from occurrence where parent=@parent and scope=@scope and role=@role and behaviour=@behaviour and reference=@reference";
			// move to extension
			using (IDataReader reader = this.Read(sql, _parameter("@parent", parent), _parameter("@scope", scope), _parameter("@role", role), _parameter("@behaviour", behaviour), _parameter("@reference", reference))) {
				while (reader.Read()) {
					return new Occurrence(parent, scope, role, behaviour, reference, reader.ReadBinaryData("data"));
				}
			}
			return Occurrence.Blank;
		}

		public override IEnumerable<Occurrence> GetOccurrencesFor(string parent) {
			const string sql = "select * from occurrence where parent=@parent";
			// move to extension
			List<Occurrence> result = new List<Occurrence>();
			using (IDataReader reader = this.Read(sql, _parameter("@parent", parent))) {
				while (reader.Read()) {
					result.Add(new Occurrence(parent, reader.ReadString("scope"), reader.ReadString("role"), reader.ReadString("behaviour"), reader.ReadString("reference"), reader.ReadBinaryData("data")));
				}
			}
			return result;
		}

		public override bool AssociationExists(string id) {
			const string sql = @"select id from association where id=@id";
			// move to extension
			using (IDataReader reader = this.Read(sql, _parameter("@id", id))) {
				return reader.Read();
			}
		}

		public override Association GetAssociation(string id) {
			// move assoc query to a virtual protected property
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
			string whereClause = String.Format("A.id=@assocId");
			string sql = String.Format(assocQuery, whereClause);

			Association.Builder builder = null;
			using (IDataReader reader = this.Read(sql, _parameter("@assocId", id))) {
				while (reader.Read()) {
					if (builder == null) {
						// the first record can be read for assoc details
						builder = reader.ReadAssociation();
					}
					reader.ReadMetadata(builder);
				}
			}

			return builder ?? Association.Blank;
		}

		public override void UpdateMetadataForPointingAssociations(
			string id, 
			string assocType, string assocRole, 
			string metaScope,string metaName, string metaValue
		) {
			const string assocQuery = @"
update metadata 
set value=@MetaValue
where
	metadata.scope=@MetaScope and metadata.Name=@MetaName and metadata.parent in (
		select A.id from association as A
		where A.type=@AssocType and A.role=@AssocRole and A.reference=@Id
	)
";		
			this.Exec(assocQuery, 
				_parameter("@Id", id),
				_parameter("@AssocType", assocType),
				_parameter("@AssocRole", assocRole),
				_parameter("@MetaScope", metaScope),
				_parameter("@MetaName", metaName),
				_parameter("@MetaValue", metaValue)
			);
		}

		public override void RemoveAssociationsFor(string parent) {
			// you can't share parameters in Sql Server client
			this.Exec(@"delete from metadata where parent in (select id from association where parent=@parent)", _parameter("@parent", parent));
			this.Exec(@"delete from association where parent=@parent", _parameter("@parent", parent));
		}

		public override void RemovePointingAssociations(string reference) {
			// you can't share parameters in Sql Server client
			this.Exec(@"delete from metadata where parent in (select id from association where reference=@reference)", _parameter("@reference", reference));
			this.Exec(@"delete from association where reference=@reference", _parameter("@reference", reference));
		}

		public override void RemoveAssociation(string id) {
			// you can't share parameters in Sql Server client
			this.Exec(@"delete from metadata where parent=@id", _parameter("@id", id));
			this.Exec(@"delete from association where id=@id", _parameter("@id", id));
		}

		public override void CreateAssociation(string id, string scope, string type, string role, string parent, string reference) {
			const string sql = @"insert into association (id, scope, type, role, parent, reference) values (@id, @scope, @type, @role, @parent, @reference)";
			this.Exec(sql, _parameter("@id", id), _parameter("@scope", scope), _parameter("@type", type), _parameter("@role", role), _parameter("@parent", parent), _parameter("@reference", reference));
		}

		public override bool TopicExists(string topicId) {
			return this.Exists("select id from topic where id=@id", _parameter("@id", topicId));
		}

		public override void CreateTopic(string id) {
			const string sql = @"insert into topic (id) values (@id)";
			this.Exec(sql, _parameter("@id", id));
		}

		public override void RemoveTopic(string topicId) {
			if (this.TopicExists(topicId)) {
				this.RemoveMetadataFor(topicId);
				this.RemoveOccurrencesFor(topicId);
				this.RemoveAssociationsFor(topicId);
				this.Exec("delete from topic where id=@topicId", _parameter("@topicId", topicId));
			}
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

		public override Topic GetTopic(string id) {
			string sql = String.Format(_topicQuerySqlTemplate, "where T.id=@id", "");
			IEnumerable<Topic.Builder> topics = this.ProcessTopicQuery(sql, _parameter("@id", id));
			return topics.FirstOrDefault() ?? Topic.Blank;
		}

		public override Topic GetTopic(string id, string scope) {
			string sql = String.Format(_topicQuerySqlTemplate, "where T.id=?id", "and scope=?scope");
			IEnumerable<Topic.Builder> topics = this.ProcessTopicQuery(sql, _parameter("?id", id), _parameter("?scope", scope));
			return topics.FirstOrDefault() ?? Topic.Blank;
		}
	}
}
