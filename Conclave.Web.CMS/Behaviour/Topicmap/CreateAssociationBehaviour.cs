using System;
using Conclave.Map.Model;
using Conclave.Map.Store;
using Conclave.Process;
using Conclave.Web.Behaviour;

namespace Conclave.Web.CMS.Behaviour.Topicmap {

	/// <summary>
	/// A behaviour concerned with creating new associations.
	/// </summary>
	/// <remarks>
	/// This is a second tier behaviour and consideres
	/// the current event, not the context.
	/// </remarks>
	public class CreateAssociationBehaviour : WebBehaviour {

		// like CreateTopicBehaviour, assoc creation
		// should probably be the result of prorotype copying
		// this implementation will be changed in that direction

		public CreateAssociationBehaviour(string message) : base(message) { }

		public override void Action(IEvent ev, WebContext context) {
			if (ev.HasRequiredParams("assoc")) {

				using (ITopicStore store = context.Services.GetObject<ITopicStore>("store::topic-map")) {
					store.Start();

					Association assoc = store.GetAssociation(ev["assoc"]);
					if (assoc != Association.Blank) {
						context.Errors.CreateMessage(String.Format("The association '{0}' already exists.", ev["assoc"])); // not good
						context.ControlState["new-assoc-id"] = Guid.NewGuid().ToString();
						context.ControlState["assoc"] = assoc;
					} else {
						if (ev.HasRequiredParams("assoc","parent", "scope", "type", "role", "reference")) {
							Association.Builder builder = new Association.Builder(ev["assoc"], ev["parent"], ev["type"], ev["scope"], ev["reference"], ev["role"]);
							builder.AddMetadata(ev["scope"], "label", ev["label"] ?? ev["reference"]);
							assoc = builder;
							store.AddAssociation(assoc);
							context.Messages.Add("Your association has been created."); // not good
						}
						context.ControlState["assoc"] = assoc;
					}
				}
			}
		}
	}
}
