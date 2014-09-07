using System;

namespace Conclave.Data.Store {

	/// <summary>
	/// Base interface for a backing store.
	/// </summary>

	public interface IStore : IDisposable {
		bool HasStarted { get; }

		void Start();
		void Stop();

	}
}
