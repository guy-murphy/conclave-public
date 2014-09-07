using Conclave.Map.Model;
using Conclave.Map.Store;
using Conclave.Process;
using Conclave.Web.Behaviour;

namespace Conclave.Web.CMS.Behaviour.Topicmap {
	public class GetTopicBehaviour : ProcessBehaviour {
		public GetTopicBehaviour(string message) : base(message) { }

		public override void Action(IEvent ev, ProcessContext context) {
			if (ev.HasRequiredParams("id")) {
				using (ITopicStore store = context.Services.GetObject<ITopicStore>("store::topic-map")) {
					store.Start();
					Topic topic = store.GetTopic(ev["id"]);
					context.ControlState["topic"] = topic;
					ev.Object = topic;
					if (topic == Topic.Blank) {
						context.Errors.CreateMessage(string.Format("The topic '{0}' does not exist.", ev["id"]));
					}
				}
			}
		}

	}
}
