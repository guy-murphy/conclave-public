## `Conclave.Map`, Project Notes

*"It's all just circles and arrows"*, is really still the best explanation
I've come up with.

A good friend recently came to a realisation regarding topicmaps,
and surmised rather eloquently:-

> Woah. I just got it. I see how a topic map could encode an entire application,
abstracting away the resources that need resolving for a particular item viewed.

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
regular associations. I also feel no attention has been given to ensuring the
data-model works well on relational backing.

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
real understanding of the power of a generalised interface and the freedom
that was afforded when you let parts of the system just get on and do their
thing. Much later while working on an inference engine that I noted the 
similarities between the rules as they were modelled and the occurrence 
behaviours in Acumen, while idly wishing I could use behaviours everywhere, 
and then asked myself...*"Why can't I used behaviours everywhere?"*... 
having no good answer, I tried it. Turns out you can use behaviours everywhere. In
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
in performance is significant. So, wiki-text can reside in the data
field and come with the regular topic query.

This would not have been appropriate for Acumen where we had a `FileStore`
that was versioned and append-only. The revisions of content were especially
important for a written-copy heavy interest. I've begun reimplementing
the filestore in Conclave, but I've halted it as I'm not sure I want to redo
it as was... I'm thinking of making the `FileStore` intigrated as a
storage model with the topicmap. Like the `data` field, but in its own table,
append-only, and version... but still part of the topic query. Done that
way it would be an extension of the self/data model, slower, but not in
a completely different place in terms of performance, which the old
`FileStore` mechanism is. You're always going to be able to just
knock this off the table with your own references and behaviours,
the desire is to provide two robust and fast defaults for the broadest
range of use-cases. The "start here", philosophy.

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
stops being the norm and becomes more explicit.

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
the Acumen/Conclave ethos of a more ordered state, and simple patterns
being used repeatedly until they become familiar, known, and then the accepted way.

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

