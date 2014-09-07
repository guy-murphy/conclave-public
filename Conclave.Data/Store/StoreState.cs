using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conclave.Data.Store {
	/// <summary>
	/// Expresses the current state of a backing store.
	/// </summary>

	public enum StoreState {
		Unstarted,
		Stopped,
		Started
	}
}
