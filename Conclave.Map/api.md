## `P:Conclave.Map.Model.Association.Blank`
Represents an empty association item as an alternative to a null value;


## `T:Conclave.Map.Model.Association.Builder`
The builder for  [Conclave.Map.Model.Association](T-Conclave.Map.Model.Association) , providing a mutable equivalent             of the concrete, immutable model.

## `P:Conclave.Map.Model.Metadata.Blank`
Represents an empty metadata item as an alternative to a null value.


## `T:Conclave.Map.Model.Metadata.Builder`
The builder for  [Conclave.Map.Model.Metadata](T-Conclave.Map.Model.Metadata) , providing a mutable equivalent             of the concrete, immutable model.


### `.op_Implicit(Conclave.Map.Model.Metadata.Builder)~Conclave.Map.Model.Metadata`
Provides for an implicit cast from a  [Conclave.Map.Model.Metadata.Builder](T-Conclave.Map.Model.Metadata.Builder)  object             to a  [Conclave.Map.Model.Metadata](T-Conclave.Map.Model.Metadata)  object.

* `builder`: The  [Conclave.Map.Model.Metadata.Builder](T-Conclave.Map.Model.Metadata.Builder)  that is to be cast.

**returns:** 
Returns a  [Conclave.Map.Model.Metadata](T-Conclave.Map.Model.Metadata)  object with the same member values             as the builder being cast from.


## `T:Conclave.Map.Model.Topic`
Model for expression of a topic withing a topic map.


### `.Clone`
Clones the topic, copying all values to the new topic but maintaining no references to the topic topic.


**returns:** 
Returns a new topic, copied from this one.

### `.Associations`
The Associations that belong to this topic.

### `.Occurrences`
The occurrences that belong to this topic.


## `T:Conclave.Map.Store.DmozReader`
Reads DMoz structure expressed as RDF into a provided TopicMap.

#### Remarks
This is a quick and dirty implementation for the purposes of bulk loading a topic store. It's not fit for any genuine purpose, but it might be a useful starting point. There's naff all error checking, and it's not reading all aspects of the RDF, it really is just bulk loading something vaguely topicmap-like.

### `:Conclave.Map.Store.ITopicStore.GetOccurrence(System.String,System.String,System.String,System.String,System.String)`
Returns any occurrence that matches the provided parameters; or, returns a blank occurrence.

* `parent`: The identity of the parent Topic to which the occurrence belongs.
* `scope`: The scope of the occurrence.
* `role`: The role that the occurrence performs.
* `behaviour`: The behaviour cited to resolve the occurrence.
* `reference`: The reference to the occurrence backing data to be resolved by the cited behaviour.

**returns:** 
Returns either the occurrence matching the provided parameters; or, returns a blank occurrence.

#### Remarks
It is API breaking for this method to return a null reference.

### `:Conclave.Map.Store.ITopicStore.GetAssociationIds`
Gets all the association ids in the store.


**returns:** 
Returns an an enumerable of all the association ids.

#### Remarks
I'm really not sure this is a smart idea. It's a disaster just waiting to happen.
TODO: Breaking. Remove.
### `:Conclave.Map.Store.ITopicStore.AssociationExists(System.String)`
Checks whether the specified association exists in the store.

* `id`: The identity of the association to check for.

**returns:** 
Returns true if the association exists; otherwise returns false.


### `:Conclave.Map.Store.ITopicStore.TopicExists(System.String)`
Checks whether the specified topic exists in the store.

* `topicId`: The identity of the topic to check for.

**returns:** 
Returns true if the topic exists; otherwise returns false.


### `:Conclave.Map.Store.ITopicStore.GetTopicIds`
Gets all the topic ids in the store.


**returns:** 
Returns an an enumerable of all the topic ids.

#### Remarks
I'm really not sure this is a smart idea. It's a disaster just waiting to happen.
TODO: Breaking. Remove.
### `:Conclave.Map.Store.ITopicStore.CreateTopic(System.String)`
Creates a topic with the specified Id.

* `id`: The identity of the topic to create.

**returns:** 
Returns the newly created  [Conclave.Map.Model.Topic](T-Conclave.Map.Model.Topic) 

It is an error to attempt to create a topic that already exists.
### `:Conclave.Map.Store.ITopicStore.RemoveTopic(System.String)`
Removes the specified topic from the store.

* `topicId`: The identity of the topic to remove.

### `:Conclave.Map.Store.ITopicStore.AddTopic(Conclave.Map.Model.Topic)`
Adds a topic to the store.

* `topic`: The topic to add to the store.

### `:Conclave.Map.Store.ITopicStore.GetTopic(System.String,System.String)`
Gets a topic of the specified identity, with its members filtered by the scope provided.

* `id`: The identity of the topic to get.
* `scope`: The scope to filter the topics members by. Spcifying the scope as null indicates no filtering will take place.

**returns:** 
Returns the topic found with the identity provided, or returns  [Conclave.Map.Model.Topic.Blank](P-Conclave.Map.Model.Topic.Blank) . This method             should not return null.


### `:Conclave.Map.Store.SqlTopicStore.GetOccurrence(System.String,System.String,System.String,System.String,System.String)`
Returns any occurrence that matches the provided parameters; or, returns a blank occurrence.

* `parent`: The identity of the parent Topic to which the occurrence belongs.
* `scope`: The scope of the occurrence.
* `role`: The role that the occurrence performs.
* `behaviour`: The behaviour cited to resolve the occurrence.
* `reference`: The reference to the occurrence backing data to be resolved by the cited behaviour.

**returns:** 
Returns either the occurrence matching the provided parameters; or, returns a blank occurrence.

#### Remarks
It is API breaking for this method to return a null reference.

### `:Conclave.Map.Store.SqlTopicStore.GetAssociationIds`
Gets all the association ids in the store.


**returns:** 
Returns an an enumeration of all the association ids.


### `:Conclave.Map.Store.SqlTopicStore.AssociationExists(System.String)`
Checks whether the specified association exists in the store.

* `id`: The identity of the association to check for.

**returns:** 
Returns true if the association exists; otherwise returns false.


### `:Conclave.Map.Store.SqlTopicStore.TopicExists(System.String)`
Checks whether the specified topic exists in the store.

* `topicId`: The identity of the topic to check for.

**returns:** 
Returns true if the topic exists; otherwise returns false.


### `:Conclave.Map.Store.SqlTopicStore.GetTopicIds`
Gets all the topic ids in the store.


**returns:** 
Returns an an enumerable of all the topic ids.


### `:Conclave.Map.Store.SqlTopicStore.CreateTopic(System.String)`
Creates a topic with the specified Id.

* `id`: The identity of the topic to create.

**returns:** 
Returns the newly created  [Conclave.Map.Model.Topic](T-Conclave.Map.Model.Topic) 

It is an error to attempt to create a topic that already exists.#### Remarks
This is a leaky abstraction and needs to be removed. It relies on knowledge of the relational model of the sql stores and that the topic table is a discrete table of topic ids alone.
TODO: Breaking. Remove, or add Topic return type for the topic created.
### `:Conclave.Map.Store.SqlTopicStore.RemoveTopic(System.String)`
Removes the specified topic from the store.

* `topicId`: The identity of the topic to remove.

### `:Conclave.Map.Store.SqlTopicStore.AddTopic(Conclave.Map.Model.Topic)`
Adds a topic to the store.

* `topic`: The topic to add to the store.
