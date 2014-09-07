namespace Conclave.Data.Store {
	public abstract class Store: IStore {

		private StoreState _state = StoreState.Unstarted;


		public bool HasStarted {
			get { return _state == StoreState.Started; }
		}
		
		public virtual void Start() {
			if (_state == StoreState.Started) throw new StoreStartedException("The store you are attempting to start has already been started.");

			_state = StoreState.Started;
		}

		public virtual void Stop() {
			if (_state == StoreState.Started) {
				_state = StoreState.Stopped;
			}
		}

		public abstract void Dispose();

	}
}
