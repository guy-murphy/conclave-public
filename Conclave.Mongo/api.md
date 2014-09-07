
## `T:Conclave.Map.Store.MongoTopicStore`
An  [Conclave.Map.Store.ITopicStore](T-Conclave.Map.Store.ITopicStore)  for MongoDB.

#### Remarks
The Mongo topic store behaves slightly differently that topic stores implemented against relational databases. This difference is largely around adding metadata, occurrences, and associations by themselves directly when there may be no parent topic in the store. See implementation notes.

You can only add child members to parents that exist in the Mongo store.


