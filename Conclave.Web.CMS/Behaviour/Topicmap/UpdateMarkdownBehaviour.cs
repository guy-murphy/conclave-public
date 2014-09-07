using Conclave.Map.Model;
using Conclave.Map.Store;
using Conclave.Process;

namespace Conclave.Web.CMS.Behaviour.Topicmap {
	class UpdateMarkdownBehaviour : ProcessBehaviour {
		public UpdateMarkdownBehaviour(string message) : base(message) { }

		public override bool Condition(IEvent ev, ProcessContext context) {
			return base.Condition(ev, context) && ev.HasParams("string-data");
		}

		public override void Action(IEvent ev, ProcessContext context) {
			Topic topic = null;
			// we'll use a topic in the control state if we can find one
			// to find the occurrence on
			// but we wont update the topic itself
			if (context.ControlState.ContainsKey("topic")) {
					topic = context.ControlState["topic"] as Topic;
			}
			using (ITopicStore store = context.Services.GetObject<ITopicStore>("store::topic-map")) {
				store.Start();
				if (topic == null) {
					topic = store.GetTopic(ev["id"]);
				}
				Occurrence occurrence = topic.Occurrences.GetOccurrence(ev["scope"], ev["role"], ev["behaviour"], ev["reference"]);
				if (occurrence == Occurrence.Blank) {
					context.Errors.CreateMessage("Unable to find occurence to update string data for.");
				} else {
					Occurrence updatedOccurrence = occurrence.Mutate(o =>
					{
						o.StringData = ev["string-data"];
						return o;
					});
					store.AddOccurrence(updatedOccurrence);
				}
			}
		}
	}
}
