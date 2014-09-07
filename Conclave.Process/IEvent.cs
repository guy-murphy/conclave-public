using System.Collections.Generic;
using System.Linq;

namespace Conclave.Process {
	public interface IEvent : IData {
		string this[string key] { get; }
		string Message { get; }
		IDictionary<string, string> Params { get; }
		void Add(string key, string value);
		ProcessContext Context { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// I felt compelled to put an escape hatch in here.
		/// It's wrong and I feel dirty, but I did it anyway.
		/// </remarks>
		object Object { get; set; }

		bool HasParams(params string[] parms);
		bool HasParamValues(IEnumerable<KeyValuePair<string, string>> match);
		bool HasRequiredParams(params string[] parms);
		IEvent Fire();
	}
}
