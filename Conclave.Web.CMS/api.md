
## `T:Conclave.Web.CMS.Behaviour.AssociationBehaviour`
A behaviour concerned with requests that manipulate a topic map associations.


### `.Edit(Conclave.Process.IEvent,Conclave.Web.WebContext)`
Edits an association. Either its core properties, or its metadata.

* `ev`: The  [Conclave.Process.IEvent](T-Conclave.Process.IEvent)  being processed.
* `context`: The  [Conclave.Web.WebContext](T-Conclave.Web.WebContext)  for the currente request.
#### Remarks
Dispatches the messages: topicmap::update-metadata, topicmap::update-association topicmap::get-topic, topicmap::resolve-navigation.

## `T:Conclave.Web.CMS.Behaviour.MetadataBehaviour`
A behaviour responsible for mediating requests concerned with topic metadata.


## `T:Conclave.Web.CMS.Behaviour.Topicmap.CreateAssociationBehaviour`
A behaviour concerned with creating new associations.

#### Remarks
This is a second tier behaviour and consideres the current event, not the context.

## `T:Conclave.Web.CMS.Behaviour.Topicmap.CreateTopicBehaviour`
A behaviour concerned with creating new topics. This is a second tier behaviour and consideres the current event, not the context.


## `T:Conclave.Web.CMS.Behaviour.Topicmap.ResolveMarkdownBehaviour`
A behaviour for transforming Markdown notation into well formed HTML.


## `T:Conclave.Web.CMS.Behaviour.TopicBehaviour`
A behaviour responsible for mediating requests concerned with topics.


## `T:Conclave.Web.CMS.Behaviour.Topicmap.UpdateOccurrenceBehaviour`
A behaviour resoponsible for updating Occurrences in a backing store.

