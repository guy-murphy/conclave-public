
## `T:Conclave.Data.Resolver.FileResolver`
A base class used to resolve filesystem like resources.

This class is not threadsafe, especially with regard to  [Conclave.Data.Resolver.FileResolver.RelativePath](P-Conclave.Data.Resolver.FileResolver.RelativePath)  and the changing             location of the underlying resource being pointed to.
## `T:Conclave.Data.Resolver.IResolver`



### `.Create`
Creates the resource if it doesn't exist.


### `.Remove`
Removes the resource.

### `.ApplicationPath`
The path of the application.

### `.RootPath`
The root of the resource.

### `.RelativePath`
The relative path to the resource.

### `.FullPath`
The full absolute path to the resource.

### `.Exists`
Determines if the resource exists.


### `:Conclave.Data.Resolver.FileResolver.Create`
### `:Conclave.Data.Resolver.FileResolver.Remove`## `P:Conclave.Data.Resolver.FileResolver.ApplicationPath`## `P:Conclave.Data.Resolver.FileResolver.RootPath`## `P:Conclave.Data.Resolver.FileResolver.RelativePath`## `P:Conclave.Data.Resolver.FileResolver.FullPath`## `P:Conclave.Data.Resolver.FileResolver.Exists`
## `T:Conclave.Data.Resolver.TemplateFileResolver`
Provides all the functionality of a  [Conclave.Data.Resolver.FileResolver](T-Conclave.Data.Resolver.FileResolver) but in addition will help find suitable templates within a directory structure.


## `T:Conclave.Data.Store.IStore`
Base interface for a backing store.


### `:Conclave.Data.Store.SqlStore.Exec(System.String)`
Executes a SQL statement against the server.

* `sql`: The SQL statement to be executed.

**returns:** 
Returns true is the statement executes successfully and is commited;             otherwise returns false, indicating the transaction has been             rolled back.

#### Remarks
This method should be used for updates and inserts. It is not suitable for queries.

### `:Conclave.Data.Store.SqlStore.Exec(System.String,System.Data.IDbDataParameter[])`
Executes a parameterised SQL statement against the server with the specified parameters.

* `sql`: The SQL statement to be executed.
* `parameters`: The parameters for the SQL statement.

**returns:** 
Returns true is the statement executes successfully and is commited;             otherwise returns false, indicating the transaction has been             rolled back.

#### Remarks
This method should be used for updates and inserts. It is not suitable for queries.

### `:Conclave.Data.Store.SqlStore.Scaler(System.String)`
Executes a scaler SQL query against the server and returns the result.

* `sql`: The SQL statement to be executed against the server.

**returns:** 
The resulting object from the query.


### `:Conclave.Data.Store.SqlStore.Query(System.String)`
Executes a SQL query against the server and returns a  [System.Data.DataSet](T-System.Data.DataSet) or the results.

* `sql`: The SQL to be executed against the server.

**returns:** 
The  [System.Data.DataSet](T-System.Data.DataSet)  of the query results.


### `:Conclave.Data.Store.SqlStore.Query(System.String,System.Data.IDbDataParameter[])`
Executes a parameterised SQL statement against the server with the specified parameters.

* `sql`: The SQL statement to be executed.
* `parameters`: The parameters for the SQL statement.

**returns:** 
The  [System.Data.DataSet](T-System.Data.DataSet)  of the query results.


### `:Conclave.Data.Store.SqlStore.Fill(System.String,System.Data.DataSet)`
Executes a SQL statement against the server and fillsa provided  [System.Data.DataSet](T-System.Data.DataSet)  with the results.

* `sql`: The SQL statement to execute against the server.
* `data`: The  [System.Data.DataSet](T-System.Data.DataSet)  to be filled with the results.

### `:Conclave.Data.Store.SqlStore.Fill(System.String,System.Data.DataSet,System.Data.IDbDataParameter[])`
Executes a parameterised SQL statement against the server and fills a             provided  [System.Data.DataSet](T-System.Data.DataSet)  with the results.

* `sql`: The SQL statement to be executed against the server.
* `data`: The  [System.Data.DataSet](T-System.Data.DataSet)  to be filled with the results.
* `parameters`: The parameters for the SQL statement.

### `:Conclave.Data.Store.SqlStore.Read(System.String)`
Executes a SQL query against the server and returns a  [System.Data.Common.DbDataReader](T-System.Data.Common.DbDataReader) with which to read the results.

* `sql`: The SQL query to execute against the server.

**returns:** 
An  [System.Data.Common.DbDataReader](T-System.Data.Common.DbDataReader)  with which to reader the results.


### `:Conclave.Data.Store.SqlStore.Read(System.String,System.Data.IDbDataParameter[])`
Executes a parameterised SQL query against the server and returns a  [System.Data.Common.DbDataReader](T-System.Data.Common.DbDataReader) with which to read the results.

* `sql`: The SQL query to execute against the server.
* `parameters`: The parameters for the SQL query.

**returns:** 
An  [System.Data.Common.DbDataReader](T-System.Data.Common.DbDataReader)  with which to reader the results.


### `:Conclave.Data.Store.SqlStore.Exists(System.String)`
Determines whether or not there are any results returned from a provided query.

* `sql`: The sql to be executed.

**returns:** 
Returns true if any results are returned; otherwise,             returns false.


### `:Conclave.Data.Store.SqlStore.Exists(System.String,System.Data.IDbDataParameter[])`
Determines whether or not there are any results returned from a provided query.

* `sql`: The sql to be executed.
* `parameters`: Any query parameters used by the sql.

**returns:** 
Returns true if any results are returned; otherwise,             returns false.


## `T:Conclave.Data.Store.StoreStartedException`
An exception throw when an attempt is made to start a store that is already open.


## `T:Conclave.Data.Store.StoreState`
Expresses the current state of a backing store.

