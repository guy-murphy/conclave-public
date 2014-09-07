using System;
using Conclave.Process;

namespace Conclave.Web.Behaviour
{
    public class HelloWorldBehaviour: WebActionBehaviour
    {

        public HelloWorldBehaviour(string message): base(message) {}

	    public override void Action(IEvent ev, WebContext context)
        { 
			context.Messages.Add("Hello World!");
			context.Messages.Add(DateTime.Now.ToLongTimeString());
        }
        
    }
}
