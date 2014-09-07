using System;
using System.Collections.Generic;

namespace Conclave.Process {
	/// <summary>
	/// A process behaviour with scoped lists and dictionaries made
	/// available for configuration of the condition for an action.
	/// </summary>
	public class ProcessActionBehaviour: ProcessBehaviour {

		private readonly ConditionLists _conditionalLists = new ConditionLists();

		public ConditionLists ConditionalLists {
			get { return _conditionalLists; }
		} 

		/// <summary>
		/// Creates a new instance of the behaviour.
		/// </summary>
		/// <param name="message">The name of the behaviour.</param>
		//public ProcessActionBehaviour(string message) : this(message, null) {}

		public ProcessActionBehaviour(string message, Dictionary<string, Dictionary<string, List<string>>> config): base(message) {
			if (config != null) {
				_conditionalLists = new ConditionLists(config);
			}
		}

		/// <summary>
		/// The action to perform when the `Condition(IEvent)` is met.
		/// </summary>
		/// <param name="ev">The event to consult.</param>
		/// <param name="context">The context upon which to perform any action.</param>
		public override void Action(IEvent ev, ProcessContext context) {
			throw new NotImplementedException();
		}
	}
}
