using System;
using System.Data;
using System.Data.Common;

namespace Conclave.Data.Store {
	public class SqlStore : Store, ISqlStore {
		// this in truth should be an abstract class
		// it's a really handy utility class to use
		// directly however, so I've left it alone

		private bool _isDisposed;
		private DbProviderFactory _factory;
		private IDbConnection _connection;
		private IDbTransaction _transaction;

		protected virtual DbProviderFactory Factory {
			get { return _factory; }
			set { _factory = value; }
		}

		public virtual IDbConnection Connection {
			get { return _connection; }
		}

		public bool InTransaction {
			get { return _transaction != null; }
		}

		public TransactionWrapper Transaction {
			get { return new TransactionWrapper(this); }
		}

		public SqlStore() : this(null) { }
		public SqlStore(DbProviderFactory factory) : this(factory, null) { }

		public SqlStore(DbProviderFactory factory, string connStr) {
			_factory = factory;
			_connection = _factory.CreateConnection();

			if (_connection != null) {
				_connection.ConnectionString = connStr;
			} else {
				throw new StoreProcessException("Unable to obtain a connection from the db factory.");
			}
		}

		~SqlStore() {
			this.Dispose(false);
		}

		public IDbDataParameter CreateParamater(string name, string value) {
			return _parameter(name, value);
		}

		protected virtual IDbDataParameter _parameter(string name, string value) {
			IDbDataParameter parameter = _factory.CreateParameter();
			if (parameter == null) throw new StoreProcessException("Unable to obtain a parameter object from the factory.");
			parameter.DbType = DbType.String;
			parameter.ParameterName = name;
			parameter.Value = value;
			return parameter;
		}

		protected virtual IDbDataParameter _parameter(string name, DateTime value) {
			IDbDataParameter parameter = _factory.CreateParameter();
			if (parameter == null) throw new StoreProcessException("Unable to obtain a parameter object from the factory.");

			parameter.DbType = DbType.DateTime2;
			parameter.ParameterName = name;
			parameter.Value = value;
			return parameter;
		}

		protected virtual IDbDataParameter _parameter(string name, int value) {
			IDbDataParameter parameter = _factory.CreateParameter();
			if (parameter == null) throw new StoreProcessException("Unable to obtain a parameter object from the factory.");

			parameter.DbType = DbType.Int32;
			parameter.ParameterName = name;
			parameter.Value = value;
			return parameter;
		}

		protected virtual IDbDataParameter _parameter(string name, long value) {
			IDbDataParameter parameter = _factory.CreateParameter();
			if (parameter == null) throw new StoreProcessException("Unable to obtain a parameter object from the factory.");

			parameter.DbType = DbType.Int64;
			parameter.ParameterName = name;
			parameter.Value = value;
			return parameter;
		}

		protected virtual IDbDataParameter _parameter(string name, byte[] value) {
			IDbDataParameter parameter = _factory.CreateParameter();
			if (parameter == null) throw new StoreProcessException("Unable to obtain a parameter object from the factory.");

			parameter.DbType = DbType.Binary;
			parameter.ParameterName = name;
			parameter.Value = value;
			return parameter;
		}

        protected virtual IDbDataParameter _parameter(string name, bool value)
        {
            IDbDataParameter parameter = _factory.CreateParameter();
            if (parameter == null) throw new StoreProcessException("Unable to obtain a parameter object from the factory.");

            parameter.DbType = DbType.Boolean;
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

        protected virtual IDbDataParameter _parameter(string name, Guid value)
        {
            IDbDataParameter parameter = _factory.CreateParameter();
            if (parameter == null) throw new StoreProcessException("Unable to obtain a parameter object from the factory.");

            parameter.DbType = DbType.Guid;
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
        }

        protected virtual IDbDataParameter _parameterNull(string name, DbType dbType)
        {
            IDbDataParameter parameter = _factory.CreateParameter();
            if (parameter == null) throw new StoreProcessException("Unable to obtain a parameter object from the factory.");

            parameter.DbType = dbType;
            parameter.ParameterName = name;
            parameter.Value = DBNull.Value;
            return parameter;
        }

		protected virtual IDbDataParameter[] _sanitiseParameters(params IDbDataParameter[] parms) {
			foreach (IDbDataParameter parm in parms) {
				if (parm.Value == null) {
					parm.Value = DBNull.Value;
				}
			}
			return parms;
		}

		protected virtual IDbDataAdapter CreateAdapter(string sql) {
			IDbDataAdapter adapter = _factory.CreateDataAdapter();
			if (adapter == null) throw new StoreProcessException("Unable to obtain an adapter object from the factory.");

			adapter.SelectCommand = this.CreateCommand(sql);
			return adapter;
		}

		protected virtual IDbCommand CreateCommand(string sql) {
			IDbCommand command = _factory.CreateCommand();
			if (command == null) throw new StoreProcessException("Unable to obtain a command object from the factory.");

			command.Connection = _connection;
			// this should only be used for parameterised queries
			// come up with a way to enforce this
			command.CommandText = sql;
			if (this.InTransaction) {
				command.Transaction = _transaction;
			}
			return command;
		}

		public sealed override void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (!_isDisposed) {
				if (this.InTransaction) {
					_transaction.Rollback();
					_transaction.Dispose();
				}
				this.Stop();
				_connection.Dispose();
				_connection = null;
				_isDisposed = true;
			}
		}

		public override void Start() {
			base.Start();
			if (_connection != null) {
				try {
					_connection.Open();
				} catch {
					throw new StoreProcessException("Unable to start the underlying connection for the store.");
				}
			}
		}

		public override void Stop() {
			base.Stop();
			_connection.Close();
		}

		internal virtual void BeginTransaction() {
			_transaction = _connection.BeginTransaction(IsolationLevel.ReadCommitted);
		}

		internal virtual void EndTransaction() {
			_transaction.Dispose();
			_transaction = null;
		}

		internal virtual void CommitTransaction() {
			_transaction.Commit();
		}

		internal virtual void RollbackTransaction() {
			_transaction.Rollback();
		}

		/// <summary>
		/// Executes a SQL statement against the server.
		/// </summary>
		/// <param name="sql">The SQL statement to be executed.</param>
		/// <returns>
		/// Returns <b>true</b> is the statement executes successfully and is commited;
		/// otherwise returns <b>false</b>, indicating the transaction has been
		/// rolled back.
		/// </returns>
		/// <remarks>
		/// This method should be used for updates and inserts. It is not
		/// suitable for queries.
		/// </remarks>

		public virtual void Exec(string sql) {
			using (IDbCommand command = this.CreateCommand(sql)) {
				try {
					command.ExecuteNonQuery();
				} catch {
					if (this.InTransaction) {
						_transaction.Rollback();
					}
					throw;
				}
			}
		}

		/// <summary>
		/// Executes a parameterised SQL statement against the server with the
		/// specified parameters.
		/// </summary>
		/// <param name="sql">The SQL statement to be executed.</param>
		/// <param name="parameters">The parameters for the SQL statement.</param>
		/// <returns>
		/// Returns <b>true</b> is the statement executes successfully and is commited;
		/// otherwise returns <b>false</b>, indicating the transaction has been
		/// rolled back.
		/// </returns>
		/// <remarks>
		/// This method should be used for updates and inserts. It is not
		/// suitable for queries.
		/// </remarks>

		public virtual void Exec(string sql, params IDbDataParameter[] parameters) {
			using (IDbCommand command = this.CreateCommand(sql)) {
				foreach (IDbDataParameter parameter in parameters) {
					command.Parameters.Add(parameter);
				}
				try {
					command.ExecuteNonQuery();
				} catch {
					if (this.InTransaction) {
						_transaction.Rollback();
					}
					throw;
				}
			}
		}

        /// <summary>
        /// Executes a scalar SQL query against the server and returns the result.
        /// </summary>
        /// <param name="sql">The SQL statement to be executed against the server.</param>
        /// <param name="parameters">The parameters for the SQL statement.</param>
        /// <returns>The resulting object from the query.</returns>

        public virtual object Scalar(string sql, params IDbDataParameter[] parameters)
        {
            using (IDbCommand command = this.CreateCommand(sql))
            {
                foreach (IDbDataParameter parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
                try
                {
                    return command.ExecuteScalar();
                }
                catch
                {
                    if (this.InTransaction)
                    {
                        _transaction.Rollback();
                    }
                    throw;
                }
            }
        }

		/// <summary>
		/// Executes a scalar SQL query against the server and returns the result.
		/// </summary>
		/// <param name="sql">The SQL statement to be executed against the server.</param>
		/// <returns>The resulting object from the query.</returns>

		public virtual object Scalar(string sql) {
			using (IDbCommand command = this.CreateCommand(sql)) {
				return command.ExecuteScalar();
			}
		}

		/// <summary>
		/// Executes a SQL query against the server and returns a <see cref="DataSet"/>
		/// or the results.
		/// </summary>
		/// <param name="sql">The SQL to be executed against the server.</param>
		/// <returns>The <see cref="DataSet"/> of the query results.</returns>

		public virtual DataSet Query(string sql) {
			DataSet data = new DataSet
			{
				Locale = System.Globalization.CultureInfo.InvariantCulture
			};
			IDbDataAdapter adapter = this.CreateAdapter(sql);
			using (adapter.SelectCommand) {
				adapter.Fill(data);
			}
			return data;
		}

		/// <summary>
		/// Executes a parameterised SQL statement against the server with the
		/// specified parameters.
		/// </summary>
		/// <param name="sql">The SQL statement to be executed.</param>
		/// <param name="parameters">The parameters for the SQL statement.</param>
		/// <returns>The <see cref="DataSet"/> of the query results.</returns>

		public virtual DataSet Query(string sql, params IDbDataParameter[] parameters) {
			DataSet data = new DataSet
			{
				Locale = System.Globalization.CultureInfo.InvariantCulture
			};
			IDbDataAdapter adapter = this.CreateAdapter(sql);
			using (adapter.SelectCommand) {
				foreach (IDbDataParameter parameter in parameters) {
					adapter.SelectCommand.Parameters.Add(parameter);
				}
				adapter.Fill(data);
			}
			return data;
		}

		/// <summary>
		/// Executes a SQL statement against the server and <i>fills</i>
		/// a provided <see cref="DataSet"/> with the results.
		/// </summary>
		/// <param name="sql">The SQL statement to execute against the server.</param>
		/// <param name="data">The <see cref="DataSet"/> to be filled with the results.</param>

		public virtual void Fill(string sql, DataSet data) {
			IDbDataAdapter adapter = this.CreateAdapter(sql);
			using (adapter.SelectCommand) {
				adapter.Fill(data);
			}
		}

		/// <summary>
		/// Executes a parameterised SQL statement against the server and <i>fills</i> a
		/// provided <see cref="DataSet"/> with the results.
		/// </summary>
		/// <param name="sql">The SQL statement to be executed against the server.</param>
		/// <param name="data">The <see cref="DataSet"/> to be filled with the results.</param>
		/// <param name="parameters">The parameters for the SQL statement.</param>

		public virtual void Fill(string sql, DataSet data, params IDbDataParameter[] parameters) {
			// no checking if store is open, as one shouldn't get this far if it isn't
			IDbDataAdapter adapter = this.CreateAdapter(sql);
			using (adapter.SelectCommand) {
				foreach (IDbDataParameter parameter in parameters) {
					adapter.SelectCommand.Parameters.Add(parameter);
				}
				adapter.Fill(data);
			}
		}

		/// <summary>
		/// Executes a SQL query against the server and returns a <see cref="DbDataReader"/>
		/// with which to read the results.
		/// </summary>
		/// <param name="sql">The SQL query to execute against the server.</param>
		/// <returns>An <see cref="DbDataReader"/> with which to reader the results.</returns>

		public virtual IDataReader Read(string sql) {
			using (IDbCommand command = this.CreateCommand(sql)) {
				return command.ExecuteReader();
			}
		}

		/// <summary>
		/// Executes a parameterised SQL query against the server and returns a <see cref="DbDataReader"/>
		/// with which to read the results.
		/// </summary>
		/// <param name="sql">The SQL query to execute against the server.</param>
		/// <param name="parameters">The parameters for the SQL query.</param>
		/// <returns>An <see cref="DbDataReader"/> with which to reader the results.</returns>

		public virtual IDataReader Read(string sql, params IDbDataParameter[] parameters) {
			using (IDbCommand command = this.CreateCommand(sql)) {
				foreach (IDbDataParameter paramter in parameters) {
					command.Parameters.Add(paramter);
				}
				return command.ExecuteReader();
			}
		}

		/// <summary>
		/// Determines whether or not there are any results returned
		/// from a provided query.
		/// </summary>
		/// <param name="sql">The sql to be executed.</param>
		/// <returns>
		/// Returns <b>true</b> if any results are returned; otherwise,
		/// returns <b>false</b>.
		/// </returns>
		public virtual bool Exists(string sql) {
			using (IDataReader reader = this.Read(sql)) {
				return reader.Read();
			}
		}

		/// <summary>
		/// Determines whether or not there are any results returned
		/// from a provided query.
		/// </summary>
		/// <param name="sql">The sql to be executed.</param>
		/// <param name="parameters">Any query parameters used by the sql.</param>
		/// <returns>
		/// Returns <b>true</b> if any results are returned; otherwise,
		/// returns <b>false</b>.
		/// </returns>
		public virtual bool Exists(string sql, params IDbDataParameter[] parameters) {
			using (IDataReader reader = this.Read(sql, parameters)) {
				return reader.Read();
			}
		}

	}
}
