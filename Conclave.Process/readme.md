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

## `P:Conclave.Process.IEvent.Object`


#### Remarks
I felt compelled to put an escape hatch in here. It's wrong and I feel dirty, but I did it anyway.

## `T:Conclave.Process.IProcessBehaviour`
The base type for behaviours in Conclave. Behaviours are intended to be registered against a context such as  [Conclave.Process.ProcessContext](T-Conclave.Process.ProcessContext) using `ProcessContext.Register(behaviour)`.

#### Remarks
When events are fired against that context, each behaviour registered will apply it's condition to the  [Conclave.Process.IEvent](T-Conclave.Process.IEvent)  being fired. If this condition returns `true`,             then the context will apply the behaviours `Action` against             the event.

Care should be taken to ensure behaviours are well behaved. To this end the following contract is implied by use of `IProcessBehaviour`:-


##### Example

    context.Register(behaviours);
    context.Fire("set-up");
    context.Fire("process-request");	
    context.Fire("tear-down");
    context.Completed();
    context.Response.ContentType = "text/xml";
    context.Response.Write(context.ControlState.ToXml());


### `.Action(Conclave.Process.IEvent)`
Process an action for the provided  [Conclave.Process.IEvent](T-Conclave.Process.IEvent) .

* `ev`: The event to be processed. 

### `.Action(Conclave.Process.IEvent,Conclave.Process.ProcessContext)`
process and action for the provided  [Conclave.Process.IEvent](T-Conclave.Process.IEvent) with the  [Conclave.Process.ProcessContext](T-Conclave.Process.ProcessContext)  provided.

* `ev`: The event to process.
* `context`: The context to use.

### `.Condition(Conclave.Process.IEvent)`
The considtion that determines whether of not the behaviours action is valid to run.

* `ev`: The event to consider with the condition.

**returns:** 
`true` if the condition is met; otherwise,  returns  `false`.

### `.Message`
Gets the message that the behaviour will respond to.


**value:** A `string` value.

## `T:Conclave.Process.ProcessBehaviour`
A simple named behaviour with a default condition matching that name againts  [Conclave.Process.IEvent.Message](P-Conclave.Process.IEvent.Message) .


### `.#ctor(System.String)`
Creates a new instance of the behaviour.

* `message`: The name of the behaviour.

### `.Condition(Conclave.Process.IEvent)`
Determines if the event specifies the behaviour by name.

* `ev`: The event to consult.

**returns:** 
Returns true if the  [Conclave.Process.IEvent.Message](P-Conclave.Process.IEvent.Message) is the same as the  [Conclave.Process.ProcessBehaviour.Message](P-Conclave.Process.ProcessBehaviour.Message) 

## `P:Conclave.Process.User.UserCredentials.Password`
A password used in conjunction with a name for authentication. Whether or not this is a hash is implementation specific, while remembering that storing clear-text passwords in a database makes the Little Baby Jesus cry.


## `T:Conclave.Process.ViewStep`
Represents a step in a rendering view pipeline.

#### Remarks
A step can either have  [Conclave.Process.ViewStep.Content](P-Conclave.Process.ViewStep.Content)  or             a  [Conclave.Process.ViewStep.Model](P-Conclave.Process.ViewStep.Model) , but not both.

### `.#ctor(System.String,System.String,System.String)`
Creates a new instance of a step with the parameters provided.

* `name`: Human readable name of the step.
* `contentType`: The type of the steps content.
* `content`: The actual content of the step.

### `.#ctor(System.String,Conclave.IData)`
Creates a new instance of a step with the parameters provided.

* `name`: The human readable name of the step.
* `model`: The actual model of the step.
### `.Name`
The human readable name of the step.

### `.ContentType`
The content type of the  [Conclave.Process.ViewStep.Content](P-Conclave.Process.ViewStep.Content) if there is any.

### `.Content`
The content if any of the step.

### `.Model`
The model if any of the step.

### `.HasContent`
Determines whether or not the step has any content.

### `.HasModel`
Determines whether or not the step has a model.

