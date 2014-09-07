## `P:Conclave.Process.IEvent.Object`


#### Remarks
I felt compelled to put an escape hatch in here. It's wrong and I feel dirty, but I did it anyway.

## `T:Conclave.Process.IProcessBehaviour`
The base type for behaviours in Conclave. Behaviours are intended to be registered against a context such as  [Conclave.Process.ProcessContext](T-Conclave.Process.ProcessContext) using `ProcessContext.Register(behaviour)`.

#### Remarks
When events are fired against that context, each behaviour registered will apply it's condition to the  [Conclave.Process.IEvent](T-Conclave.Process.IEvent)  being fired. If this condition returns `true`,             then the context will apply the behaviours `Action` against             the event.

Care should be taken to ensure behaviours are well behaved. To this end the following contract is implied by use of `IProcessBehaviour`:-


##### Example

    context.Register(behaviours);
    context.Fire("set-up");
    context.Fire("process-request");	
    context.Fire("tear-down");
    context.Completed();
    context.Response.ContentType = "text/xml";
    context.Response.Write(context.ControlState.ToXml());


### `.Action(Conclave.Process.IEvent)`
Process an action for the provided  [Conclave.Process.IEvent](T-Conclave.Process.IEvent) .

* `ev`: The event to be processed. 

### `.Action(Conclave.Process.IEvent,Conclave.Process.ProcessContext)`
process and action for the provided  [Conclave.Process.IEvent](T-Conclave.Process.IEvent) with the  [Conclave.Process.ProcessContext](T-Conclave.Process.ProcessContext)  provided.

* `ev`: The event to process.
* `context`: The context to use.

### `.Condition(Conclave.Process.IEvent)`
The considtion that determines whether of not the behaviours action is valid to run.

* `ev`: The event to consider with the condition.

**returns:** 
`true` if the condition is met; otherwise,  returns  `false`.

### `.Message`
Gets the message that the behaviour will respond to.


**value:** A `string` value.

## `T:Conclave.Process.ProcessBehaviour`
A simple named behaviour with a default condition matching that name againts  [Conclave.Process.IEvent.Message](P-Conclave.Process.IEvent.Message) .


### `.#ctor(System.String)`
Creates a new instance of the behaviour.

* `message`: The name of the behaviour.

### `.Condition(Conclave.Process.IEvent)`
Determines if the event specifies the behaviour by name.

* `ev`: The event to consult.

**returns:** 
Returns true if the  [Conclave.Process.IEvent.Message](P-Conclave.Process.IEvent.Message) is the same as the  [Conclave.Process.ProcessBehaviour.Message](P-Conclave.Process.ProcessBehaviour.Message) 

## `P:Conclave.Process.User.UserCredentials.Password`
A password used in conjunction with a name for authentication. Whether or not this is a hash is implementation specific, while remembering that storing clear-text passwords in a database makes the Little Baby Jesus cry.


## `T:Conclave.Process.ViewStep`
Represents a step in a rendering view pipeline.

#### Remarks
A step can either have  [Conclave.Process.ViewStep.Content](P-Conclave.Process.ViewStep.Content)  or             a  [Conclave.Process.ViewStep.Model](P-Conclave.Process.ViewStep.Model) , but not both.

### `.#ctor(System.String,System.String,System.String)`
Creates a new instance of a step with the parameters provided.

* `name`: Human readable name of the step.
* `contentType`: The type of the steps content.
* `content`: The actual content of the step.

### `.#ctor(System.String,Conclave.IData)`
Creates a new instance of a step with the parameters provided.

* `name`: The human readable name of the step.
* `model`: The actual model of the step.
### `.Name`
The human readable name of the step.

### `.ContentType`
The content type of the  [Conclave.Process.ViewStep.Content](P-Conclave.Process.ViewStep.Content) if there is any.

### `.Content`
The content if any of the step.

### `.Model`
The model if any of the step.

### `.HasContent`
Determines whether or not the step has any content.

### `.HasModel`
Determines whether or not the step has a model.

