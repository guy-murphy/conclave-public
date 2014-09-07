using Conclave.Collections;
using Conclave.Process;
using Conclave.Map.Model;

namespace Conclave.Web.CMS.Behaviour.Topicmap {
	public class ResolveNavigationBehaviour: ProcessBehaviour {

		public ResolveNavigationBehaviour(string message) : base(message) {}

		public override void Action(Process.IEvent ev, ProcessContext context) {
			if (!context.ControlState.ContainsKey("topic")) {
				context.Errors.Add(new ErrorMessage("There was no topic to resolve a navigation view for."));
			} else {
				if (!context.ControlState.ContainsKey("navigation-view")) { // no point doing this if we've already done it
					// this is a dictionary of dictionaries of lists... move to its own type when stable
					// consider becoming the default "presentation" of a topic, 
					// and investigate linq GroupBy to see if we can skip this construct entirely
					DataDictionary<DataDictionary<DataCollection<Association>>> navigation = new DataDictionary<DataDictionary<DataCollection<Association>>>();
					// iterate over the associations in the primary topic
					Topic topic = context.ControlState["topic"] as Topic ?? Topic.Blank;
					foreach (Association assoc in topic.Associations) {
						// populate the sub dictionaries as needed
						if (!navigation.ContainsKey(assoc.Type)) { 
							navigation[assoc.Type] = new DataDictionary<DataCollection<Association>>();
						}
						// populate the lists as needed
						if (!navigation[assoc.Type].ContainsKey(assoc.Role)) {
							navigation[assoc.Type][assoc.Role] = new DataCollection<Association>();	
						}
						// add the actual association
						navigation[assoc.Type][assoc.Role].Add(assoc);
					}
					context.ControlState["navigation-view"] = navigation;
				}
			}
		}
	}
}
