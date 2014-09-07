using Conclave.Map.Model;
using Conclave.Map.Store;
using Conclave.Process;
using Conclave.Web.Behaviour;

namespace Conclave.Web.CMS.Behaviour.Topicmap {
	public class GetAssociationBehaviour : ProcessBehaviour {
		public GetAssociationBehaviour(string message) : base(message) {}

		public override void Action(IEvent ev, ProcessContext context) {
			if (!context.HasParams("assoc")) {
				context.Errors.Add(new ErrorMessage("There was no @id to get an association for."));
			} else {
				if (!context.ControlState.ContainsKey("assoc")) {
					// only get the assoc if it hasn't already been got
					using (ITopicStore store = context.Services.GetObject<ITopicStore>("store::topic-map")) {
						store.Start();
						Association assoc = store.GetAssociation(context.Params["assoc"]);
						context.ControlState["assoc"] = assoc;
						ev.Object = assoc;
						if (assoc == Association.Blank) {
							context.Errors.CreateMessage(string.Format("The association '{0}' does not exist.", context.Params["assoc"]));
						}
					}
				}
			}
		}
	}
}
