## `Conclave.Web.CMS`, Project Notes

To be clear, this is not a full functioning CMS, nor is it intended to be.
Consider it more of a scaffolding example. It contains useful code, and may
grow over time, but it's not pitching to be anybodies next CMS.

At the time of writing the CMS assembly is a library of 15 behaviours, and nothing
else.

One of the core goals of Conclave is
[Extensibility](http://en.wikipedia.org/wiki/Extensibility).

> In software engineering, extensibility (not to be confused with forward compatibility) is a system design principle where the implementation takes future growth into consideration. It is a systemic measure of the ability to extend a system and the level of effort required to implement the extension. Extensions can be through the addition of new functionality or through modification of existing functionality. The central theme is to provide for change – typically enhancements – while minimizing impact to existing system functions.

Listening, observing, reacting, extensibility has been implemented in 101 different
patterns, in a near infinite variety of application types. Conclave is not
the first framework to tackle extensibility. It remains a crucial box however,
that must demonstrate itself to have been ticked.

Messages and reactive behaviours make it very easy to extend a system, and also
feature switch. XML/XSL fits into that quite nicely also, with XML making it
very easy to extend data-model presentation (the clue is in the name). XSL
behaves very well in the absence of anticipated XML, and its stylesheets
are very easy to extend without impact.

### Three layers of application functionality

I found for myself what I felt to be a particular sweet-spot when I ported
Acumen to jAcumen <https://github.com/guy-murphy/jAcumen>, which was
implemented with Java, JRuby, Rails, and XSL. This fell into a balance I never
quite achieved in .NET, until Conclave.

In jAcumen, the `view` method in `topic_controller.rb`:-

	# We present the topic with its
	# resources resolved, in a form
	# suitable to read.
	def view
	  get_topic(@current_id)
	  get_related_for_current
	  fetch_blogs
	  fetch_comments
	end

Which in Conclave, `TopicBehaviour` is:-

	protected virtual void View(IEvent ev, WebContext context) {
		if (context.HasRequiredParams("id")) {
			context.FireWith("topicmap::get-topic", "id");
			context.Fire("topicmap::resolve-navigation");
			context.Fire("topicmap::resolve-occurrences");
		}
	}

Then we have the `get_topic` in jAcumen from `topics.rb`:-

	# We get the topic from the store
	# of the specified identity, filtered
	# by the *current* scope and language.
	#
	# This method takes the parameter:-
	# :resolved => [true|false]
	# which determines whether or not the
	# occurences should be resolved.
	#
	# :assoc => [associaction_id]
	# which designates that the association
	# of the specified ID should be selected
	# from the fetched topic and placed into
	# the view_state.
	def get_topic(id, opts={})
	  puts "#> get topic: #{id}"
	  @topic_store.start
	  # get the topic and resolve the occurences if appropriate
	  topic = @topic_store.get_topic_with(id, @current_language, @current_scope)
	  resolve_occurences!(topic, @current_map) unless opts[:resolve] == false
	  @view_state[:topic] = topic
	  # do we need to fetch an association?
	  if opts.has_key?(:assoc)
	    assoc = topic.get_associations.values.find do |a| 
	      a.get_id == opts[:assoc]
	    end
	    @view_state[:association] = assoc unless assoc.nil?
	  end
	  return topic
	ensure
	  @topic_store.stop
	end

While in Conclave, in the `GetTopicBehaviour`, we have:-

	public override void Action(IEvent ev, WebContext context) {
		if (ev.HasRequiredParams("id")) {
			if (!context.ControlState.ContainsKey("topic")) { // only get the topic if it hasn't already been got
				using (ITopicStore store = context.Services.GetObject<ITopicStore>("store::topic-map")) {
					store.Start();
					Topic topic = store.GetTopic(ev["id"]);
					context.ControlState["topic"] = topic;
					if (topic == Topic.Blank) {
						context.Errors.CreateMessage(string.Format("The topic '{0}' oes not exist.", ev["id"]));
					}
				}
			}
		}
	}

And finally, in jAcumen we had implementations of the topic store in Java
just as in .NET, with both serialising a ViewState or ControlState, and
rendering with XSL.

It's not that this method of organisation is especially virtuous over another
preferred approach, it's simply what I found to be a good balance that
refactors and extends well, while not becoming too dense. It also remains
for the most part imperative an not to strange by most Web developers
expectations, reaction being something you use by degree as needed.

Importantly in this scheme, behaviours at the front are reacting by mostly observing
the context, while the second layer of behaviours are mostly reacting by
observing the actual `IEvent` message and parameters. The first layer simply says
to the system what needs to be done, the second layer is the logic of how
that will be done, and the third layer (model/store) does the lifting and shields
from external resources.

*Take a look at the readme in the project fold for API documentation on
each behaviour for a break-down of what each behaviour does... 
you'll have to scroll down.*