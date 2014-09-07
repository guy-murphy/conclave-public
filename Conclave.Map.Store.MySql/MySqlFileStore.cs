using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conclave.Data.Store;
using MySql.Data.MySqlClient;

namespace Conclave.Map.Store.MySql {

	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// This class is not thread safe.
	/// </remarks>

	public class MySqlFileStore: SqlStore {
		
		private static FileStoreResource.Buidler _readResource(IDataReader reader, bool readData = true) {
			FileStoreResource.Buidler builder = new FileStoreResource.Buidler()
			{
				Id = reader.ReadString("id"),
				Namespace = reader.ReadString("namespace"),
				Name = reader.ReadString("name"),
				Type = reader.ReadString("type"),
				Created = reader.ReadDateTime("created"),
				Opened = reader.ReadDateTime("opened"),
				Commited = reader.ReadDateTime("commited"),
				CreatedBy = reader.ReadString("created_by"),
				LockedBy = reader.ReadString("locked_by"),
				Rights = reader.ReadInt("rights"),
				Description = reader.ReadString("description"),
				Data = new byte[0]
			};
			if (readData) {
				builder.Data = reader.ReadBinaryData("data");
			}
			return builder;
		}

		private FileStoreResource _getInstanceFor(string sql, string ns, string name) {
			using (IDataReader reader = this.Read(sql, _parameter("?ns", ns), _parameter("?name", name))) {
				if (reader.Read()) {
					return _readResource(reader);
				} else {
					return FileStoreResource.Blank;
				}
			}
		}

		private Encoding _encoding = new UTF8Encoding();
		private string _namespace = "default";

		public virtual string DefaultFileType {
			get { return "default"; }
		}

		public Encoding StringEncoding {
			get { return _encoding; }
			set { _encoding = value; }
		}

		public string Namespace {
			get { return _namespace; }
			set { _namespace = value; }
		}

		public IEnumerable<string> Ids {
			get {
				List<string> ids = new List<string>();
				using (IDataReader reader = this.Read("select id from filestore")) {
					while (reader.Read()) {
						ids.Add(reader.ReadString("id"));
					}
				}
				return ids;
			}
		}

		public MySqlFileStore(string connStr)
			: base(MySqlClientFactory.Instance, connStr) {
		}

		public bool ResourceExists(string id) {
			using (IDataReader reader = this.Read("select id from filestore where id=?id", _parameter("?id", id))) {
				return reader.Read();
			}
		}

		public bool ResourceExists(string ns, string name) {
			using (IDataReader reader = this.Read("select id from filestore where namespace=?ns and name=?name", _parameter("?ns", ns), _parameter("?name", name))) {
				return reader.Read();
			}
		}

		public void AddResource(FileStoreResource resource) {
			const string sql = @"
insert into filestore
	(id, namespace, name, type, created, opened, commited, created_by, locked_by, rights, description, data)
values
	(?id, ?ns, ?name, ?type, ?created, ?opened, ?commited, ?createdBy, ?lockedBy, ?rights, ?description, ?data)
";

			this.RemoveResourceInstance(resource.Id);

			this.Exec(
				sql,
				_parameter("?id", resource.Id),
				_parameter("?ns", resource.Namespace),
				_parameter("?name", resource.Name),
				_parameter("?type", resource.Type),
				_parameter("?created", resource.CreatedOn),
				_parameter("?opened", resource.OpenedOn),
				_parameter("?commited", resource.CommitedOn),
				_parameter("?createdBy", resource.CreatedBy),
				_parameter("?LockedBy", resource.LockedBy),
				_parameter("?rights", resource.Rights),
				_parameter("?description", resource.Description),
				_parameter("?data", resource.Data)
			);

			if (String.IsNullOrEmpty(resource.LockedBy)) {
				//this.SynchroniseResource(resource);
			} else if (resource.LockedBy == "DELETED") {
				//this.CleanupResource(reasource);
			}

		}

		public void RemoveResourceInstance(string id) {
			FileStoreResource resource = this.GetResourceInstance(id);
			if (resource != FileStoreResource.Blank) {
				this.Exec("delete from filestore where id=?id", _parameter("?id", id));
			}
		}

		public void RemoveResource(string name) {
			this.RemoveResource(this.Namespace, name);
		}

		public void RemoveResource(string ns, string name) {
			this.Exec("delete from filestore where namespace=?ns and name=?name", _parameter("?ns", ns), _parameter("?name", name));
		}

		public FileStoreResource GetResourceInstance(string id) {
			using (IDataReader reader = this.Read("select * from filestore where id=?id", _parameter("?id", id))) {
				return _readResource(reader);
			}
		}

		public FileStoreResource GetLockedInstance(string name) {
			return this.GetLockedInstance(this.Namespace, name);
		}
		
		public FileStoreResource GetLockedInstance(string nameSpace, string name) {
			string sql = @"
				select 
					* from filestore
				where 
					namespace = ?ns and 
					name = ?fsName and
					locked_by is NOT NULL and
					opened = (
						select max(opened)
							from filestore
						where
							namespace = ?ns and 
							name = ?name
					) AND
					  opened > (
						select max(commited)
							from filestore
							where
								namespace = ?ns and 
								name = ?name and
								locked_by IS NULL
					  )";
			return _getInstanceFor(sql, nameSpace, name);
		}

	}
}
