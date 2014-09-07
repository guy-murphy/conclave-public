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
## `T:Conclave.Collections.ConcurrentDataCollection`1`
An implementation of  [Conclave.Collections.IDataCollection`1](T-Conclave.Collections.IDataCollection`1)  that             is safe for concurrent use.

`T`: The type of the elements in the collection.

## `T:Conclave.Collections.IDataCollection`1`
Represents a collection of  [Conclave.IData](T-Conclave.IData)   objects,              that can be accessed by index.

`T`: The type of elements in the list.

## `T:Conclave.IData`
Base interface for data models in Conclave.

#### Remarks
While this represents a point of extensibility for data models in Conclave this is pretty much limited to simple serialisation. `IData` had more relevance in Acumen pre-extension methods, now it's primary untility is simply to flag what is data-model and to be able to contrain based upon that.

This approach to model serialisation is simple, but also fast, uncluttered, and explicit. The writers used obviously are  often used by a whole object graph being written out, and is part of a broader application concern. The writer should not be used in any way that yields side-effects beyond the serialisation at hand.  [Conclave.IData.ToXml-System.Xml.XmlWriter](M-Conclave.IData.ToXml-System.Xml.XmlWriter)  and  [Conclave.IData.ToJson-Newtonsoft.Json.JsonWriter](M-Conclave.IData.ToJson-Newtonsoft.Json.JsonWriter) need to be fast and reliable. 

Conclave favours an application where XML (or JSON for apps with data clients) is its primary external interface, which is transformed (normally with XSL) into a view stuitable for a particular class of user-agent. Other approaches may have a different regard for a data-model and `IData` serves as a point of extension in those cases perhaps.

Extension methods are provided in  [Conclave.DataEx](T-Conclave.DataEx)  to provide `IData.ToXml()` and `IData.ToJson()`.



### `.ToXml(System.Xml.XmlWriter)`
Produces an Xml representation of the model.

* `writer`: The writer to used to write the Xml to. 

### `.ToJson(Newtonsoft.Json.JsonWriter)`
Produces a Json respresentation of the model.

* `writer`: The writer to use for producing JSON.

### `:Conclave.Collections.ConcurrentDataCollection`1.#ctor`
Instantiates a new empty collection.


### `:Conclave.Collections.ConcurrentDataCollection`1.#ctor(System.Collections.Generic.IEnumerable{`0})`
Instanciates a new data collection with elements copied from the provided collection.

* `collection`: The collection whose elements are copied into the new data collection.

### `:Conclave.Collections.ConcurrentDataCollection`1.ContentToXml(System.Xml.XmlWriter)`
Produces an XML representation of the collections elements  to a provided writer.

* `writer`: The  [System.Xml.XmlWriter](T-System.Xml.XmlWriter)  the representation is written to.

### `:Conclave.Collections.ConcurrentDataCollection`1.ContextToJson(Newtonsoft.Json.JsonWriter)`
Produces an JSON representation of the collection to a provided writer.

* `writer`: The  [Newtonsoft.Json.JsonWriter](T-Newtonsoft.Json.JsonWriter)  the representation is written to.

### `:Conclave.Collections.ConcurrentDataCollection`1.ToXml(System.Xml.XmlWriter)`
Produces an XML representation of the collection  to a provided writer.

* `writer`: The  [System.Xml.XmlWriter](T-System.Xml.XmlWriter)  the representation is written to.

### `:Conclave.Collections.ConcurrentDataCollection`1.ToJson(Newtonsoft.Json.JsonWriter)`
Produces an JSON representation of the collection  to a provided writer.

* `writer`: The  [Newtonsoft.Json.JsonWriter](T-Newtonsoft.Json.JsonWriter)  the representation is written to.

## `T:Conclave.Collections.IDataDictionary`1`
Represents a generic dictionary that implements  [Conclave.IData](T-Conclave.IData) , where the keys are strings.

`T`: The type of the element values.

### `.Import(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{System.String,`0}})`
Import the provided key/value pairs into the dictionary.

* `other`: The key/value pairs to copy.

**returns:** 
This dictionary.


## `T:Conclave.Collections.DataDictionary`1`
A collection of key/value pairs, where the key is a string.

`TValue`: The type of the element values.

### `.#ctor`
Instantiates a new empty instance of the dictionary.


### `.#ctor(System.Collections.Generic.IDictionary{System.String,`0})`
instantiates a new dictionary with the elements copied over from the dictionary provided.

* `other`: The dictionary to copy elements from.

### `.#ctor(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{System.String,`0}})`
instantiates a new dictionary with the elements copied from iterating over the key/value pairs provided.

* `other`: The key/value pairs to copy.

## `T:Conclave.Collections.DataCollection`1`
An implementation of  [Conclave.Collections.IDataCollection`1](T-Conclave.Collections.IDataCollection`1)  as a simple  [System.Collections.Generic.List`1](T-System.Collections.Generic.List`1) . 

`T`: The type of elements in the list.

### `.#ctor`
Instanciates a new, empty data collection.


### `.#ctor(System.Collections.Generic.IEnumerable{`0})`
Instanciates a new data collection with elements copied from the provided collection.

* `collection`: The collection whose elements are copied into the new data collection.

### `.ContentToXml(System.Xml.XmlWriter)`
Produces an XML representation of the dictionaries elements, to a provided writer.

* `writer`: The  [System.Xml.XmlWriter](T-System.Xml.XmlWriter)  the representation is written to.

### `.ContextToJson(Newtonsoft.Json.JsonWriter)`
Produces an JSON representation of the dictionaries elements, to a provided writer.

* `writer`: The  [Newtonsoft.Json.JsonWriter](T-Newtonsoft.Json.JsonWriter)  the representation is written to.

### `.ToXml(System.Xml.XmlWriter)`
Produces an XML representation of the dictionaries  to a provided writer.

* `writer`: The  [System.Xml.XmlWriter](T-System.Xml.XmlWriter)  the representation is written to.

### `.ToJson(Newtonsoft.Json.JsonWriter)`
Produces an JSON representation of the dictionaries  to a provided writer.

* `writer`: The  [Newtonsoft.Json.JsonWriter](T-Newtonsoft.Json.JsonWriter)  the representation is written to.

## `T:Conclave.Collections.DataModel`
A  [System.Dynamic.DynamicObject](T-System.Dynamic.DynamicObject)  implementing             an  [Conclave.Collections.IDataDictionary`1](T-Conclave.Collections.IDataDictionary`1)  .

#### Remarks
This class is intended to help with exposing models to Razor templates, as it allows ad hoc properties to be used as dictionary keys, `model.UserDetails.Name` rather than `model["UserDetails"].Name`

The initial idea was for  the `ControlState` to be one of these. When I start playing about with Razor a bit more I'll test it to see if there's any consequences.



## `T:Conclave.Extensions.DictionaryEx`
Utility extension methods provided for dictionaries.


### `.Import``2(System.Collections.Generic.IDictionary{``0,``1},System.Collections.Generic.IDictionary{``0,``1})`
Copies the elements from one dictionary to another.

`TKey`: The type of the dictionary keys.
`TValue`: The type of the dictionary values.
* `self`: The dictionary being acted on.
* `other`: The dictionary being copied from.

### `.ContentToXml(System.Collections.Generic.IDictionary{System.String,Conclave.IData},System.Xml.XmlWriter)`
Produces an XML representation of the elements of a dictionary.

* `self`: The dictionary being acted upon.
* `writer`: The  [System.Xml.XmlWriter](T-System.Xml.XmlWriter)  the representation             is written to.

## `T:Conclave.Extensions.ListEx`
Utility extension methods provided for lists.

#### Remarks
Just some methods to allow a list to be treated as a stack. If a stack is being used as a context in tree processing, sometimes being able to peek at more than the last element, or also treat the stack like a list is useful.

### `.Push``1(System.Collections.Generic.List{``0},``0)`
Pushes an elelent onto the list as if it were a stack.

`T`: The type of the list elements.
* `self`: The list being acted on.
* `item`: The element being pushed onto the list.

**returns:** 
The list being acted on.


### `.Pop``1(System.Collections.Generic.List{``0})`
Pops an element from the list as if it were a stack.

`T`: The type of the list elements.
* `self`: The list being acted on.

**returns:** 
The element that was poped.


### `.Peek``1(System.Collections.Generic.List{``0},System.Int32)`
Provides an index of the list in reverse order, with `list.Peek(0)` considering the last element of the list, and `list.Peek(1)` being the penultimate element of the list. No bounds checking is provided.

`T`: The type of the list elements.
* `self`: The list being acted on.
* `i`: The index of the peek.

**returns:** 
The element found at the index.


### `.Peek``1(System.Collections.Generic.List{``0})`
Takes a look at the last element of a list without removing it, as if it were a stack.

`T`: The type of the list elements.
* `self`: The list being acted on.

**returns:** 
The last element of the list.


## `T:Conclave.Extensions.StringBuilderEx`
Some utility extension methods provided for string builders.


### `.Filter(System.Text.StringBuilder,System.Predicate{System.Char})`
Filters a `StringBuilder`, removing any elements that the provided predicate returns true for.

* `self`: The string builder being acted upon.
* `test`: The predicate to test each element for removal.

**returns:** 
The string builder being acted upon.


### `.RemoveNonNumeric(System.Text.StringBuilder)`
Removes all non-numeric characters from the string builder.

* `self`: The string builder being acted upon.

**returns:** 
The string builder being acted upon.


### `.RemoveNonAlpha(System.Text.StringBuilder)`
Removes all the non-alphabetic characters from the string builder.

* `self`: The string builder being acted upon.

**returns:** 
The string builder being acted upon.


### `.RemoveNonAlphaNumeric(System.Text.StringBuilder)`
Removes all non-alphanumeric characters from the string builder.

* `self`: The string builder being acted upon.

**returns:** 
The string builder being acted upon.


### `.RemoveWhitespace(System.Text.StringBuilder)`
Removes all whitespace from the string builder.

* `self`: The string builder being acted upon.

**returns:** 
The string builder being acted upon.


### `.TrimLeftBy(System.Text.StringBuilder,System.Int32)`
Removes a specified number of characters from the left-side of the string builder.

* `self`: The string builder being acted upon.
* `amount`: 

**returns:** 
The string builder being acted upon.


### `.TrimRightBy(System.Text.StringBuilder,System.Int32)`
Removes a specified number of characters from the right-side of the string builder.

* `self`: The string builder being acted upon.
* `amount`: 

**returns:** 
The string builder being acted upon.


### `.TrimEndsBy(System.Text.StringBuilder,System.Int32)`
Removes a specified number of characters from each end of the string builder.

* `self`: The string builder being acted upon.
* `amount`: 

**returns:** 
The string builder being acted upon.


### `:Conclave.Extensions.StringEx.HasValue(System.String)`
Determines if the string is not null and has a length greater than zero.

* `self`: The subject of extension.

**returns:** 
Returns true if the string has a values;             otherwise returns false.


### `:Conclave.Extensions.StringEx.AssertHasValue(System.String,System.String)`
Checks if a string has a value and if not throws an  [System.ArgumentNullException](T-System.ArgumentNullException) .

* `self`: The subject of extension.
* `message`: The message to use as part of the exception.
[Conclave.Extensions.StringEx.HasValue-System.String](M-Conclave.Extensions.StringEx.HasValue-System.String)
### `:Conclave.Extensions.StringEx.Prepend(System.String,System.Int32,System.Char)`
Places the 

* `self`: 
* `number`: 
* `character`: 

**returns:** 



### `:Conclave.Extensions.StringEx.IsXmlName(System.String)`
Determines if a string is a valid XML tag name.

* `self`: The subject of extension.

**returns:** 
Returns true if the string is a valid XML name;             otherwise, returns false.


## `T:Conclave.DataEx`
Extension methods for  [Conclave.IData](T-Conclave.IData) largely concerned with supporting both `.ToXml(...)` and `.ToJson(...)`


### `.ToXml(Conclave.IData)`
Generates an XML representation of the specified  [Conclave.IData](T-Conclave.IData)  object.

* `self`: The data model to produce XML for.

**returns:** 
Returns the XML representation as a `string`.

#### Remarks
This is implemented by creating a `StringWriter` and calling `.ToXml(IData, StringWriter)`

### `.ToXml(Conclave.IData,System.IO.TextWriter)`


* `self`: 
* `writer`: 

## `T:Conclave.IValidates`
An interface for a class that provides checks to determine is its data is valid.

#### Remarks
This was originally part of `IData`, but as that model moved toward an immutable one, the emphasis of validation was lessened and moved to be the responsibility of the model constructor. Given an immutable model it simply shoulnd't be instanciated in an invalid state.
### `.IsValid`
Indicates whether or not a model is in a valid state.


**value:** `true` is the model is in a valid state; otherwise, `false`.

## `T:Conclave.TextData`
An implementation of  [Conclave.IData](T-Conclave.IData)  that             represents a simple text node within a model.


### `.op_Implicit(System.String)~Conclave.TextData`
Implicitly casts a string of text into a `TextData` object, by instantiating a `TextData` object from the string.

* `text`: The string of text to be cast.

**returns:** 
Returns the `TextData` object created.


### `.op_Implicit(Conclave.TextData)~System.String`
Implicitly casts a `TextData` object into a string.

* `text`: The `TextData` object to cast.

**returns:** 
Returns the string value of the `TextData` object.


### `.#ctor(System.String)`
Instantiates a new `TextData` object with the value of the text provided.

* `text`: The text to initialise from.

### `.#ctor(Conclave.TextData)`
Instantiates a new `TextData` object as a copy of the one provided.

* `text`: The `TextData` to copy.

### `.Clone`
Creates a new instance as a copy of the original.


**returns:** 
A copy as a `TextData` object.

### `.Value`
The string value of the text data.

