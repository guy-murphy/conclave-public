
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

