using Conclave.Map.Model;
using Conclave.Map.Store;
using Conclave.Process;

namespace Conclave.Web.CMS.Behaviour.Topicmap {

	/// <summary>
	/// A behaviour resoponsible for updating Occurrences in a backing store.
	/// </summary>
	public class UpdateOccurrenceBehaviour: ProcessBehaviour {

		public UpdateOccurrenceBehaviour(string message) : base(message) { }
		
		public override void Action(IEvent ev, ProcessContext context) {
			// deliberately verbose to make clear
			// the heuristic here is perhaps not obvious
			// the key of an occurrence is parent, scope, role, behaviour, reference
			// also remembering that an occurrence can also hold binary data if it has a "self" reference
			// so, we're not actually going to alter an occurrences property
			// instead we're going to try and add what would be the new occurrence, if it doesn't already exist
			// it's for a separate opperation to remove the old occurrence if so desired, it's safer not to assume that
			// as its not much extra work, and sometimes useful
			if (ev.HasRequiredParams("parent", "scope", "role", "behaviour", "reference")) {

				using (ITopicStore store = context.Services.GetObject<ITopicStore>("store::topic-map")) {
					store.Start();
					if (ev["update"] == "remove") {
						store.RemoveOccurrence(ev["parent"], ev["scope"], ev["role"], ev["behaviour"], ev["reference"]);	
					} else { // we're trying to alter what we think is an existting occurrence
						Occurrence existingOccurrence = store.GetOccurrence(ev["parent"], ev["scope"], ev["role"], ev["behaviour"], ev["reference"]);
						// it doesn't matter that much if this occurrence actually exists
						// we're only interested in carrying forward the data if any exists
						// if the occurrence doesn't exist then we'll have Occurrence.Blank
						// and it's data is a fine default
						// we're copying forward to a slightly different version, not really updating

						// the occurrence properties will be those of the existing occurrence that have been passed in
						// but with one of them being updated to a provided value, depending on which one the
						// update parameter specifies
						string parent = ev["parent"];
						string scope = ev["scope"];
						string role = (ev["update"] == "role") ? ev["value"] : ev["role"];
						string behaviour = (ev["update"] == "behaviour") ? ev["value"] : ev["behaviour"];
						string reference = (ev["update"] == "reference") ? ev["value"] : ev["reference"];

						if (store.OccurrenceExists(parent, scope, role, behaviour, reference)) {
							context.Errors.CreateMessage("The altered version of this occurrence already exists. No occurrence has been updated.");
						} else {
							Occurrence newOccurrence = new Occurrence(parent, scope, role, behaviour, reference, existingOccurrence.Data);
							store.AddOccurrence(newOccurrence);
							context.Messages.Add("New occurrence added successfully.");
						}
					}
				}
			}
		}

	}
}
