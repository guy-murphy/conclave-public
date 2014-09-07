using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conclave.Process {
	public struct DoubleKeyValuePair<TValue> {

		private readonly string _key1;
		private readonly string _key2;
		private readonly TValue _value;

		public string Key1 {
			get { return _key1; }
		}

		public string Key2 {
			get { return _key2; }
		}

		public TValue Value {
			get { return _value; }
		}

		public DoubleKeyValuePair(string k1, string k2, TValue value) {
			_key1 = k1;
			_key2 = k2;
			_value = value;
		}

	}
}
