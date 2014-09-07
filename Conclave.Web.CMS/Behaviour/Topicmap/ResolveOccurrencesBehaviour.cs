using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conclave.Map.Model;
using Conclave.Process;
using Conclave.Web.Behaviour;

namespace Conclave.Web.CMS.Behaviour.Topicmap {
	public class ResolveOccurrencesBehaviour: ProcessBehaviour {
		public ResolveOccurrencesBehaviour(string message) : base(message) {}

		public override void Action(IEvent ev, ProcessContext context) {
			if (!context.ControlState.ContainsKey("topic")) {
				context.Errors.Add(new ErrorMessage("There was no topic to resolve occurrences for."));
			} else {
				Topic topic = context.ControlState["topic"] as Topic;
				if (topic == null) {
					context.Errors.Add(new ErrorMessage("The topic was of the incorrect type."));
				} else {
					foreach (Occurrence occur in topic.Occurrences) {
						Event resolve = new Event(context, "topicmap::resolve-occurrence", occur) 
						{
							{ "occurrence::behaviour", occur.Behaviour }
						};
						resolve.Fire();
					}
				}
			}
		}
	}
}
