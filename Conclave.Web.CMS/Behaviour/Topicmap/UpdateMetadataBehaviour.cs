using Conclave.Map.Model;
using Conclave.Map.Store;
using Conclave.Process;
using Conclave.Web.Behaviour;

namespace Conclave.Web.CMS.Behaviour.Topicmap {
	public class UpdateMetadataBehaviour: ProcessBehaviour {

		public UpdateMetadataBehaviour(string message) : base(message) { }
		
		public override void Action(IEvent ev, ProcessContext context) {
			if (ev.HasRequiredParams("parent", "scope", "name")) {
				using (ITopicStore store = context.Services.GetObject<ITopicStore>("store::topic-map")) {
					store.Start();
					if (ev.HasParams("value")) {
						Metadata metadata = new Metadata(ev["parent"], ev["scope"], ev["name"], ev["value"]);
						store.AddMetadata(metadata);
					} else { // there's no value so we're removing the metadata
						store.RemoveMetadata(ev["parent"], ev["scope"], ev["name"]);
					}
				}		
			}

		}
	}
}
