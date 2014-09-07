using System;
using Conclave.Process;
using Conclave.Web.Behaviour;

namespace Conclave.Web.CMS.Behaviour {
	public class OccurrenceBehaviour: WebActionBehaviour {

		public OccurrenceBehaviour(string message) : base(message) { }
		
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

		protected void Edit(IEvent ev, WebContext context) {
			if (context.Request.IsPost) {
				context.FireWith("topicmap::update-occurrence", "parent", "scope", "role", "behaviour", "reference", "update");
			}
			context.FireWith("topicmap::get-topic", "id");
			context.Fire("topicmap::resolve-navigation");
		}

		protected void InlineUpdate(IEvent ev, WebContext context) {
			if (context.Request.IsPost && context.HasRequiredParams("pk", "value")) {
				string pk = context.Params["pk"];
				string[] parts = pk.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length >= 6) {
					Event update = new Event(context, "topicmap::update-occurrence", "update", "value") 
					{
						{"parent", parts[0]},
						{"scope", parts[1]},
						{"role", parts[2]},
						{"behaviour", parts[3]},
						{"reference", parts[4]},
						{"update", parts[5]}
					};
					update.Fire();
				} else {
					context.Errors.CreateMessage("The occurrence to update was incorrectly specified.");
				}
			}
		}

	}
}
