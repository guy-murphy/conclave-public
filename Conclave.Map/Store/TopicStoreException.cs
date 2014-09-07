using System;
using System.Runtime.Serialization;

namespace Conclave.Map.Store {

	[Serializable]
	public sealed class TopicStoreException : ApplicationException {
		public TopicStoreException(string message) : base(message) { }
		private TopicStoreException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

}
