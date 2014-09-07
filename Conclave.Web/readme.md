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
  
## `T:Conclave.Web.Behaviour.AuthenticateBehaviour`
A behaviour concerned with authenticating the user.

#### Remarks
This behaviour should be a singleton, and maintain no instance fields. Interaction with the authenticated user is via the [System.Security.Principal.GenericPrincipal](T-System.Security.Principal.GenericPrincipal)  object hanging off [Conclave.Web.WebContext.User](P-Conclave.Web.WebContext.User) 

### `.#ctor(System.String)`
Creates an instance of the authentication behaviour that will respond to the message provided.

* `message`: The message to which the behaviour will respond.

## `T:Conclave.Web.Behaviour.RenderBehaviour`
Controls the writting of results from the last view step in the view pipeline to the response stream.

#### Remarks
This behaviour is not responsible for producing or transforming views, merely the writting of results. Consult  [Conclave.Web.Behaviour.ProcessViewsBehaviour](T-Conclave.Web.Behaviour.ProcessViewsBehaviour) for the actually processing of views.



### `.#ctor(System.String)`
Creates a new instance of a render behaviour.

* `name`: The name of the render behaviour.
Throws an exception is there is no view step to actually render.
### `.Action(Conclave.Process.IEvent,Conclave.Web.WebContext)`
## `T:Conclave.Web.Behaviour.ViewStateBehaviour`
Constructs the initial view state of the reuqest as a  [Conclave.Process.ViewStep](T-Conclave.Process.ViewStep)  composed of the current  [Conclave.Process.ProcessContext.ControlState](P-Conclave.Process.ProcessContext.ControlState) .

#### Remarks
This is basically a filtering of the  [Conclave.Process.ProcessContext.ControlState](P-Conclave.Process.ProcessContext.ControlState)  into             the model called the view state, that is going to be rendered. Any item in the control state with a key             starting with the underscore character '_' is regarded as protected, and will             not be copied forward to the view state.

If one wished to present a model for render by different means, or wanted to change how the filtering was done, this is the behaviour you would swap out for an alternate implementation.



## `T:Conclave.Web.Behaviour.View.JsonViewBehaviour`
Serialise the model of the last view step to XML.


## `T:Conclave.Web.Behaviour.View.RazorViewBehaviour`
A web behaviour that resolves razor templates to generate views.

#### Remarks
Razor isn't getting a lot of attention in Conclave initially, at some point I'll pay it some attention, but it's really not a priority as personally I'm not a big fan.

Razor is the fast food of templating. It's really tasting and super-saturated with utility, and it's bad for you. When rendering a view you really shouldn't be able to yield side-effects, and you shouldn't be able to consider anything other than the view you're rendering. In Razor you can do anything you want. And you will. Especially when people aren't looking.

Worse, you'll start architecting clever helpers, and mappings, and... you'll start refactoring, and all your templates will become enmeshed in one glorious front-end monolith.

Razor. Just say "no"... Okay, I'm over-egging it a bit.

Joking aside, I get why Razor is so popular. It's simple, bendy, easy for .NET devs to dive into, and you can brute force yourself out of any situation. It does however in my view encourage poor practice and blurs an important application layer so the middle and front of the application risk becoming quickly enmeshed.

Conclave favours XML/XSL, I understand why you might not, hence  [Conclave.Web.Behaviour.View.RazorViewBehaviour](T-Conclave.Web.Behaviour.View.RazorViewBehaviour) .



### `.#ctor(System.String)`


* `message`: 
#### Remarks
This constructor defaults the content type to `text/html`.

### `.#ctor(System.String,System.String)`


* `message`: 
* `contentType`: 

## `T:Conclave.Web.Behaviour.View.XmlViewBehaviour`
Serialise the model of the last view step to XML.

## `P:Conclave.Web.UrlInfo.Regex`
The regular expression being used to break URLs down into their parts.

#### Remarks
If a regular expression isn't provided via the contructor then the default  [Conclave.Web.UrlInfo.DefaultRegex](F-Conclave.Web.UrlInfo.DefaultRegex)  is used.
## `P:Conclave.Web.UrlInfo.Match`
The matches produced by matching the [Conclave.Web.UrlInfo.Regex](P-Conclave.Web.UrlInfo.Regex)  against the  [Conclave.Web.UrlInfo.Url](P-Conclave.Web.UrlInfo.Url) .

## `P:Conclave.Web.UrlInfo.Url`
The url being processed.

## `P:Conclave.Web.UrlInfo.Protocol`
The protocol specified by the  [Conclave.Web.UrlInfo.Url](P-Conclave.Web.UrlInfo.Url) 

## `P:Conclave.Web.UrlInfo.Domain`
The domain specified by the  [Conclave.Web.UrlInfo.Url](P-Conclave.Web.UrlInfo.Url) 

## `P:Conclave.Web.UrlInfo.FullPath`
The full path specified by the  [Conclave.Web.UrlInfo.Url](P-Conclave.Web.UrlInfo.Url) 

## `P:Conclave.Web.UrlInfo.AppPath`
The application path specified by the  [Conclave.Web.UrlInfo.Url](P-Conclave.Web.UrlInfo.Url) 

## `P:Conclave.Web.UrlInfo.AppUrl`
The URL of the current application.

## `P:Conclave.Web.UrlInfo.File`
The file specified by the  [Conclave.Web.UrlInfo.Url](P-Conclave.Web.UrlInfo.Url) 

## `P:Conclave.Web.UrlInfo.Extension`
The extension specified by the  [Conclave.Web.UrlInfo.Url](P-Conclave.Web.UrlInfo.Url) 

## `P:Conclave.Web.UrlInfo.Tail`
The tail specified by the  [Conclave.Web.UrlInfo.Url](P-Conclave.Web.UrlInfo.Url) 

## `P:Conclave.Web.UrlInfo.QueryString`
The query string specified by the  [Conclave.Web.UrlInfo.Url](P-Conclave.Web.UrlInfo.Url) 

## `P:Conclave.Web.UrlInfo.Query`
A name / value dictionary as the propduct of parsing the  [Conclave.Web.UrlInfo.QueryString](P-Conclave.Web.UrlInfo.QueryString) 


## `T:Conclave.Web.WebContext`
Extends  [Conclave.Process.ProcessContext](T-Conclave.Process.ProcessContext)  with Web specific             information about an individual request being processed.

#### Remarks
The context object is threaded through the whole stack and provides a controled pattern and workflow of state, along with access to resources and services external to the application. Everything hangs off the context.

### `.#ctor(System.Web.HttpContext)`
Instantiates a new context object purposed for Web applications.

* `underlyingContext`: The underlying http context to wrap.

## `T:Conclave.Web.DefaultHandler`
The default  [System.Web.IHttpHandler](T-System.Web.IHttpHandler)  for Conclave.

