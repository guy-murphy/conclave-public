using System;
using Conclave.Process;
using Conclave.Web.Behaviour;

namespace Conclave.Web.CMS.Behaviour {

	/// <summary>
	/// A behaviour responsible for mediating requests
	/// concerned with topics.
	/// </summary>

	public class TopicBehaviour : WebActionBehaviour {

		public TopicBehaviour(string message) : base(message) { }

		public override void Action(IEvent ev, WebContext context) {
			if (context.HasParams("action")) {
				switch (context.Params["action"]) {
					case "view":
						this.View(ev, context);
						break;
					case "edit":
						this.Edit(ev, context);
						break;
					case "create":
						this.Create(ev, context);
						break;
					case "identity":

						break;
				}
			}
		}

		protected virtual void View(IEvent ev, WebContext context) {
			if (context.HasRequiredParams("id")) {
				context.FireWith("topicmap::get-topic", "id");
				context.Fire("topicmap::resolve-navigation");
				context.Fire("topicmap::resolve-occurrences");
			}
		}

		protected virtual void Edit(IEvent ev, WebContext context) {
			if (context.HasRequiredParams("id")) {
				if (context.Request.IsPost && context.HasRequiredParams("scope", "role", "behaviour", "reference", "string-data")) {
					context.FireWith("topicmap::update-occurrence-data", "id", "scope", "role", "behaviour", "reference", "string-data");
				}
				context.FireWith("topicmap::get-topic", "id");
				context.Fire("topicmap::resolve-navigation");
				context.Fire("topicmap::resolve-occurrences");
			}
		}

		protected virtual void Create(IEvent ev, WebContext context) {
			if (context.Request.IsGet) {
				context.ControlState["new-topic-id"] = context.HasParams("id") ? context.Params["id"] : Guid.NewGuid().ToString();
			} else if (context.Request.IsPost && context.HasRequiredParams("id")) {
				context.FireWith("topicmap::create-topic", "id");
			}
			context.FireWith("topicmap::get-topic", "id");
			context.Fire("topicmap::resolve-navigation");
			context.Fire("topicmap::resolve-occurrences");	
		}

		protected void Remove(IEvent ev, WebContext context) {
			context.FireWith("topicmap::remove-topic", "id");
		}

	}
}
