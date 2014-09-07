﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Conclave.Collections;
using Conclave.Process;

namespace Conclave.Web.Behaviour {

	/// <summary>
	/// Constructs the initial view state of the reuqest
	/// as a <see cref="ViewStep"/> composed of the current <see cref="ProcessContext.ControlState"/>.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is basically a filtering of the <see cref="ProcessContext.ControlState"/> into
	/// the model called the view state, that is going to be rendered. Any item in the control state with a key
	/// starting with the underscore character '_' is regarded as protected, and will
	/// not be copied forward to the view state.
	/// </para>
	/// <para>
	/// If one wished to present a model for render by different means,
	/// or wanted to change how the filtering was done, this is the behaviour
	/// you would swap out for an alternate implementation.
	/// </para>
	/// </remarks>
	public class ViewStateBehaviour: WebBehaviour {

		public ViewStateBehaviour(string name) : base(name) { }

		public override void Action(IEvent ev, WebContext context) {
			DataDictionary<IData> model = new DataDictionary<IData>();

			// copy from the context
			model["messages"] = context.Messages;
			model["errors"] = context.Errors;
			model["flags"] = context.Flags;
			model["timers"] = context.Timers;
			model["params"] = new DataDictionary<string>(context.Params.Where(p => !p.Key.StartsWith("_")));

			// copy from the control state
			foreach (KeyValuePair<string, object> entry in context.ControlState) {
				if (!entry.Key.StartsWith("_")) { // exclude "private" items
					if (entry.Value is IData) {
						model[entry.Key] = entry.Value as IData;
					} else {
						model[entry.Key] = new TextData(entry.Value.ToString());
					}
				}
			}

			if (context.HasParams("model-item") && model.ContainsKey(context.Params["model-item"])) {
				context.ViewSteps.CreateStep("view-state", model[context.Params["model-item"]]);
			} else {
				context.ViewSteps.CreateStep("view-state", model);
			}
		}

	}
}
