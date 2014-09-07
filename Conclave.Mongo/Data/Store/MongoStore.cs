using MongoDB.Bson;
using MongoDB.Driver;

namespace Conclave.Data.Store {

	public class MongoStore: Store {

		private readonly MongoClient _client;
		private readonly MongoServer _server;
		private readonly MongoDatabase _db;

		public MongoCollection<BsonDocument> this[string collectionName] {
			get { return this.Database.GetCollection(collectionName); }
		}

		public MongoClient Client {
			get { return _client; }
		}

		public MongoServer Server {
			get { return _server; }
		}

		public MongoDatabase Database {
			get { return _db; }
		}

		public MongoStore(string connStr, string dbName) {
			_client = new MongoClient(connStr);
			_server = _client.GetServer();
			_db = _server.GetDatabase(dbName);
		}

		public sealed override void Dispose() {
			// nada
		}
	}
}
