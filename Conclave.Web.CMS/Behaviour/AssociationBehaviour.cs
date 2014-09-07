using System;
using Conclave.Process;
using Conclave.Web.Behaviour;

namespace Conclave.Web.CMS.Behaviour {
	/// <summary>
	/// A behaviour concerned with requests
	/// that manipulate a topic map associations.
	/// </summary>
	public class AssociationBehaviour : WebActionBehaviour {

		public AssociationBehaviour(string message) : base(message) { }

		public override void Action(IEvent ev, WebContext context) {
			if (context.HasParams("action")) {
				switch (context.Params["action"]) {
					case "edit":
						this.Edit(ev, context);
						break;
					case "inline-update-metadata":
						this.InlineUpdateMetadata(ev, context);
						break;
					case "inline-update":
						this.InlineUpdate(ev, context);
						break;
					case "create":
						this.Create(ev, context);
						break;
				}
			}
		}

		/// <summary>
		/// Edits an association. Either its core properties,
		/// or its metadata.
		/// </summary>
		/// <param name="ev">The <see cref="IEvent"/> being processed.</param>
		/// <param name="context">The <see cref="WebContext"/> for the currente request.</param>
		/// <remarks>
		/// Dispatches the messages: topicmap::update-metadata, topicmap::update-association
		/// topicmap::get-topic, topicmap::resolve-navigation.
		/// </remarks>
		protected void Edit(IEvent ev, WebContext context) {
			if (context.Request.IsPost && context.HasParams("update")) {
				if (context.HasParams("parent", "name", "scope")) {
					context.FireWith("topicmap::update-metadata", "parent", "name", "scope", "value");
				} else if (context.HasRequiredParams("assoc")) {
					context.FireWith("topicmap::update-association", "assoc", "update");
				}
			}
			context.FireWith("topicmap::get-topic","id");
			context.Fire("topicmap::resolve-navigation");
		}

		// this is the same implementation as MetadataBehaviour.InlineUpdate
		// and both need to be refactored into a common implementation
		protected void InlineUpdateMetadata(IEvent ev, WebContext context) {
			if (context.Request.IsPost && context.HasRequiredParams("pk", "value")) {
				// The 'pk' is the primary key and identifies which metadata item
				// is to be updated. It comes in the form 'parent::name::scope'
				// this isn't an especially safe scheme, and needs to be changed
				// to something that is. Topics with odd ids are going to collide
				// with this scheme.
				string pk = context.Params["pk"];
				string[] parts = pk.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length >= 3) {
					Event update = new Event(context, "topicmap::update-metadata", "value") 
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

		protected void InlineUpdate(IEvent ev, WebContext context) {
			if (context.Request.IsPost && context.HasRequiredParams("pk", "value")) {
				string pk = context.Params["pk"];
				string[] parts = pk.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length >= 2) {
					Event update = new Event(context, "topicmap::update-association", "value") 
					{
						{"assoc", parts[0]},
	                    {"update", parts[1]}                                              
					};
					update.Fire();
				} else {
					context.Errors.CreateMessage("The association to update was incorrectly specified.");
				}
			}
		}

		protected virtual void Create(IEvent ev, WebContext context) {
			if (context.Request.IsGet && context.HasParams("assoc")) {
				context.FireWith("topicmap::get-association", "assoc");
			} else if (context.Request.IsPost && context.HasRequiredParams("assoc","parent","scope","type","role","reference")) {
				context.FireWith("topicmap::create-association", "assoc", "parent", "scope", "type", "role", "reference");
			}
			context.ControlState["new-assoc-id"] = Guid.NewGuid().ToString();
			context.FireWith("topicmap::get-topic","id");
			context.Fire("topicmap::resolve-navigation");
			context.Fire("topicmap::resolve-occurrences");
		}

	}
}
