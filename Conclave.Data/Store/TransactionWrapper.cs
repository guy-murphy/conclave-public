using System;
using System.Data;

namespace Conclave.Data.Store {
	public class TransactionWrapper : IDisposable, IDbTransaction {

		private readonly SqlStore _store;
		private bool _isDisposed;
		private bool _isRolledBack;
		private bool _isCommited;

		public TransactionWrapper(SqlStore store) {
			_store = store;
			_store.BeginTransaction();
		}

		~TransactionWrapper() {
			this.Dispose(false);
		}

		public void Dispose() {
			if (!_isRolledBack && !_isCommited) {
				this.Commit();
			}
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing) {
			if (!_isDisposed) {
				if (disposing) {
					_store.EndTransaction();
				}
			}
			_isDisposed = true;
		}

		#region IDbTransaction Members

		public void Commit() {
			_store.CommitTransaction();
			_isCommited = true;
		}

		public IDbConnection Connection {
			get { return _store.Connection; }
		}

		public IsolationLevel IsolationLevel {
			get { return IsolationLevel.ReadCommitted; }
		}

		public void Rollback() {
			_store.RollbackTransaction();
			_isRolledBack = true;
		}

		#endregion
	}
}
