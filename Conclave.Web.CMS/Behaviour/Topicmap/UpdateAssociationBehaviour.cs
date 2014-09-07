using Conclave.Map.Model;
using Conclave.Map.Store;
using Conclave.Process;
using Conclave.Web.Behaviour;

namespace Conclave.Web.CMS.Behaviour.Topicmap {
	public class UpdateAssociationBehaviour : WebActionBehaviour {

		public UpdateAssociationBehaviour(string message) : base(message) { }

		public override void Action(IEvent ev, WebContext context) {
			if (context.Request.IsPost && ev.HasRequiredParams("assoc", "update")) {
				using (ITopicStore store = context.Services.GetObject<ITopicStore>("store::topic-map")) {
					store.Start();

					if (ev.HasParams("value")) {
						Association assoc = store.GetAssociation(ev["assoc"]);
						if (assoc != Association.Blank) {
							switch (ev["update"]) {
								case "type":
									assoc = assoc.Mutate(a =>
									{
										a.Type = ev["value"];
										return a;
									});
									break;
								case "role":
									assoc = assoc.Mutate(a =>
									{
										a.Role = ev["value"];
										return a;
									});
									break;
								case "scope":
									// not entirely sure of the implications
									// of this yet esepcially in relation to assoc metadata
									//assoc = assoc.Mutate(a =>
									//{
									//	a.Scope = value;
									//	return a;
									//});
									break;
								case "remove":
									store.RemoveAssociation(ev["assoc"]);
									break;
								default:
									context.Errors.CreateMessage("The update requested was not recognised.");
									break;
							}
							if (ev["update"] != "remove") {
								store.AddAssociation(assoc);
							}
						}
					}
				}
			}
		}

	}
}
