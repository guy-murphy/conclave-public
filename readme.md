# Conclave (alpha)
#### Pragmatic Web application development, on the .NET platform for 80% of us.

*Conclave is prototype/alpha quality code being shared among a close circle of
friends familiar with the work, playing about with some ideas. Conclave is what 
I'm bringing to that particular party, and if it is of interest to anybody else 
that would be really cool, but at this stage that's not a primary concern.*

Conclave is a Behaviour Oriented Web application micro framework occupying
a similar space as MVC.NET but with an emphasis on simplicity and extensibility,
and no emphasis as a framework on patterns such as MVC, while ensuring
developers can implement an MVC application (or otherwise) as they choose.

Conclave is also a simplified Topicmap model and backing store. While
significantly divergent from any standard it has grown out of over a decade
of similar implementations, and seeks to be a practical and pragmatic
topic solution for commercial Web applications that are smaller in scale and
ambition than national libraries (Conclave has been tested with 780k topics). 
In this incarnation I'm playing with an
immutable model and a Builder Pattern as in future implementations I want to
move toward an append only, versioned, immutable backing storage, making this
a necessary step along the way.

The CMS portion of Conclave is intended as both a playpen for playing with ideas
and comparisons of approaches. Along the way it's hoped it turns into a half
decent content management system, but it will always be a developer focused
application rather than for end users, as its primary purpose is serving as
an example and a scaffolding from which developers can build applications.

Conclave CMS is moderately high performance, offering up to 7.6k requests per
second from a backing store of 780,000 topics. Any optimisation that is likely
to be platform or application specific has been avoided. So there is no data
caching, and authentication has a simple but general implementation. Whether
or not an application is sat on one server or multiple and load balanced
makes such a huge difference to caching strategy, along with different model
characteristics that there is a danger of getting in the way of the developer.
The goal then is to obtain a sufficient baseline of performance, and sufficient
hooks. Alternative implementations of caching both in memory and out of
process will eventually be provided, but it's not a high priority at this stage.
The focus here is on the 80% of common utility and to avoid the 20% of
divergent need. I can't know what you need, all I can try and do is ensure that
the framework doesn't get in your way when your need diverges from mine.

The documentation for Conclave is initially directed at a group of friends and
former colleagues who are familiar with this work. Half of them worked on a
content management system and in-house .NET MVC framework and topicmap
engine with me. The other half worked with me developing a behavioural framework that
served as both a data-quality inference engine and application framework. This
documentation is directed at them (my diaspora of a team) first and foremost.
If it's of use to anybody else at this stage, that's a bonus. The documentation
also rambles a little as I'm laying down notes that properly need pulling
out and expanding on elsewhere... that will happen over the Winter. Indulge
the rambling for now, these notes are as much for me as anybody else.

There's lots of notes in the source code comments, particularly high-lighting
potentially contentious areas. Conclave has a simple procedure for extracting
API XML docs, solution and project notes, and constructing `readme.md`
documents suitable primarily for GitHub. More work with be done on this
in the future to make the documentation more generally suitable, and a better
body of hypertext. If you wish to change the documentation to your own
purposes, take a look at `Conclave.Documentation.Generator`. You might want
to scroll down any given document before dismissing it, as there's often
several types of content on the same page.

### A Shrinking Codebase
The versions of application framework and content delivery software that
have preceded Conclave have normally been expansive processes as the scope
of concern has widened. Conclave, even as it encompasses the beginnings
of a behavioural processing model, is a process of reducing and pairing down.
It's a long overdue consideration of "You Aint Gonna Need It" (YAGNI). As a
result of this there are some types that look a bit anaemic as they're in
the process of being bled.

This is a much smaller codebase than Acumen, while providing more utility,
and significantly better performance... not least because it is a smaller
codebase. 

Lastly keep in mind there's still refactoring taking place. Sometimes you will
see functionality being incubated in one place, with no long-term intention
of it remaining there. I'm still very much in the middle of testing and routing
out assumptions expressed through interfaces, this is most evident on
`ITopicStore` at the moment which leaks a surprising amount of assumptions
about being backed by a relational store.

## A Starting Point
It's just an `IHttpHander`.

If you're having a poke around the codebase or even thinking of trying it out
and need somewhere to start, `Conclave.Web.DefaultHandler` is probably the best
place. That coupled with an understanding of `WebContext` and `WebBehaviour`
along with some comfort configuring Srping.NET is all that's really needed.

`Conclave.CMS` is the web application directory. You'll find all the site
resources naturally enough in the `Conclave.CMS/Resources` directory.
Pay attention not only to `Web.config`, but also `config.xml` and
`behaviours.xml`

`Conclave.Web.CMS`, is the assembly with the backing behaviours for
the CMS.

Anybody coming from an MVC.NET background or similar, should probably start
by emulating that pattern using Conclave. You have a topicmap model, and some
behaviours as controllers, with either XSL, Razor or some other templating
for views. If you've another pattern close to your heart by all means
run with it, there's no reason not to implement CQRS or whatever takes
your fancy. I would particularly appreciate any feedback on pain points
experienced implementing different patterns.

## License
I'll worry about a license over the Winter if there seems to be any legs to
this as a project. Until then the only thing I will stress is redistribution
of this code is expressly forbidden without prior permission. It's one thing
if some individual devs want to play about with this code, but I'll not have
it foisted on unsuspecting clients.

Technically, at this point, all rights are reserved... make of that what you
will... if you're going to use the codebase commercially, don't take the piss.

If anybody takes a fancy to the idea of porting Conclave sideways to a different
platform, even if only in part... please feel free, you have my full
support. If somebody doesn't get to it first I will implement Conclave in
JS for Node eventually, as I'm really curious as to how it would fit, but
it's in the middle of quite a long list of things I need to get around
to doing. Clojure and Elixir are also on the list.



## Project Docs
* [Conclave](#Conclave)
* [Conclave.CMS](#Conclave.CMS)
* [Conclave.Map](#Conclave.Map)
* [Conclave.Mongo](#Conclave.Mongo)
* [Conclave.MySql](#Conclave.MySql)
* [Conclave.Process](#Conclave.Process)
* [Conclave.SqlServer](#Conclave.SqlServer)
* [Conclave.Web](#Conclave.Web)
* [Conclave.Web.CMS](#Conclave.Web.CMS)


<a name="Conclave"></a>
## `Conclave`, Project Notes

The base classes for `IData` concerns. This used to be
a much larger assembly, but has been paired down until
it's feeling a bit thin.

`Conclave.Collection.DataModel` is about the only
non-obvious thing I'd draw attention to. I need to
test dropping this in as the `ProcessContext.ControlState`
as the `DataModel` implementation is easier to use
from Razor.

I've a bunch of stuff to move into `StringBuilderEx`
from variious scraps around the place, bringing
`StringBuilder` up to par with `string` in terms
of utility methods.

### `IData`, and XML/JSON Web applications

At the moment Conclave has behaviours supporting both Razor and XSLT. It's
trivial to plug in new templating schemes, and I'll drop in a 
`StringTemplateViewBehaviour` as I want to test the performance of ST4,
<http://www.stringtemplate.org/>. Conclave however favours being 
*an XML (or JSON) application*.

The root of the idea is that Conclave is an application that exposes a model
as a representation of the result of a request, and that model is serialised
to XML. This is then transformed with XSL into a view suitable for the user
agent if needed. `IData` forms a simple but foundational role as it denotes
a model that may *potentially* be exposed at the "surface" of the application.
`IData` insists `.ToXml(...)` implementations, with the `ControlState`
itself being an `IData` implementation containing `IData` element values. So
the end of processing a request is `context.ControlState.ToXml()`

The goals for this are considered in `Conclave.Web`, but it's important to
stress here, your model implements `IData`, which also mandates `.ToJson(...)`,
and you put your `IData` object in the `ControlState` as side effect of
behaviours... See `Conclave.Process` for detail. 

* [Conclave project folder](./Conclave/)

<a name="Conclave.CMS"></a>
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

* [Conclave.CMS project folder](./Conclave.CMS/)

<a name="Conclave.Map"></a>
## `Conclave.Map`, Project Notes

*"It's all just circles and arrows"*, is really still the best explanation
I've come up with.

This work is based upon over a decade of tinkering with Topicmap-**like**
datamodels, and content delivery applications built on top of them.

The [Associative Model of Data](http://en.wikipedia.org/wiki/Associative_model_of_data)
is probably a better place to start for people coming to Topicmaps afresh, with
an excellent early white-paper on the subject <http://www.sentences.com/docs/other_docs/AMD.pdf>
being a very worth-while read for any Web developer... The Web is a graph,
and graph models tend to therefore be quite useful for Web sites and applications,
if nothing else.

From there, Google "topicmap"... The wikipedia page is a bit shit, and a lot
of the material is quite historic at this point, but plenty of people have
put good effort into explaining topicmaps, and it explains the
underpinning of `Conclave.Map`

`Conclave.Map` is heavily divergent from from the ISO standard, largely
because I think the standard is overly rarefied and directed to cater to the
needs of quite academic interests. While the standard may rightly ensure
that index merger for national archives is properly addressed, I just want to
build Web sites.

I disagree with some of the design decisions that have been made in the standard,
especially around relationships that I feel should just be expressed as
regular associations. I also feel no attention has been made to ensuring the
data-model works well on relational storage.

### `Topic`, nodes... the circles.
`(id)`

*Imaging drawing circles on a whiteboard representing high level entities,
and labelling them. They're your topics.*

A topic is essentially a point of identity. It's a conceptual node. On relational
storage the topic table is the most boring table ever, as it's just `id`.

A topic also represents a composition of view and resource for that topic, so
for many concerns of Web applications you can reasonably start by thinking
of a topic as a page or at least a unit of composed content. This is quite a
shallow view however and merely a starting point.

### `Association`, arcs... the arrows.
`(id, parent, type, scope, reference, role)`

*Now imagine drawing arrows between the circles, and labelling them, where the
labels reflect the role the circle being pointed to plays in relation
to the circle being pointed from. These are your associations.*... The rest is
detail.

In the standard a topicmap is a hypergraph, meaning a single associations can
cite many topics participating in the association each with a role. This got
dropped early on because it made the association model much denser, harder
to query, and confused the hell out of a lot of people. They were reason alone
but there are situations were you might be okay with 100 topics pointing at
topic-x, but you don't necessarily want to be pointing from topic-x to
100 topics. Each locator in the topicmap standard would be its own association
in `Conclave.Map`, meaning you can still model everything you would have, but
you can cut down on a lot of arcs when pragmatically you don't need them and
they're just slowing down your queries. 

It also means a topic is a self contained view on any hierarchical backing store 
like a document store, which in turn means a whole topic can be fetched by id
and no other querying needs to take place. 

As in `Acumen.Map`, associations remain individually addressable, and belong 
to their parent topic.

We didn't maintain associations well in Acumen, and so we couldn't rely upon
then for a navigation view for a topic and instead came to rely upon a
`GetRelatedTopics(...)` query to form a navigation view, just for labels
basically. This was a gross error, and we never corrected it. Conclave avoids
that mistake and so getting related topics is not a common case. This alone
accounts for a huge performance increase over Acumen for simple pages.

### `Metadata`, name/value pairs
`(parent, scope, name, value)`

*Now we ascribe properties to the circles and the arrows.*

Both topics and associations can have metadata, that is application specific.
Items such as *'created-on'* and *'created-by'*, and most commonly labels. In
Acumen metadata had both `scope` and `lang` as Acumen required multilingual
content as a core requirement. With only `scope` you can yield the same effect
as we did in Acumen, especially with a little name mangling... eg. *'es::sales'*
and *'de::sales'*.

Conclave (like Acumen before it) does not insist on maintaining different
pages of content but instead will compose a different view of a topic
based upon scope.

Conclave doesn't have the fallback to a default of *'Any'* where the language
isn't present, and if there were the kind of multilingual focus that Acumen
had, you'd probably want that back... Without that need, such fallback to
default would make scope leaky, which is never desirable, especially if you
want to apply security to scope, which quite a few people will want to do at
some point.

It fits a broader use-case like this, and can be changed if needed.

### `Occurrence`, content references
`(parent, scope, role, behaviour, reference, data)`

*Lastly, we point from topics to external resources and name the behaviours
that will resolve them.*

Occurrences belong only to topics and represent occurrences of that topic, and
like metadata are scoped. This is where the most significant if subtle
changes from `Acumen.Map` have been made.

Cited by an occurrence is a `reference` which is to be interpreted by a component
known to the system as a named `behaviour`. That behaviour is responsible for
resolving the reference, and placing the content inline with the occurrence.
This content might be the actual data, such as marked up content, or it might
be further references to be presented to a user-agent to resolve, such
and links to images for a gallery.

This allows new types of content and data to be integrated easily. The
`reference` might be a url, and id, a composite key, a query... any string
that a behaviour can resolve against an external resource. 

We were fearless about adding new content types to Acumen, it was trivial
and welcomed as a relaxing distraction.

This is an extension to the standard topicmap model, and is the first forming
of the idea of modelling behaviour as a point of extensibility. It was the first
really understanding of the power of a generalised interface and the freedom
that was afforded when you let parts of the system just get on and do their
thing. Much later while working on an inference engine that I noted the 
similarities between the rules as they were modelled and the occurrence 
behaviours in Acumen, and idly wishing I could use behaviours everywhere, 
and then asked myself...*"Why can't I used behaviours everywhere?"*... and
having no good answer, tried it. Turns out I can use behaviours everywhere. In
Conclave the behaviours cited by occurrences are regular `IProcessBehaviour`.
Resolving these references becomes a regular call into the system.

	foreach (Occurrence occur in topic.Occurrences) {
		Event resolve = new Event(context, "topicmap::resolve-occurrence", occur) 
		{
			{ "behaviour", occur.Behaviour }
		};
		resolve.Fire();
	}

In theory you could yield side-effect from this, but given the current model
this would be surprising, so probably not a good idea. I have considered
changing the occurrence model to a two-way external interface model, but that's the
subject of future incarnations of Conclave. 

The biggest change is adding a data field, and the convention that 
`reference="self"` denotes that the resource is the data field rather
than an external resource. It is still for a cited behaviour to resolve
this data, but a large proportion of applications have no need to reference
external resources for a lot of the content, and in those cases the difference
in performance is significant. So, wiki-text can have reside in the data
field and come with the regular topic query.

This would not have been appropriate for Acumen where we had a `FileStore`
that was versioned and append-only. The revisions of content were especially
important for a written-copy heavy interest. I've begun reimplementing
the filestore in Conclave, but I've halted it as I'm not sure I want to redo
it as was.

The data field is binary, with the model offering a decoded string version
as `.StringData`, but it would be possible to serialise objects into this field.
I'm not sure that would be a good idea, but you could.

### `Conclave.Map.Model`, immutable model and `Builder` pattern

The biggest change with `Conclave.Map.Model` over Acumen is the ditching of
model interfaces (no more `ITopic`) in favour of concrete immutable types
with an associated `Builder` as a nested class. In seven years we've never 
wanted an alternative implementation of `ITopic`.

The idea is that like `String` and `StringBuilder`, you make the mutable and
immutable version of your types distinct, and you get out of the habit of
scattering mutable state everywhere and then agonising over locks. I'd watched
some material on immutability in Clojure and how sharing partial backing
among data-structures was implemented, so when Microsoft release the immutable
collections library with the builder pattern and using some of the same techniques
as Clojure it was the final prod.

Each element of the model has both a concrete immutable type and a builder,
so there are both classes `Topic` and `Topic.Builder`. The builder is used
exactly as you would a `StringBuilder`. The best place to look in the codebase 
for solid examples of using builders that makes sense and doesn't sound contrived
is in the implementations of `ITopicStore` such as `MySqlTopicStore` or 
`MongoTopicStore` where you can see the object graph being built up from builders
as the query results are read, and then cast to the immutable when done.

Each builder implements implicit cast operators to and from both types. The
idea of this is to form the same kind of barrier as a *clone in, clone out* policy
with severed references. This is why I dropped the interfaces, when you have
`.SomeMethod(Topic topic)` you want to ensure the immutable topic is what you
will get.

There's a couple of reasons for trying this out... a whole lot of thread-safety
and concurrency concerns go out the window with immutable models. Mutating state
stops being the norm and become more explicit.

	Occurrence o2 = o1.Mutate(m =>
	{
		m.StringData = _markdown;
		return m;
	});

With the `Mutate` method implemented:-

	public Occurrence Mutate(Func<Builder, Occurrence> mutator) {
		Builder builder = new Builder(this);
		return mutator(builder);
	}

There's things you can do so the immutability doesn't feel like a chain
around your ankle.

Now the constructor of your concrete model element can act like a constructor
and ensure valid state. Because it's immutable if the state isn't correct at
instantiation it never will be. User input validation is an obvious concern, but
it's surprising how much state validation code you scatter about the place
when constantly changing state.

Also similar to `String.Empty` there is a `Topic.Blank`, `Association.Blank` and
so on, as part of my mission to see if I can eliminate `null` references
except were algorithmically significant. Public methods for `ITopicStore`
for instance would return `Topic.Blank` if one wasn't found, never null.

Profiles are not showing this as a performance concern. What I can't
be sure about, but strongly suspect is that the huge performance gains of
Conclave over Acumen are in large part because of much stricter state
and the amount of code that has been eliminated as a result. The builder
pattern is part of that. Suspend disbelief for a wee while and consider it,
you don't find `StringBuilder` odd, and as you have string interning, topic
interning might be the sort of thing can be chased from this.

Now part of the choice to go this way is because I also want to look at
a versioned append only backing for a topic store, and as identity and versioning
is beefed up in the model there's short-cuts can be taken in data processing.

As you can see, its still very fuzzy in my head... In truth the Mr. Hickey
Clojure chap convinced me in a Youtube video that I need more experience in this
direction. I'm enjoying it, things feel clearer, I'm realising how wildly I used
to scatter mutable state all over the place. It's nice not to. It also fits
the Acumen/Conclave ethos of a more ordered state.

### `ITopicStore`

Check out the Mongo implementation... I need to check this on Couch too as
if I recall aright, CouchDB has a viable mobile offering.

Not all the functionality has been brought over from Acumen yet. `GetRelatedTopics`
and `GetPointingTopics` aren't in yet, and I'll not put them in until I'm
actually going to use them (soon, I need them for blogging). 
They're a straight sideways port based on old implementations, 
so there's no concern there.

The focus has been on covering the broadest possible range of use-cases
from the single `GetTopic`, with the greater emphasis on association metadata,
and the extension of occurrences for self referenced data. This means that
a straight up vanilla Website, can serve its content from a single topic query,
in many cases with no need to resolve occurrence data external to the map
by virtue of self data. This is were most of the performance has been gained,
do as much as possible in one query, and do less stuff in general.

`GetRelatedTopics` isn't a core query anymore, but `GetPointingTopics` is still
likely to be useful for any appending content such as blogs, comments, appended
notes for example. It's a well understood query on relational stores, and I've
tested it on Mongo, with the appropriate index, and its a really simple query,
easy to process resulting data, and cheap. The .NET driver for Mongo is really
nice.

`ITopicStore` is likely to have it's focus tightened more to the core basic
functionality, and I'm thinking of introducing `IExtendedTopicStore` for
the broader queries and anything that is likely to have very variable
performance depending on type and size of backing data. This'll shuffle along
with general refactoring over Winter.

Scope is woven through, but hasn't had the tires kicked on it at all as I don't
have a real use for it at the moment. It's a much simpler affair than in Acumen
so the kind of filtering and leaking bugs we had to iron out of Acumen shouldn't
be present in Conclave. I just expect to have failed to wire up a couple of bits.

### Overall model

    <topic id="Top/Business/Arts_and_Entertainment/Sports/Facilities">
      <metadata>
        <metadata 
			for="Top/Business/Arts_and_Entertainment/Sports/Facilities" 
			scope="default" 
			name="label" 
			value="Facilities" 
		/>
        <metadata 
			for="Top/Business/Arts_and_Entertainment/Sports/Facilities" 
			scope="default" 
			name="test" 
			value="test" 
		/>
        <metadata 
			for="Top/Business/Arts_and_Entertainment/Sports/Facilities" 
			scope="default" 
			name="description" 
			value="This category contains links to businesses associations." 
		/>
        <metadata 
			for="Top/Business/Arts_and_Entertainment/Sports/Facilities" 
			scope="default" 
			name="last-update" 
			value="2007-01-02 17:51:53" 
		/>
      </metadata>
      <associations>
        <association 
			parent="Top/Business/Arts_and_Entertainment/Sports/Facilities" type="navigation" 
			scope="default" 
			reference="Top/Science/Social_Sciences/Economics/Sports_Economics/Stadiums_and_Arenas" 
			role="related" 
			id="dcd05a88-e7b9-46d7-bd57-986b5f71b073"
		>
          <metadata>
            <metadata for="dcd05a88-e7b9-46d7-bd57-986b5f71b073" scope="default" name="label" value="Stadiums and Arenas" />
          </metadata>
        </association>
        <association 
			parent="Top/Business/Arts_and_Entertainment/Sports/Facilities" 
			type="navigation" 
			scope="default" 
			reference="Top/Shopping/Sports/Tennis/Courts_and_Surfaces" 
			role="related" 
			id="0b4b38ef-3272-43d5-a2c2-394e049d09c4"
		>
          <metadata>
            <metadata 
				for="0b4b38ef-3272-43d5-a2c2-394e049d09c4" 
				scope="default" 
				name="label" 
				value="Courts and Surfaces" 
			/>
          </metadata>
        </association>
        <association 
			parent="Top/Business/Arts_and_Entertainment/Sports/Facilities" 
			type="similar" 
			scope="default" 
			reference="Top/Business/Construction_and_Maintenance/Building_Types/Sports" 
			role="Construction and Maintenance" 
			id="5c1d5b46-bd8e-44ec-8270-c743a763f90e"
		>
          <metadata>
            <metadata 
				for="5c1d5b46-bd8e-44ec-8270-c743a763f90e" 
				scope="default" 
				name="label" 
				value="Sports" 
			/>
          </metadata>
        </association>
        <association 
			parent="Top/Business/Arts_and_Entertainment/Sports/Facilities" 
			type="navigation" 
			scope="default" 
			reference="Top/Sports/Golf/Course_Architecture" 
			role="related" id="67af56e5-8ac9-49d1-8ea6-77866706af72"
		>
          <metadata>
            <metadata for="67af56e5-8ac9-49d1-8ea6-77866706af72" scope="default" name="label" value="Course Architecture" />
          </metadata>
        </association>
        <association parent="Top/Business/Arts_and_Entertainment/Sports/Facilities" type="test" scope="default" reference="test" role="test" id="69a28526-af24-41d6-b282-ec06d3d6b066">
          <metadata>
            <metadata 
				for="69a28526-af24-41d6-b282-ec06d3d6b066" 
				scope="default" 
				name="label" 
				value="Test Label" 
			/>
          </metadata>
        </association>
      </associations>
      <occurrences>
        <occurrence 
			for="Top/Business/Arts_and_Entertainment/Sports/Facilities" scope="default" role="wiki" 
			behaviour="markdown" 
			reference="self"
		>
         	<string-data encoding="Unicode (UTF-8)">
				<![CDATA[
					This is a topic, there are many like it, but this topic is mine.
				]]>
			</string-data>
          	<resolved-model>
            	<text-data>
					<![CDATA[
						<p>
							This is a topic, there are many like it, but this topic is mine.
						</p>
					]]>
				</text-data>
          </resolved-model>
        </occurrence>
      </occurrences>
    </topic>



* [Conclave.Map project folder](./Conclave.Map/)

<a name="Conclave.Mongo"></a>
## `Conclave.Mongo`, Project Notes

Tests were run on an **i7-3770K, with a surplus of RAM** for any test run. The
database is on the same box as the Web server. The page delivered is a full
end to end with templated render, occurrence wiki resolution,
and authentication; although the
authentication has remained against MariaDB as it's the topic store I want
to gauge. It's the smallest realistic page delivery from `Conclave.CMS` but
it represents a valid performance ceiling.

The Mongo implementation of `ITopicStore` is the first to survive the
*"test to failure"* test that I've run over the years, not only without
failing (breaking a performance or resource requirement), but while offering
the same levels of performance as on smaller datasets. This is personally
quite an arresting point to have reached. *At 2048 concurrent requests
apache bench measures >7k req/sec*.

With some changes to the service container (large proportion of time spent is here),
and authentication I think >8k req/sec for this test is likely.

Currently the mongo implementation of `ITopicStore` is by far the fastest
implementation. With 780k topics loaded...

#### -c 8

    Server Software:        Microsoft-IIS/8.0
    Server Hostname:        localhost
    Server Port:            80
    
    Document Path:          //conclave.cms/public/topic/view.aspx?id=Top/
							Business/Arts_and_Entertainment/Sports/Facilities
    Document Length:        4998 bytes
    
    Concurrency Level:      8
    Time taken for tests:   2.758 seconds
    Complete requests:      10000
    Failed requests:        48
       (Connect: 0, Receive: 0, Length: 48, Exceptions: 0)
    Write errors:           0
    Keep-Alive requests:    10000
    Total transferred:      55750048 bytes
    HTML transferred:       49980048 bytes
    Requests per second:    3625.35 [#/sec] (mean)
    Time per request:       2.207 [ms] (mean)
    Time per request:       0.276 [ms] (mean, across all concurrent requests)
    Transfer rate:          19737.65 [Kbytes/sec] received
    
    Connection Times (ms)
                  min  mean[+/-sd] median   max
    Connect:        0    0   0.0      0       1
    Processing:     1    2   1.9      2      21
    Waiting:        1    2   1.9      2      21
    Total:          1    2   1.9      2      21
    
    Percentage of the requests served within a certain time (ms)
      50%      2
      66%      2
      75%      2
      80%      2
      90%      2
      95%      3
      98%     10
      99%     16
     100%     21 (longest request)

"Length", failed requests are as a result of the content length of the response
being different on 48 pages as a result of a timing counter at the bottom of
the page giving a different time for 48 pages. This has been confirmed.

With higher concurrency levels we still get good results:-
#### -c 256
    Concurrency Level:      256
    ...
    Requests per second:    3785.96 [#/sec] (mean)
    Time per request:       67.618 [ms] (mean)
    Time per request:       0.264 [ms] (mean, across all concurrent requests)

#### -c 512
    Concurrency Level:      512
    ...
    Requests per second:    3663.88 [#/sec] (mean)
    Time per request:       139.743 [ms] (mean)
    Time per request:       0.273 [ms] (mean, across all concurrent requests)

#### -c 1024
    Concurrency Level:      1024
    ...
    Requests per second:    4942.52 [#/sec] (mean)
    Time per request:       207.182 [ms] (mean)
    Time per request:       0.202 [ms] (mean, across all concurrent requests)

#### -c 2048
    Concurrency Level:      2048
    ...
    Requests per second:    7042.34 [#/sec] (mean)
    Time per request:       290.812 [ms] (mean)
    Time per request:       0.142 [ms] (mean, across all concurrent requests)

At `-c 2048` the site remains usable... It just wont die. I am saturating all cores,
(you'd cook your processors) but I'm not spiking the ram... Not caching yet... The event logs are squeaky clean.

In comparison the
`SqlServerTopicStore` yields 2k req/sec with 300k topics, and 2.7k req/sec
with only 7.5k topics... `MongoTopicStore` has been tested with 7.5k, 300k,
and 780k topics, and yields the same benchmark on this test for each.

In terms of write, Mongo will load 780k topics faster than Sql Server will 
load 7.5k topics. If write throughput is in any way important to you
Mongo starts becoming compelling. You can always use a write-through store
pointing at Mongo on the front and Sql Server on the back for belt and braces.
I know we played about with this early on, but a couple of us latterly
took this approach quite a bit in production and it worked well. Rather than
thinking in terms of caching, we thought in terms of fast and slow stores,
and a write-through interface... `MongoTopicStore` looks a good candidate
for a "fast store". I'm still not convinced about Redis for this model,
so Mongo might wind up **it**.

I'm aware of why you wouldn't want to hook up a life-support machine to Mongo,
but most commercial Web applications aren't anywhere in the same proximity
as a life-support machine. No transactions, no locks and dirty reads wont 
magically save you by virtue of being on a rdbms either. 

Lastly `GetPointingTopics` is a comparatively cheap query on Mongo.

The next step is doing throughput tests using random topic selection so I'm not just
testing a hot-spot on a cache somewhere.

I would still expect the real-world experience of a real application backed by
Mongo to come in around 2k r/s, as this test is still quite synthetic.

### Update

7.6k now... I've also hit it with siege, and it just wont fall over... because there's no caching, ram usage is near constant.... -c 8192 will drop it to 5.5k, and the load times jump up to 2s, but it maintains throughput, and stays stable... You'd cook your CPUs however.

I have a vague and unspecified suspicion that the managed state and immutability has more of an impact than is obvious. You can wire it up in complex patterns, but the processing and state model is actually very stripped back and simple.

### Model

This store interacts with the following model:-

	{
	  "_id" : ObjectId("525d4a0f992df13340096f91"),
	  "_type" : "topic",
	  "id" : "Top/Regional/North_America/United_States/Colorado/Counties/Adams",
	  "metadata" : [{
	      "_type" : "metadata",
	      "for" : "Top/Regional/North_America/United_States/Colorado/Counties/Adams",
	      "scope" : "default",
	      "name" : "last-update",
	      "value" : "2007-01-02 22:52:21"
	    }, {
	      "_type" : "metadata",
	      "for" : "Top/Regional/North_America/United_States/Colorado/Counties/Adams",
	      "scope" : "default",
	      "name" : "label",
	      "value" : "Adams"
	    }, {
	      "_type" : "metadata",
	      "for" : "Top/Regional/North_America/United_States/Colorado/Counties/Adams",
	      "scope" : "default",
	      "name" : "description",
	      "value" : "County Demographics: population was 265,038 in 199"
	    }],
	  "associations" : [{
	      "_type" : "association",
	      "parent" : "Top/Regional/North_America/United_States/Colorado/Counties/Adams",
	      "type" : "navigation",
	      "scope" : "default",
	      "reference" : "Top/Regional/North_America/United_States/Colorado/Regions/Front_Range",
	      "role" : "related",
	      "id" : "26a9fdb3-1f87-403f-ae02-5c2cb559e637",
	      "metadata" : [{
	          "_type" : "metadata",
	          "for" : "26a9fdb3-1f87-403f-ae02-5c2cb559e637",
	          "scope" : "default",
	          "name" : "label",
	          "value" : "Front Range"
	        }]
	    }],
	  "occurrences" : [{
	      "_type" : "occurrence",
	      "for" : "Top/Regional/North_America/United_States/Colorado/Counties/Adams",
	      "scope" : "default",
	      "role" : "wiki",
	      "behaviour" : "markdown",
	      "reference" : "self",
	      "string-data" : "To be or not to be that is the question.",
	      "resolved-model" : undefined
	    }]
	}

And the store further assumes the indexes:-
 
* `{"id": 1}`
* `{"associations.id": 1}`
* `{"associations.reference": 1}`

The last index requirement will be moved to the implementation of 
`IExtendedTopicStore` as it is needed for `GetPointingTopics`.

* [Conclave.Mongo project folder](./Conclave.Mongo/)

<a name="Conclave.MySql"></a>
## `Conclave.MySql`, Project Notes

This test is run with 7.5k topics, and is actually using MariaDB, not MySql.

The results for MariaDB unlike SqlServer were a lot more spiky, varying from
test to test from 1.9k r/s to 2.4k r/s. This one was pretty representative.

#### -c 8
	
	Server Software:        Microsoft-IIS/8.0
	Server Hostname:        localhost
	Server Port:            80
	
	Document Path:          /conclave.cms/public/topic/view.aspx?id=Kids_and_Teens/
							Entertainment/Museums/Social_Studies/History
	Document Length:        5172 bytes
	
	Concurrency Level:      8
	Time taken for tests:   4.751 seconds
	Complete requests:      10000
	Failed requests:        86
	   (Connect: 0, Receive: 0, Length: 86, Exceptions: 0)
	Write errors:           0
	Total transferred:      57440086 bytes
	HTML transferred:       51720086 bytes
	Requests per second:    2104.99 [#/sec] (mean)
	Time per request:       3.800 [ms] (mean)
	Time per request:       0.475 [ms] (mean, across all concurrent requests)
	Transfer rate:          11807.72 [Kbytes/sec] received
	
	Connection Times (ms)
	              min  mean[+/-sd] median   max
	Connect:        0    0   0.3      0       8
	Processing:     2    4   1.7      3      23
	Waiting:        2    4   1.7      3      23
	Total:          2    4   1.7      3      23
	
	Percentage of the requests served within a certain time (ms)
	  50%      3
	  66%      4
	  75%      4
	  80%      4
	  90%      4
	  95%      5
	  98%     11
	  99%     12
	 100%     23 (longest request)


* [Conclave.MySql project folder](./Conclave.MySql/)

<a name="Conclave.Process"></a>
## `Conclave.Process`, Project notes
#### Finding a way to make anarchy work, through inversion of behavioural control

This is the meat and potatoes of Conclave. It's the experiment itself.
Conclave is an "opinionated" piece of software not because I'm convinced, but
because this is the idea I'm trying out.

`Conclave.Process` seeks to have a clean and distinct layer from
`Conclave.Web`, but they do dovetail very closely, with the Web concern extending
the processing model. There's still a little tension along this line, with
`WebContext.Services` and `.Application` probably needing to move up to
`ProcessContext`

The processing model can't really be assured until I implement a desktop
application with it, as something like that is the only way to truly expose
any request/response assumptions.

This processing model has been in production for 2 years in a data quality
service, with a narrow request time-frame, and very chunky backing data. As
a data-quality service it operated as a simple rule based inference engine.
In the last year it has been used as a general purpose application framework
for a registration process, as a message queue worker process, and as a point
of integration between a legacy application and a large third-party service.

### Processing Model

Conclave doesn't focus on MVC, MVVM, HMVC, MVA, MVP or some other M-pattern... Yeah
I had to check [WikiPedia](http://en.wikipedia.org/wiki/Model%E2%80%93view%E2%80%93controller)... 
It focuses instead on supporting a processing model, and lets
the development team decide what pattern of process suits them.

MVC.NET is also something of an incumbent in the .NET Web-dev environment at the
moment, and while I think it's a wonderful framework for many Websites, I feel
for applications that are going to grow unpredictably in complexity over time
while contending with developer churn in the maintaining team... in such
applications I feel MVC.NET doesn't answer cross-cutting concerns well.

#### Cross-cutting concerns

This bit is too important for me to waffle, 
and WikiPedia says it better than I could,
[Cross-cutting](http://en.wikipedia.org/wiki/Cross-cutting_concern):-

>In computer science, cross-cutting concerns are aspects of a program that 
affect other concerns. These concerns often cannot be cleanly decomposed 
from the rest of the system in both the design and implementation, and 
can result in either scattering (code duplication), tangling (significant 
dependencies between systems), or both.
	
>For instance, if writing an application for handling medical records, the 
indexing of such records is a core concern, while logging a history of 
changes to the record database or user database, or an authentication 
system, would be cross-cutting concerns since they touch more parts of 
the program.

While many of the architectural concerns such as authentication and
authorisation can be anticipated many of the emergent business concerns
can't be anticipated well, and many of them will cut across your initial
application partitioning. If you'd decided on a well considered MVC and
RESTful applications, those cross-cutting concerns are what will choke the
original architectural design, like ivy round a tree.

[Aspect Oriented Programming](http://en.wikipedia.org/wiki/Aspect-oriented_programming)
addresses this:-

> In computing, aspect-oriented programming (AOP) is a programming paradigm that aims to increase modularity by allowing the separation of cross-cutting concerns. AOP forms a basis for aspect-oriented software development.

> AOP includes programming methods and tools that support the modularization of concerns **at the level of the source code**, while "aspect-oriented software development" refers to a whole engineering discipline.

The emphasis is mine, and highlights why AOP hasn't made it very far in Web
development for .NET, and isn't likely to as its invasive and exotic.

So one of the goals here is to cater for cross-cutting concerns that mug you
in the middle of user acceptance testing, with an emphasis on expecting
business logic to emerge over years.


### Behaviour Oriented Development

Before we get into this, let me be clear... I love me a bit of OO. The .NET
implementation of Conclave is quite obviously implemented as an OO application, and
before any other design goal it seeks to be a reasonable, well considered, and
safe OO design. In recent years I've become increasingly aware of what the
functional chaps have been screaming for decades. The huge, byzantine, 
hierarchical, specialised APIs, with sprawling mutable state are insane
and not supportable long-term by most modest development teams. Applications
collapse under their own weight unless steps have been taken to mitigate this.

The idea of a behaviour is a simple one, based on old well founded ideas of
message passing, and on some platforms would be regarded as prosaic.

You have a **process context** which maintains execution state, including input
and output state. In some applications
this may be long-lived, and in applications like Web apps they will tend to be
short-lived and represent the execution of a request.

The basic unit of work is a described as a **process behaviour**, which is a
simple construct being a tuple of delegates, **condition** which has a boolean
return value and should ideally be deterministic and certainly yield no 
side-effect; and **action** which will normally yield side-effects. Both
these delegates act upon an **event**, which maintains its own set of
parameters, *and a reference to the context*.

The context maintains a collection of behaviours, and when and event is fired
on the context, the condition of each behaviour is visited (no order
guaranteed) and passed the event being fired. For any behaviour whose condition
returns `true`, its action is executed. That's it.

Actually while logically that's it, the implementation uses Rx.NET:-

	public void Register(IProcessBehaviour behaviour) {
		this.Bus.Where(behaviour.Condition).Subscribe(behaviour.Action);
	}

Where the `.Bus` is an Rx.NET `ISubject<IEvent>`, and the underlying fire method
is implemented as:-

	public IEvent Fire(IEvent ev) {
		if (ev.Context != this) throw new ProcessException(...);
		try {
			this.Bus.OnNext(ev);
		} catch (Exception err) {
			this.Errors.Add(...);
		}
		return ev;
	}

The **hosting application** is responsible for initialising the context
including its initial state, and firing the first event. Thereafter further
events can be fired by the host application upon the context, or more normally
the initial behaviours will fire further events.

In a Web application that looks like:-

	public void ProcessRequest(WebContext context) {
		var behaviours = context.Services.GetObject<List<IProcessBehaviour>>("request-behaviours");
		context.Register(behaviours);
		context.Fire("process-request");
		context.Completed();
	}

Noting that this is a `WebContext` and comes with an initial state as part of
`WebContext.Request`, and a service container for not just services but
also config.

Now the behaviour that lights up when the message `{process-request}` is fired
if one called `SimpleSequenceBehaviour`, which is configured to iterate
over a sequence of messages called the *life-cycle*, and fire them.

	<list element-type="string">
		<value>bootstrap</value>
		<value>parse-request</value>
		<value>authenticate</value>
		<value>work</value>
		<value>view-state</value>
		<value>process-views</value>
		<value>render</value>
	</list>

Behaviours that respond to these messages may in turn emit more of their own.

If you wanted to change this life-cycle, you could edit the list and add your
own behaviours. If you wanted something very different happening for
`ProcessRequest` you could. You might load some initial state for the user
from the database into `context.Params` or `context.Flags`, then load
a life-cycle specific to the user from the database, and use those. I'm not
recommending that, just highlighting that we've only just got off the starting
blocks, and already you can wire it up whatever which way takes your fancy.

Side-effects from behaviours can be outside the system, writes to a database
being common, but within the application state is maintained on message events
, and upon `context.ControlState`. An example here is of a behaviour whose
action is to obtain an entity (here a `Topic`) from backing storage, and placing
it in the `ControlState`

		public override void Action(IEvent ev, WebContext context) {
			if (ev.HasRequiredParams("id")) {
				if (!context.ControlState.ContainsKey("topic")) { // only get the topic if it hasn't already been got
					using (ITopicStore store = context.Services.GetObject<ITopicStore>("store::topic-map")) {
						store.Start();
						Topic topic = store.GetTopic(ev["id"]);
						context.ControlState["topic"] = topic;
						if (topic == Topic.Blank) {
							context.Errors.CreateMessage(...);
						}
					}
				}
			}
		}

That's all there is to it really. Behaviours are just loosely coupled procedures, with a generalised interface and a fancy name because I needed to call them something.
`Conclave.Web` and `Conclave.Web.CMS` are a bunch of behaviours covering a range 
of common activity for Web applications and will be grown over time.

If we take a look at `Conclave.Web.RenderBehaviour`:-

	public override void Action(IEvent ev, WebContext context) {
		// first check that we have something to write to the response stream
		if (context.ViewSteps.HasSteps) {
			// then check to see if it's text to be written out
			if (context.ViewSteps.Last.HasContent) {
				context.Response.ContentType = context.ViewSteps.Last.ContentType;
				context.Response.Write(context.ViewSteps.Last.Content);
			} else {
				throw new WebException("No content was generated by the last view step to render.");
			}
		} else {
			throw new WebException("There are no view steps to render for this request.");
		}
	}

In Conclave we attempt to implement as much process as possible as behaviours.
Whether it be fetching content, registering a new user and writing them to the
database, authenticating or writing metrics, they're all implemented as a behaviour.

This approach is very flexible and very fast. In benchmarks and profiles, loading
behaviours, and checking all behaviours conditions repeatedly is not a performance
concern. Not even close. The interaction with the service container is more of a concern that any condition processing. On that note, behaviours are singletons for
safety as well as performance reasons.

### Shipping Containers
A [Mr. Malcom McLean](http://en.wikipedia.org/wiki/Malcom_McLean) invented the
shipping container (based on prior work) in the 1950s, and opinion seems
to vary between it having had a significant impact on the the world economy
to it being a game changer.

Before the shipping container, goods would come in an near infinite variety
of containers. Rigid, soft, regular or irregular in shape, all manner of
sizes, and all degrees of robustness. Integration between different modes of
transport involved unique requirements at each stage. With goods holding
at docks and depots, and the constant loading and unloading, breakages and
spoilage was common, and security that was near impossible to impose.

It was chaos, and the shipping container cut through it all. From the warehouse
on truck, to a train at the station, then to the docks and onto a ship, and
the reverse on the other end... the one container moved and was handled with
standardised equipment and procedures. Secure, robust, and well understood.

In Conclave the `IProcessBehaviour` is the shipping container. One size fits
all. No special little snow-flakes. Exercise your creative genius in how
you wire the behaviours together for your processes, not a dozen bespoke
APIs. What happens inside a behaviour is an implementation detail. An
application is wired together as the interactions between a context and
behaviours.

Application architects and developers are given clear parameters of the space
in which they can work. If everybody sticks to the containers, any implementation
can be swapped out, and lines of responsibility are cleanly drawn. When learning
OO we all all learnt about nice neat modular boxes, that would all just plug
together into different shapes, and yet we still keep on coming across
applications that are a crazy nest of wires... I decided to try and make the
boxes. I decided to try and do what we were taught was meant to happen.

### Inversion of Behavioural Control

We appreciate the value of the `interface`, as it shields us from implementation.
But we're still left instantiating concrete types with `new`.

	ITopicMap store = new MySqlTopicMap(...)

So we don't get the full benefits from the abstraction. We appreciate then
the facility of service containers such as Spring and Autofac which allow
us to abstract away this instantiation with concrete types.

	ITopicStore store = context.Services.GetObject<ITopicStore>("store::topic-map")

In this case, from the application code we know nothing about the implementing
type and concern ourselves only with the interface. The less we know about
the implementation, the more decoupled we become, the generic our code becomes,
and most importantly, the more resilient to change it becomes.

We're still left knowing a lot about this component, and it still remains
very distinct from other components. At the moment `ITopicStore` has 37 methods.
After refactoring, this is likely to drop down to around 20 methods, but that's
still a lot to know about the size and shape of this component, especially if
I only want to get a topic by id. If `ITopicStore` were a third-party interface, we wouldn't every have control over it.

Consider this method:-

	protected virtual void View(IEvent ev, WebContext context) {
		if (context.HasRequiredParams("id")) {
			context.FireWith("topicmap::get-topic", "id");
			context.Fire("topicmap::resolve-navigation");
			context.Fire("topicmap::resolve-occurrences");
		}
	}

Behaviours decide what they will handle, not the code initiating the
imperative. *Behavioural control has been inverted*. If no behaviour
reacts to these messages, nothing will happen. What we have is a processing
model that allows us to inform the system, and then the system decide
what it wants to do about it.

In a broader system architecture, if we were talking about wiring together
different processes using message queues, this would be matter-of-fact.
Erlang developers will have a smug grin on their faces about now... All we've
done is taken this principle of notifying the system via a generic interface
and made it our primary mechanism.

Feature switching emerges out of this quite naturally as an almost
inevitable consequence. Which I take as a good sign.

### Imperative and Reactive (Signal and State)

There's a balance of style here with two ends to a scale.

On one end of the scale is the imperative style, which involves signalling
a specific component a concrete imperative, acting upon a local state.

At the other end is the reactive approach where components observe shared
state, react to and modify shared state.

Conclave supports both these approaches along with the grey bit in the middle.

It is envisioned that most applications built upon Conclave will start
favouring an imperative approach and became more reactive over time, but in
truth this is a matter of taste.

The view method above, is changed to a more reactive approach bellow:-

	protected virtual void View(IEvent ev, WebContext context) {
		context.FireWith("topicmap::get-topic", "id");
	}

Where the behaviour handling `{topicmap::get-topic}` will emit
`{topicmap::get-topic-done}` when it has finished. Any behaviour
that it interesting in processing on the bases of a newly fetched topic
would then listen for this message, and check if 
`context.ControlState.ContainsKey("topic")`, and act if this condition is met.

This approach is more extensible but can be harder to reason about what exactly
is happening in a system.

### Ease of testing is an architectural indicator

Testing a component discretely, removed from its hosting system is an interesting
thing, as the first thing you're testing is the ability to operate the component
separated from the rest of the system.

If a component is easy to test, it will tend to be more discrete and easier
to swap out. Inversely, being hard to test is often a warning sign
that the component is tightly coupled with other parts of the system.

One of the measures for behaviours in Conclave as they are developed is
the ease with which they can not just have unit tests written for them
but whether a developer can use a component from a command-line harness. Can
another developer pick the behaviour up and play with it out of curiosity
without getting snarled in dependencies?

`Conclave.Web` explores this in more details concerning the very heavy
dependency that `HttpContext` represents and the steps taken to ensure
that those behaviours that actually touch `HttpContext` are kept to only
what is essential... Anything touching `HttpContext` is a lot harder to test.

### Portability is an architectural indicator

How easy would it be to port from .NET to Java? How about Python or JavaScript?

The bits that survive are architecture. The bits that don't are implementation.
That's perhaps a little over-stated, to make a point, 
especially with similar imperative languages, there remains value however in considering
what of our codebase would be lost in translation.

`context.Bus` which exposes an `ISubject<IEvent>` used to be a public member,
but became protected. When I asked myself what it would be like to port
`ProcessContext` to Java, it was obvious that `ISubject<IEvent>` is an implementation
detail, and needed to not be public. We have instead the `.Register(...)` and
`.Fire(...)` generalised methods, which are easy to implement
on differing systems via a variety
of means including a simple collection of behaviours to be iterated.

Context and behaviour is intended to be easy to port and reimplement. The focus
is upon a processing model, and using that to focus on our applications
processes as distinct from implementation, in as light-weight a manner as possible.

`Conclave.Process` also seeks to facilitate portable applications. If the
behaviours do the same thing, regardless of the implementation detail, the
application will do the same thing. This is a simplification, but it's a
guiding principle, not assertion of an absolute.

### Change can and will come from unforeseen angles
*Change is the Zombie Apocalypse. If you're not prepared, it will devour you and all you love.* 

The whole processing model is built upon the paranoid notion, that you're going 
to need to change parts of your system radically, without impacting anything else, 
and you can't know which parts ahead of time. 
It extends this with the belief that your requirements are going to change
over time and the application that you started writing may well not be
the application you end up with years down the line.

Writing a Web application and then saying "done" is not the challenge. That's
just the price of admission. It's a long-term game and the challenge is to bend 
that application into new shapes as the business processes grow, without breaking it, 
with a regular Web development team.

### Messages cross machine-boundaries well

It's kind of obvious, but worth stating.

We'd built a simplified message queue which we called the JobProcessor, that
was backed by MongoDB (with automatic fail-over to a Sql Server implementation, 
don't you start scowling at me), and worker processes built with the same behavioural
model (same libs) as the application. So we'd realised these messages made barriers more
permeable, and the behavioural model could spread across a system.

I had mentioned to a colleague (The-Freakishly-Fast-One, he's more code
than man now), that in the same way we presented a simple list of messages
and errors to the front end, it would be cool if we could have an ordered
list of messages destined for the client, a behavioural model in JavaScript for
the user agent, and pump those messages into the client-side process context on
page load. Using ajax requests the client behaviours could still execute against
the server and maintain the cycle of message pumping. Well, Mr. Freakishly Fast
knocked it up in about an hour and a half, and it worked.

Behaviours in the client, in the Web application, and for distributed processes
scattered across the system. Now this was hugely exciting and novel for a team
of .NET Web developers, but it's worth underlining that if you were writing
the back-end for an online poker system in Erlang, this would sound like
baby-talk, and the implementations beyond crude. Sounding exotic to the ears
of .NET developers does not make it so.

I just too a look at [Riot.js](https://moot.it/blog/technology/riotjs-the-1kb-mvp-framework.html) and it seems to be doing a similar thing with the 
[Observer Pattern](http://en.wikipedia.org/wiki/Observer_pattern). In Conclave
everything is a potential observer and we notify the system not dependent
observers directly.

This makes me a lot more eager to port Conclave to NodeJS. Not because of Node
itself, it's fantastically interesting and wonderfully bendy, but I've done like
for like in Node and a .NET `HttpListener` with attention to async, and there's
nothing magical happening in Node... However, the same model and implementation
distributed across the whole system, backed by a document store expressing
JSON, does sound super sexy in a "turtles all the way down" kind of way.

### "Done" is mostly an illusion

If you're working in a pukka engineering outfit, you'll know it, you'll know
my statement doesn't apply to you, and why.

If you have a particularly good PM or Scrum Master, and a company fully engaged in
some flavour of Agile project coordination, and a firm understanding of iterative
development, you'll know it, you'll know my statement doesn't apply to you, and
why. You may also appreciate why a framework that focuses on
composition and loose coupling of behaviour may benefit iterative development.

Every other commercial Web application developer... "done" is a stick used to
beat you about the head with. Neither you, nor anybody around you could say
what done looks like, and its a rather strange concept for an application that
has had a full team of developers changing it every single day for the last
three years.

For the majority of Web developers, uncertainty and change is the norm. Every
initial implementation is in truth a prototype. 

We need to start writing systems that are in keeping with our commercial realities.

With the focus on behavioural composition, and the flexibility it affords, it
is hoped initial poorly understood implementations can be replaced with better
understood implementations without impact. It's hoped that applications can
be rewired without involving a rewrite, and that the application can not
only be extended, but that new concerns can be woven through the main body
of the application without wreaking havoc. The intention is to provision for an
application that is a constant state of rewrite, and "done" is only ever
"done, for now".

## Developer Oriented Development

*"Commercial software development is first and foremost a social exercise
before it is anything else."*

I've found myself repeating this phrase to my teams and managers for a couple
of years now. I'm still not entirely sure what I mean by it, but I continue
to believe it is essentially true. Before any code is written you have a group
of people trying to solve problems. While the application is being developed,
and afterwards when it is being maintained and extended, it is always a group
of people trying to reason about and solve problems.

Now the intent here isn't to engage in abstract emotive declarations. If
we accept that we're dealing with a group of people before anything else,
then our software and systems need to be mindful of the needs of those
people from the start and always. The developer is the first and primary
user of any system. Systems that do not cater to these primary users
will encounter increasing difficulties over time.

Ruby, despite any severe performance issues enjoyed amazing success. In Web
development Rails hit like a tidal wave despite being built on top of these
performance issues. Both Ruby and Rails were very consciously targeted not
just an what developers needed, but further at making the activity of
development fun. Ruby and Rails allowed a broad swathe of Web developers
to reason about their problems domain and share their insights with each
other with a common basis of vocabulary. Rails provided a sane starting
point for developers and said "like this", with the expectations that developers
would cherry-pick what they needed from the stack, and swap in their own
implementations to scratch their own particular itches.

The behavioural composition favoured by Conclave hopes to give Web developers
a simple, and widely applicable conceptual model and vocabulary that facilitates
them working together in teams. With clear definitions of the common units
of work, it becomes easier for developers to work together as it becomes
easier for the components they are developing to work together. Friction
between component parts will it is felt tend to translate into friction between
developers.  

An architectural model that favours change and evolution over time, 
more closely matches the activities that most Web developers find
themselves engaged in. As developers and architects explore and come to understand
their problems better, so the application can change with their changing understanding.

Anxious and fearful developers are not productive. Developers are anxious 
when they know they haven't got a clue how to build the thing they've
just been tasked with, and downright fearful when they're not sure where to
start, with the knowledge that if they make the wrong calls now, the whole
thing is likely to fail. Conclave seeks to alleviate that anxiety at least
in part by saying, "start like this".

**TL;DR** Systems need to be designed for their developers needs, and more
broadly the needs of their teams.

### Developer churn

A commercial reality for many development teams is that developers come and
go, and if you have a developer for longer than 2 years, it's a bonus that
can't be relied upon.

The people that developed the system you are working on may no longer be
around, and you find yourself working with the people who knew them. If they
are still around, they have probably moved onto other projects and teams. Even
if you can collar them to talk about code they wrote 3 years ago; code that
they wrote 3 years ago and now feel vaguely ashamed about, is not at the top
of their list of things they like to talk about.

As time moves on the areas of a system labelled "here be dragons", will tend
to grow. Bringing new starters in a team up to speed is a very real
problem and can reduce drastically the amount of productive time you
get from a developer over the limited time that you have them.

By concentrating on discrete units of functionality, and focusing on loose coupling,
and least possible knowledge by a component in order to do its job, the intent
in part is to make it easier to enable new-starters to engage in real and
productive work as they are being inducted into the broader system.

### Climbing over the corpses of the developers who went before you

When a system does not make it clear how new elements of functionality
are to be introduced, developers start hijacking each others code. They look
for a method that occurs roughly in the right place in the life-cycle, and
they inject their code there. A method might be called `RegisterUser`, but
after three years it's got five radically different business concerns
touching completely different parts of the system, all piggy-backing
on the original code.

A system has to make it very clear how new functionality is introduced,
and it needs to make such introduction as easy as possible with provision
for the least impact to existing functionality. In Conclave behaviours
have a narrow and defined scope of purpose, and they stick to it. If
you're introducing new functionality, you're introducing new behaviour.

This seems like a trivial thing, and it would be easy to dismiss it's importance,
in my own experience its a significant reality that is not adequately addressed. 

### Ideas churn, or constantly reinventing wheels

It's not only developers that churn, its the ideas that come and go with them.
Even among those developers that stick around, their views on how to write code
today are different than they were three years ago, and different from how
they will write code in three years time. Implementation details are as
likely to be based upon a trending blog-post last week as they are upon
experience or testing of candidate implementations.

Some of these ideas will be good, and some of them will be truly terrible. Your
not going to be able to keep the terrible ideas out, they are going to wind up
in your application. All you can do, is acknowledge that this is a commercial
reality and seek to mitigate its harm over time.

With the focus on behavioural composition, and most importantly a focus on
what is the internal interface of the application and what is implementation
detail, the intent is that bad decisions can be backed out of, and wont
infect the whole system.

From this way of looking at this, whether you are using parameterised queries,
stored procs, or Entity Framework for `GetTopicBehaviour` in an implementation
detail. It is not part of the concern of application composition.

Conclave has further opinions in this area expressed in `Conclave.Data` and 
`Conclave.Map` based on a simple model/store pattern.

### Roll your own framework

Web applications can be seen as an extension of the framework upon which they
are built. One of the goals of Conclave is that it can facilitate a Web
development team to roll their own Web framework that meets the needs of their
application. It is hoped that `Conclave.Process` is a sane and flexible starting
point for this. `Conclave.Web` and `Conclave.CMS` then serve as an example
of how this might take place, largely with a choice of conventions. Your
referred conventions might be different than mine, or your application and
organisation might dictate conventions that are different than mine. Conclave is
a starting point, it's a prototype framework, it will always be a prototype. It
might serve as a prototype for your team and application. 

### The Scenario

> You're team lead of 6 Web developers with a wide range of experience,
personality types, and people skills. You're the closest thing to a
Scrum-master your team is going to see. Likewise for better or for worse
you are often essentially the architect. You're also a mentor and teacher
and hopefully working hard to ensure your team are happy, healthy and
productive.

> A new mid-level developer is joining your team. 
You would like to get the developer up to speed with production code
as quickly as possible. You may previously have worked at places where it can
take a good 6 months before developers are genuinely useful to the team. You'd
like to have the new starter working on production code next week, working
alongside their team-mates usefully and productively, without being in reality
a burden for whomever gets lumbered with them.

> In the mid-term, you'd like a clear idea of how you're going to induct them into
the teams patterns and practices over the next 3 months, with the confidence
that they'll know what they need to know to work on 80% of the system.
You'd find it encouraging if you heard senior members of the team telling
the new starter "this is how we do things here" rather than relaying to them
an inexhaustible list of obscure idiosyncrasies and edge cases as they
are introduced to four different Data Access Layers built upon four completely
different technologies, and a thousand special little snow-flakes built on
top of that.

> For now, this week you'd like them to settle into the teams tool-chain, and the
companies procedures. While they're doing that Jake will help them set up
a "hello world" application on their box. Next week they're paired with Jake
who has four behaviours to write, and the new starter is going to write the 
unit tests for them. The week after they're going to swap, and the new starter is going to write the behaviours and Jake will write the tests.

> From there, the new-starter will know how they can contribute
units of functionality to the teams efforts, and the team will know how they
can make proper use of the new-starter as their understanding of the
system grows.

Now you're off to the races. Hopefully the new-starter can find the process,
fun, interesting, and most importantly affirming (anxious developers are
high maintenance, so alleviate anxiety where you can).

With a basic understanding of behavioural composition, the new member of your
team has a concrete understanding of the systems basic units of code,
and discrete tasks that they are able to achieve without understanding the
wider system. This is the same simplicity afforded by a controller in MVC,
but for everything. The new member is alleviated from the anxiety of not
being sure where they can inject their implementation, and how to
introduce code alongside other developers implementation without causing
unforeseen side-effects in a large system that they know nothing about.

As the new starter is inducted into the team, you hope their work will be
transparent enough to the broader team so they can be saved from pit-falls.

The flip-side of the coin, the inverse scenario, is how you retain the experienced
members of your team without them becoming frustrated and disenchanted with
a monolithic nightmare of an application that can't be tamed, and reduces
large swathes of development time to maintenance. These are the things that
drive your best developers away.

These are the problems that `Conclave.Process` is trying to solve. In commercial
development, this is the important stuff. In part Conclave is trying to remain
mindful of the human requirements within a commercial Web development team, with
the belief that if these human requirements are failed, no other requirements
will be well met.

## Prior Art

*There's nothing new under the sun.*

Experience tells us in software development if you have a good idea, or well defined problems, the first statement should probably be *"somebody has done this before"*,
followed by a flurry of research on prior-art. It's very rare unless you
genuinely are on the cutting-edge (and even then) that you'll come up
against something that isn't a *solved problem*.

Googling [Behaviour Oriented Design](https://www.google.co.uk/#q=behaviour+oriented+design)
turns up some interesting results. Turns out, predictably enough, it's a thing.
BOD is a pattern used in the shallow end of AI as an organisational pattern. Lots
of interesting terms crop up around this material,
[Reactive, Behaviour Based Artificial Intelligence](http://www.cs.bath.ac.uk/~jjb/web/rbb.html), *"behavior-based reasoning"*, *"reactive reasoning"*.

<http://www.cs.bath.ac.uk/~jjb/ftp/AgeS02.pdf> appears to be quite a central paper
to the thesis, and the approach seems to have become a regular part of the pallete
of patterns used in the game industry. 

Business-logic in commercial applications it is felt can benefit from the mechanism
in the same way that actors within a game can.

if I had followed my own advice and looked for the wheel somebody had already
made earlier on in the project I would have proceeded in a far bolder (and quicker)
fashion, as I wouldn't have sought quite so much affirmation of the pattern
before trusting it. It's a thing, I haven't just plucked it out of thin air...
whether or not it's a worthwhile thing still warrants critical consideration,
but it's not merely a novel approach. Others have arrived at the same place
in seeking to solve their problems. Some of them seem a good bit brighter than me.
This is a working developer arriving at the same point as a bunch of computer
scientists 20 years after the fact, and saying *"yeah we need this stuff"*.



* [Conclave.Process project folder](./Conclave.Process/)

<a name="Conclave.SqlServer"></a>
## `Conclave.SqlServer`, Project Notes

This test is run with 300k topics.

Although not at the performance levels of the Mongo implementation it's better
that I was expecting.

#### -c 8
	
	Server Software:        Microsoft-IIS/8.0
	Server Hostname:        localhost
	Server Port:            80

	Document Path:          /conclave.cms/public/topic/view.aspx?id=Top/Business/Arts_and_Entertainment/Sports/Facilities
	Document Length:        5261 bytes

	Concurrency Level:      8
	Time taken for tests:   3.319 seconds
	Complete requests:      10000
	Failed requests:        63
	   (Connect: 0, Receive: 0, Length: 63, Exceptions: 0)
	Write errors:           0
	Keep-Alive requests:    10000
	Total transferred:      58380063 bytes
	HTML transferred:       52610063 bytes
	Requests per second:    3012.57 [#/sec] (mean)
	Time per request:       2.656 [ms] (mean)
	Time per request:       0.332 [ms] (mean, across all concurrent requests)
	Transfer rate:          17175.21 [Kbytes/sec] received

	Connection Times (ms)
				  min  mean[+/-sd] median   max
	Connect:        0    0   0.0      0       1
	Processing:     2    3   1.8      2      22
	Waiting:        2    3   1.8      2      22
	Total:          2    3   1.8      2      22

	Percentage of the requests served within a certain time (ms)
	  50%      2
	  66%      3
	  75%      3
	  80%      3
	  90%      3
	  95%      3
	  98%     10
	  99%     11
	 100%     22 (longest request)

#### -c 256

	Concurrency Level:      256
	...
	Requests per second:    2987.37 [#/sec] (mean)
	Time per request:       85.694 [ms] (mean)
	Time per request:       0.335 [ms] (mean, across all concurrent requests)

#### -c 512

	Concurrency Level:      512
	...
	Requests per second:    2851.89 [#/sec] (mean)
	Time per request:       179.530 [ms] (mean)
	Time per request:       0.351 [ms] (mean, across all concurrent requests)

#### -c 1024

	Concurrency Level:      1024
	...
	Requests per second:    4552.33 [#/sec] (mean)
	Time per request:       224.940 [ms] (mean)
	Time per request:       0.220 [ms] (mean, across all concurrent requests)

#### -c 2048

I'd do more tests before I'd fully trust this.

	Concurrency Level:      2048
	...
	Requests per second:    6917.62 [#/sec] (mean)
	Time per request:       296.056 [ms] (mean)
	Time per request:       0.145 [ms] (mean, across all concurrent requests)



* [Conclave.SqlServer project folder](./Conclave.SqlServer/)

<a name="Conclave.Web"></a>
## `Conclave.Web`, Project Notes

Like Acumen, Conclave extends `HttpApplication` as `WebApplication`. Implements
`IHttpHandler` as `DefaultHandler`, and wraps `HttpContext` as `WebContext`,
`HttpRequest` as `WebRequest` and `HttpResponse` as `WebResponse`. Behaviours
are loaded in the `DefaultHandler`, and the first event fired from there. Like
much else in Conclave, these types are expected to be starting points.


I'll beef the API documentation up for the behaviours as quickly as possible.
Until then, look at the life-cycle configured in `config.xml`, then
check the behaviours configured in `behaviours.xml`

You can treat behaviours in a course grained fashion if it suits, like
controllers, and to start off with this isn't a bad idea. Unlike MVC.NET
there is no routing in Conclave. `ParseRequestBehaviour` is the closest
to routing, instead decomposing the request into a set of parameters which
the conditions of other behaviours may consider. This uses the URL form
of `/area/concern/action.aspx/view?querystring` from Acumen. Again, as a
starting point, it's expected applications will arrange their own
URL and parameter scheme.

So when `{work}` comes around in the life-cycle, which behaviours respond
will be based in large part what parameters were setup by the parse request
behaviour.

The view pipeline works quite well in Conclave, unlike previous versions, and
is processed as a series of steps, piping from one step to the other.

`XslViewBehaviour` and `RazorViewBehaviour` will both cascade up a directory
structure looking for a template named after the action, or named a default.

	private IEnumerable<string> _possibleTemplates(WebContext context) {
		string area = context.Params["area"];
		string concern = context.Params["concern"];
		string action = String.Format("{0}.xslt", context.Params["action"]);

		// area/concern/action
		yield return Path.Combine(area, concern, action);
		yield return Path.Combine(area, concern, "default.xslt");
		// area/action
		yield return Path.Combine(area, action);
		yield return Path.Combine(area, "default.xslt");
		// concern/action
		yield return Path.Combine(concern, action);
		yield return Path.Combine(concern, "default.xslt");
		// action
		yield return action;
		yield return "default.xslt";
	}

The use of `yield return` here makes it easy to modify this lookup to a scheme
that suits you.

### There's a reason for XML and XSLT
And the reason remains compelling despite XSLT being tricky.

I get it why people favour Razor and similar, for the same reason the super-saturated
BigMac is also so tasty. That doesn't make it good for your application.

While it is possible to blow a hole through the wall and yield side-effects
from XSLT, you have to really go out of your way to do it, plugging in extensions.
Out of the box, you can't yield side-effects. In
[String Template](http://www.stringtemplate.org/about.html) you can't yield
side-effects.

In Razor, you can do whatever you want, including query the database. Razor leads
to questions like: [Is try catch in a view bad practice?](http://stackoverflow.com/questions/9845391/is-try-catch-in-a-view-bad-practice). Not just on the .NET
platform, but as an industry we know allowing anything and everything in a
view template is an atrocious idea. This isn't a controversial position. I would
like the person who thinks commercial Web developers don't abuse the shit out
of this to put their hands up now. Even when conscientiously trying to do
the right thing, you can watch developers refactor across the view boundary into
the application with helpers.

Doing the right thing is sometimes harder than doing "whatever, however". Use
a proper templating system when you can. I realise circumstance wont always
permit it, and sometimes you wont have a choice. At least if you know how
it should be done, you'll appreciate what you should avoid doing in Razor,
no matter what good reason you think you have.

XSLT is fast, safe, and *enforces* a clean separation of concerns. Transforming
final application state into a view for the user-agent is a very smooth workflow.
The XML on the left-hand side, needs to transform into the HTML on the right-hand
side. It's pure functional, and so there's something of a conceptual hump to get
over, but it will do you no harm to learn a pure functional language.

Namespaces can also be a right life-saver with overlapping models, and just
keeping model concerns separate.

I realise there's lots of reasons why somebody wouldn't choose XSLT, and so
Razor is supported, and I'll drop in String Template as a priority. String Template
used to have terrible performance, but from comments on the recent version I
gather that may have changed, and I'm curious. Something like String Template
might be the sweet-spot between Razor and XSLT. Imperative, top-down, but safe.

It's worth noting quickly in passing, the match/template and apply-templates/select acting upon a document is similar to a reactive process.

### `HttpContext` is a very heavy dependency, avoid when possible

`WebBehaviour` which extends `ProcessBehaviour`, introduces the `WebContext`:-

	public abstract void Action(IEvent ev, WebContext context);

This wraps the `HttpContext` and a bunch of other types starting with `Http`.
This would seem unremarkable in a Web application, but comes with very serious
consequence. This is a very strong coupling to the ASP.NET environment, and
to a certain degree IIS.

If you need the web context, you need it, but only use a Web behaviour you
you actually must. Don't fall into the habit of using it unthinkingly and
you'll find your system much easier to test, and your behaviours much more
reusable.

I still need to push all non-http stuff out of `WebBehaviour` so that you
really only need it for request and response.

`WebActionBehaviour` in `Conclave.Web.CMS` is a good example of me not following
my own advice. It extends `WebActionBehaviour` to use the configurable condition
constraints it provides. It doesn't actually touch anything Web related. This
will be tidied up when I give a even coverage of the different common types
of constraints behaviour conditions are going to use.
  

* [Conclave.Web project folder](./Conclave.Web/)

<a name="Conclave.Web.CMS"></a>
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

* [Conclave.Web.CMS project folder](./Conclave.Web.CMS/)
