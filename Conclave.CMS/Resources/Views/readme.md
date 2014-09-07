# View Templates

There is an approach in specifying view from controller common in many MVC-like frameworks, and that is for
the controller to be able to specify the view that should be used for render.  So one might have `entity/edit`
presenting a model that a `entity/edit` template renders. So far, so obvious. Then we have `entity/update`
controller that actually updates the entity. Now we decide we want to continuing editing this class of
entity after updating and want the same view as `entity/edit` for `entity/edit`. We can copy and paste
from the `entity/edit` template to the `entity/update` template, but not only are we repeating ourselves
but it's a right pain in the arse. So instead we specify in the `entity/update` controller that the `entity/edit`
template should be used for view render.

And herein lies the problem. In my view this problem is present in most systems that take this approach
but perhaps to a degree that isn't problematic enough to really worry about. In Conclave however because
of its goals, this is more of a problem.

## Leaky Abstraction

It is desirable for each part of a system to know as little as necessary about any other part of the system.
Among other things it makes the system more sympathetic to change, and produces less unforeseen side-effects,
in general terms making the world a better place for little children and kittens alike.

In a layered architecture commonly knowledge about components in the stack flows in one direction
with higher level components knowing about lower level components, but lower level components
knowing nothing about higher level components built atop and with them.

Likewise in process flow, commonly processes might know about what went before them, but it is often
desirable that they not know what comes after them.

We layer our abstractions. Experience tells us it makes systems easier to build, manage and change over time
with greater opportunity for reuse of work.

When one part of the system relies upon knowledge (that the programmer implicitly has) about what
lies on the other side of an interface that it's not really supposed to know, we call that a leaky abstraction.
The abstraction is in place, but the programmer is breaching it with what they know to lie on the other side.

## The Problem

When the `entity/update` controller specifies to render it as if it were the `entity/edit` controller
it expresses knowledge of two things. That there is an `entity/edit` controller, and how it is going to be
rendered.

Now this isn't necessarily the worst thing in the world, but it's more knowledge than is necessary, and depending
on your point of view, assumption that this component of code should not be making.

In Conclave the closest thing we have to controllers is *behaviours*, which you certainly can use
exactly as controllers. The thing is, in Conclave we say that behaviours should really make no
assumptions about what comes after them, or indeed alongside of them where at all possible. A behaviour
to update an entity should do just that and really not concern itself with anything else at all. In Conclave
behaviours observe and act. Whatever is responsible for rendering some model should make the decisions
about what and how to render it.

So we can certainly do `context.Params["action"] = "edit"` from the one of perhaps many behaviours
acting upon `entity/update`. And we know that this will have the desired result in terms of render
but we just sacrificed architectural integrity at one point of the system, to accommodate a problem that lies
in a completely different part of the system... remember, we didn't want to manage two templates.

## The Solution

Now as it happens there's quite a few solutions in terms of not repeating ourselves in view templates if
we just attempt to solve them closer to where they occur.

We can have symlinks on the files system. These aren't so popular in the Windows world so we can just have
a stub of a template file for `entity/update` that imports the `entity/edit` template. In this way the "controller",
or whatever composition of components performs this role, doesn't need to know anything about how it's rendered,
and it doesn't need to know anything about how its siblings render either. Once we get to the templates
we resolve the problem with knowledge that is local to the problem, and without insinuating that
knowledge into parts of the system where it really doesn't belong.

## Other Solutions

Many will already have noted a perhaps better approach to the specific problem outlines above.
We can have two behaviours acting upon `entity/edit` with one further acting upon `context.IsGet` and one
acting upon `contect.IsPost`, and both utilising the same template. This is the approach taken with
`Conclave.CMS` as a matter of preference. In this regard the above original example would be contrived
if it weren't something that people want to be able to actually do.

One of the goals of Conclave is to avoid render-as, redirect, process-as etc., and these approaches are often
expedient hacks that mask problems and often become the source of tricky bugs. Each component is
concerned with one thing and doesn't stick its nose into other components business.