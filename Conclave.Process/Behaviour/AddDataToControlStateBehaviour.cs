using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conclave.Process.Behaviour
{
    public class AddDataToControlStateBehaviour : ProcessBehaviour
    {
        private readonly IDictionary<string, object> _data;

        protected AddDataToControlStateBehaviour(string message, IDictionary<string, object> data) : base(message)
        {
            _data = data;
		}

		public override void Action(IEvent ev, ProcessContext context)
        {
            foreach(KeyValuePair<string, object> kvp in _data)
            {
                context.ControlState.Add(kvp);
            }
		}
    }
}
