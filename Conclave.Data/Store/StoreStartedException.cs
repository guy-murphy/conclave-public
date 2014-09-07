using System;
using System.Runtime.Serialization;

namespace Conclave.Data.Store {
	/// <summary>
	/// An exception throw when an attempt is made
	/// to start a store that is already open.
	/// </summary>

	[Serializable]
	public sealed class StoreStartedException : ApplicationException {
		public StoreStartedException(string message) : base(message) { }
		private StoreStartedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
