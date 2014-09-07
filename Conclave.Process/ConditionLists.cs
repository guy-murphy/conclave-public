using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Conclave.Process {
	public class ConditionLists: IEnumerable<DoubleKeyValuePair<IEnumerable<string>>> {

		private readonly HashSet<DoubleKeyValuePair<IEnumerable<string>>> _lists = new HashSet<DoubleKeyValuePair<IEnumerable<string>>>();

		public IEnumerable<string> this[string k1, string  k2 ] {
			get {
				foreach (DoubleKeyValuePair<IEnumerable<string>> entry in _lists) {
					if (entry.Key1 == k1 && entry.Key2 == k2) {
						return entry.Value;
					}
				}
				return new string[0];
			}
			set {
				if (_lists.Any(entry => entry.Key1 == k1 && entry.Key2 == k2)) {
					throw new InvalidOperationException("You may only assign once to a given key pair.");
				}
				_lists.Add(new DoubleKeyValuePair<IEnumerable<string>>(k1, k2, ImmutableList.Create<string>(value.ToArray())));
			}
		}

		public ConditionLists() { }

		public ConditionLists(Dictionary<string, Dictionary<string, List<string>>> config) {
			foreach (KeyValuePair<string, Dictionary<string, List<string>>> outer in config) {
				foreach (KeyValuePair<string, List<string>> inner in outer.Value) {
					this[outer.Key, inner.Key] = inner.Value;
				}
			}
		}


		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<DoubleKeyValuePair<IEnumerable<string>>> GetEnumerator() {
			return _lists.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
