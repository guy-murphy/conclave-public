using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Conclave.Data.Store {
	interface ISqlStore: IStore {

		bool InTransaction { get; }
		TransactionWrapper Transaction { get; }
		
		void Exec(string sql);
		void Exec(string sql, params IDbDataParameter[] parameters);

		void Fill(string sql, DataSet data);
		void Fill(string sql, DataSet data, params IDbDataParameter[] parameters);
		
		DataSet Query(string sql, params IDbDataParameter[] parameters);
		DataSet Query(string sql);

		IDataReader Read(string sql, params IDbDataParameter[] parameters);
		IDataReader Read(string sql);

		bool Exists(string sql);
		bool Exists(string sql, params IDbDataParameter[] parameters);

        object Scalar(string sql, params IDbDataParameter[] parameters);
		object Scalar(string sql);

	}
}
