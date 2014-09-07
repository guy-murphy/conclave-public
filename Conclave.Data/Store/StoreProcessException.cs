using System;
using System.Runtime.Serialization;

namespace Conclave.Data.Store {
	[Serializable]
	public sealed class StoreProcessException : ApplicationException {
		public StoreProcessException(string message) : base(message) { }
		private StoreProcessException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
