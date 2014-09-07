﻿
namespace Conclave.Process {

	/// <summary>
	/// The base type for behaviours in Conclave. Behaviours are intended
	/// to be registered against a context such as <see cref="ProcessContext"/>
	/// using `ProcessContext.Register(behaviour)`.
	/// </summary>
	/// <remarks>
	/// <para>
	/// When events are fired
	/// against that context, each behaviour registered will apply it's condition
	/// to the <see cref="IEvent"/> being fired. If this condition returns `true`,
	/// then the context will apply the behaviours `Action` against
	/// the event.
	/// </para>
	/// <para>
	/// Care should be taken to ensure behaviours are well behaved. To this
	/// end the following contract is implied by use of `IProcessBehaviour`:-
	/// </para>
	/// </remarks>
	/// <example>
	/// <code> <![CDATA[
	///		context.Register(behaviours);
	///		context.Fire("set-up");
	///		context.Fire("process-request");	
	///		context.Fire("tear-down");
	///		context.Completed();
	///		context.Response.ContentType = "text/xml";
	///		context.Response.Write(context.ControlState.ToXml());
	/// ]]> </code>
	/// </example>
	public interface IProcessBehaviour {
		/// <summary>
		/// Gets the message that the behaviour will respond to.
		/// </summary>
		/// <value>A `string` value.</value>
		string Message { get; }
		/// <summary>
		/// Process an action for the provided <see cref="IEvent"/>.
		/// </summary>
		/// <param name="ev">The event to be processed. </param>
		void Action(IEvent ev);
		/// <summary>
		/// process and action for the provided <see cref="IEvent"/>
		/// with the <see cref="ProcessContext"/> provided.
		/// </summary>
		/// <param name="ev">The event to process.</param>
		/// <param name="context">The context to use.</param>
		void Action(IEvent ev, ProcessContext context);

		/// <summary>
		/// The considtion that determines whether of not the behaviours action
		/// is valid to run.
		/// </summary>
		/// <param name="ev">The event to consider with the condition.</param>
		/// <returns>
		/// `true` if the condition is met; otherwise,  returns  `false`.
		/// </returns>
		bool Condition(IEvent ev);
	}
}
