using System;
using System.Data;
using Conclave.Data.Store;
using MySql.Data.MySqlClient;

namespace Conclave.Process.User {
	public class MySqlUserCredentialsStore: SqlStore, IUserCredentialsStore {

		private UserCredentials _readCredentials(IDataReader reader) {
			UserCredentials.Builder credentials = new UserCredentials.Builder() 
			{
				Id = reader.ReadString("id"),
				Name = reader.ReadString("name"),
				Email = reader.ReadString("email"),
				Password = reader.ReadString("pwd"),
				Mask = reader.ReadLong("mask")
			};
			return credentials;
		}

		public MySqlUserCredentialsStore(string connStr)
			: base(MySqlClientFactory.Instance, connStr) {
		}

		public bool UserCredentialsExist(string id) {
			return this.Exists("select id from credentials where id=?id", _parameter("?id", id));
		}

		public bool UserCredentialsExist(string name, string pwd) {
			return this.Exists("select id from credentials where name=?name and pwd=?pwd", _parameter("?name", name), _parameter("?pwd", pwd));
		}
		
		public UserCredentials GetUserCredentials(string id) {
			string sql = "select * from credentials where id=?id";
			using (IDataReader reader = this.Read(sql, _parameter("?id", id))) {
				return reader.Read() ? _readCredentials(reader) : UserCredentials.Blank;
			}
		}

		public UserCredentials GetUserCredentials(string name, string pwd) {
			string sql = "select * from credentials where name=?name and pwd=?pwd";
			using (IDataReader reader = this.Read(sql, _parameter("?name", name), _parameter("?pwd", pwd))) {
				return reader.Read() ? _readCredentials(reader) : UserCredentials.Blank;
			}
		}

		public string CreateUserCredentials(string name, string email, string pwd, long mask) {
			string id = Guid.NewGuid().ToString();
			this.CreateUserCredentials(id, name, email, pwd, mask);
			return id;
		}

		public void CreateUserCredentials(string id, string name, string email, string pwd, long mask) {
			if (this.UserCredentialsExist("id")) {
				// probably best not to let this exception bubble to the surface
				throw new InvalidOperationException("A user with that id already exists.");
			} else {
				if (this.Exists("select id from credentials where name=?name", _parameter("?name", name))) {
					throw new InvalidOperationException("Invalid name");
				} else {
					this.Exec("insert into credentials (id,name,email,pwd,mask) value (?id,?name,?email,?pwd,?mask)",
						  _parameter("?id", id),
						  _parameter("?name", name),
						  _parameter("?email", email),
						  _parameter("?pwd", pwd),
						  _parameter("?mask", mask)
					);
				}
			}
		}

		public void RemoveUserCredentials(string id) {
			this.Exec("delete from credentials where id=?id", _parameter("id", id));
		}

		public void UpdateEmail(string id, string email) {
			if (this.UserCredentialsExist(id)) {
				this.Exec("update credentials set email=?email where id=?id", _parameter("?id", id), _parameter("?email", email));
			}
		}

		public void UpdatePassword(string id, string pwd) {
			if (this.UserCredentialsExist(id)) {
				this.Exec("update credentials set pwd=?pwd where id=?id", _parameter("?id", id), _parameter("?pwd", pwd));
			}
		}

		public void UpdateMask(string id, long mask) {
			if (this.UserCredentialsExist(id)) {
				this.Exec("update credentials set mask=?mask where id=?id", _parameter("?id", id), _parameter("?mask", mask));
			}
		}
	}
}
