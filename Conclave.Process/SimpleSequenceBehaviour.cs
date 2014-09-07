using System.Collections.Generic;

namespace Conclave.Process {
	public class SimpleSequenceBehaviour: ProcessBehaviour {

		private readonly IEnumerable<string> _sequence;

		protected SimpleSequenceBehaviour(string name, IEnumerable<string> sequence) : base(name) {
			_sequence = sequence;
		}

		public override void Action(IEvent ev, ProcessContext context) {
			foreach (string item in _sequence) {
				context.Timers.Begin(item);
				context.Fire(item);
				context.Timers.End(item);
			}
		}

	}
}
