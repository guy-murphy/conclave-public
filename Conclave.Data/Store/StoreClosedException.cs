using System;
using System.Runtime.Serialization;

namespace Conclave.Data.Store {
	/// <summary>
	/// An exception throw when an attempt is made
	/// to close a store that is already closed.
	/// </summary>

	[Serializable]
	public sealed class StoreClosedException : ApplicationException {
		public StoreClosedException(string message) : base(message) { }
        private StoreClosedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
