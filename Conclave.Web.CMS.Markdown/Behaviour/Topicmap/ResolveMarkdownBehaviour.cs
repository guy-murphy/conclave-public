using System;

using Conclave.Collections;
using Conclave.Extensions;
using Conclave.Map.Model;
using Conclave.Process;
using Conclave.Web.Behaviour;

namespace Conclave.Web.CMS.Behaviour.Topicmap { // this is the wrong place, and is only here for now

	/// <summary>
	/// A behaviour for transforming Markdown notation
	/// into well formed HTML.
	/// </summary>
	public class ResolveMarkdownBehaviour : ProcessBehaviour {

		private static readonly ConcurrentDataDictionary<string> _memoize = new ConcurrentDataDictionary<string>();
		public ResolveMarkdownBehaviour(string message) : base(message) { }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ev"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		/// <remarks>
		/// Considers the occurrence::behaviour that is copied forward from the occurrence
		/// to the event parameters and checks that it is set to "markdown".
		/// </remarks>
		public override bool Condition(IEvent ev, ProcessContext context) {
			return base.Condition(ev, context) && ev["occurrence::behaviour"] == "markdown";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="ev"></param>
		/// <param name="context"></param>
		/// <remarks>
		/// The assumption of this behaviour is that the `ev.Object` will carry on it
		/// the occurrence that is being acted upon.
		/// </remarks>
		public override void Action(IEvent ev, ProcessContext context) {
			Occurrence o = ev.Object as Occurrence;
			if (o == null) {
				context.Errors.Add(new ErrorMessage("There is no occurence to resolve markdown for."));
			} else {
				if (o.ResolvedModel == null && o.HasData) { // its only set the once
					// first check if we have already processed this
					string hash = o.StringData.Hash();
					//string resolved = context.Cache.Get(hash) as String;
					string resolved = _memoize.ContainsKey(hash) ? _memoize[hash] : null;
					if (resolved == null) {
						resolved = o.StringData.ParseAsMarkdown();
						// cache the transform
						//context.Cache.Insert(hash, resolved);
						_memoize[hash] = resolved;
					}
					o.ResolvedModel = new TextData(resolved);
				}
			}
		}
	}
}
