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