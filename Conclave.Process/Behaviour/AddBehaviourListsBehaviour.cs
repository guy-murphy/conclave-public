using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conclave.Process.Behaviour
{
    /// <summary>
    /// Accept a list of names of behaviour lists and register them on the context
    /// </summary>
    public class AddBehaviourListsBehaviour : ProcessBehaviour
    {
        private readonly List<string> _behaviourListNames;

        protected AddBehaviourListsBehaviour(string message, IEnumerable<string> behaviourListNames) : base(message)
        {
            _behaviourListNames = behaviourListNames.ToList();
		}

		public override void Action(IEvent ev, ProcessContext context)
        {
            foreach (string behaviourListName in _behaviourListNames)
            {
                context.Register(context.Services.GetObject<List<IProcessBehaviour>>(behaviourListName));
            }
		}
    }
}
