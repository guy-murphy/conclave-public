using System;

using Conclave.Collections;

namespace Conclave.Process {
	public static class ConcurrentDataDictionaryEx {

		public static void Add(this ConcurrentDataCollection<ErrorMessage> self, string message) {
			self.Add(new ErrorMessage(message));			
		}

		public static void Add(this ConcurrentDataCollection<ErrorMessage> self, string message, Exception exception) {
			self.Add(new ErrorMessage(message, exception));
		}
	}
}
