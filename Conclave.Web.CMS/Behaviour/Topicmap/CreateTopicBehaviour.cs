using System;
using System.Text;
using Conclave.Map.Model;
using Conclave.Map.Store;
using Conclave.Process;
using Conclave.Web.Behaviour;

namespace Conclave.Web.CMS.Behaviour.Topicmap {
	/// <summary>
	/// A behaviour concerned with creating new topics.
	/// This is a second tier behaviour and consideres
	/// the current event, not the context.
	/// </summary>
	public class CreateTopicBehaviour : WebBehaviour {

		// this class needs to reduced down to just creating a topic
		// with other decoration being moved to other behaviours,
		// until then it serves as a place to incubate topic creation

		// copying a prototype topic for a new instance may be used
		// to add flexibility to topic creation

		public CreateTopicBehaviour(string message) : base(message) { }

		// remove non-alphanumeric characters
		// uppercase first char of words
		// wont work well with funky ids
		private string _labelFromId(string id) {
			StringBuilder builder = new StringBuilder();
			char c;
			for (int i = 0; i < id.Length; i++) {
				c = id[i];
				if (Char.IsLetterOrDigit(c)) {
					if (i == 0 || Char.IsWhiteSpace(id[i - 1])) { // if beginning of word
						builder.Append(Char.ToUpperInvariant(c));
					} else {
						builder.Append(c);
					}
				}
			}
			return builder.ToString();
		}

		public override void Action(IEvent ev, WebContext context) {
			if (ev.HasRequiredParams("id")) {
				string id = ev["id"];

				using (ITopicStore store = context.Services.GetObject<ITopicStore>("store::topic-map")) {
					store.Start();
					Topic topic = store.GetTopic(id);
					if (topic != Topic.Blank) {
						context.Errors.CreateMessage(String.Format("The topic '{0}' already exists.", id));
						context.ControlState["topic"] = topic;
					} else {
						// create the new topic
						Topic.Builder builder = new Topic.Builder(id);
						builder.AddMetadata("label", ev["label"] ?? _labelFromId(id));
						if (ev.HasParams("description")) builder.AddMetadata("description",ev["description"]);
						if (ev.HasParams("keywords")) builder.AddMetadata("keywords", ev["keywords"]);
						
						// default content for the topic
						// this should be built from an occurrence prototype hanging off a topic prototype
						// this is really not a good way to be doing this, but can live until prototyping
						builder.AddOccurrence("wiki", "markdown", ev["text"] ?? String.Format("New topic for {0}.", id));
						// add the topic to the store and wind up
						topic = builder;					
						context.ControlState["topic"] = topic;
						ev.Object = topic;
						store.AddTopic(topic);
						// let anybody else downstream know we've created a topic
						Event notification = new Event(context, "topicmap::topic-created", topic);
						notification.Fire();
					}
				}
			}
		}

	}
}
