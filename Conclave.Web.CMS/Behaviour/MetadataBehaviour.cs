using System;
using Conclave.Process;
using Conclave.Web.Behaviour;

namespace Conclave.Web.CMS.Behaviour {

	/// <summary>
	/// A behaviour responsible for mediating requests
	/// concerned with topic metadata.
	/// </summary>

	public class MetadataBehaviour: WebActionBehaviour {

		public MetadataBehaviour(string message) : base(message) { }

		public override void Action(IEvent ev, WebContext context) {
			if (context.HasParams("action")) {
				switch (context.Params["action"]) {
					case "inline-update":
						this.InlineUpdate(ev, context);
						break;
					case "edit":
						this.Edit(ev, context);
						break;
				}
			}
		}

		private void InlineUpdate(IEvent ev, WebContext context) {
			if ( context.Request.IsPost && context.HasRequiredParams("pk", "value")) {
				// The 'pk' is the primary key and identifies which metadata item
				// is to be updated. It comes in the form 'parent::name::scope'
				// this isn't an especially safe scheme, and needs to be changed
				// to something that is. Topics with odd ids are going to collide
				// with this scheme.
				string pk = context.Params["pk"];
				string[] parts = pk.Split(new string[] {"::"}, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length >= 3) {
					Event update = new Event(context, "topicmap::update-metadata","value") 
					{
						{"parent", parts[0]},
						{"name", parts[1]},
						{"scope", parts[2]}
					};
					update.Fire();
				} else {
					context.Errors.CreateMessage("The metadata to update was incorrectly specified.");
				}
			}
		}

		private void Edit(IEvent ev, WebContext context) {
			if (context.Request.IsPost) {
				context.FireWith("topicmap::update-metadata","parent", "name", "scope","value");
			}
			context.FireWith("topicmap::get-topic", "id");
			context.Fire("topicmap::resolve-navigation");
		}

	}
}
