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

Except where explicitly noted otherwise, all source code within this
repository has the following license applied:-

	Copyright 2013, Guy J. Murphy

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

	  http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.

