//using System;
//using Conclave.Extensions;
//using Conclave.Map.Model;
//using Conclave.Parsing;
//using Conclave.Process;
//using Conclave.Web.Behaviour;

//namespace Conclave.Web.CMS.Behaviour.Topicmap { // this is the wrong place, and is only here for now

//	/// <summary>
//	/// A behaviour for transforming Markdown notation
//	/// into well formed HTML.
//	/// </summary>
//	public class ResolveMarkdownBehaviour: WebBehaviour {
//		public ResolveMarkdownBehaviour(string message) : base(message) {}

//		public override bool Condition(Process.IEvent ev, WebContext context) {
//			return base.Condition(ev, context) && ev["behaviour"] == "markdown";
//		}

//		public override void Action(Process.IEvent ev, WebContext context) {
//			Occurrence o = ev.Object as Occurrence;
//			if (o == null) {
//				context.Errors.Add(new ErrorMessage("There is no occurence to resolve markdown for."));
//			} else {
//				if (o.ResolvedModel == null && o.HasData) { // its only set the once
//					// first check if we have already processed this
//					string hash = o.StringData.Hash();
//					string resolved = context.Cache.Get(hash) as String;
//					if (resolved == null) {
//						Markdown2 parser = new Markdown2();
//						resolved = parser.Transform(o.StringData);
//						// cache the transform
//						context.Cache.Insert(hash, resolved);
//					}
//					o.ResolvedModel = new TextData(resolved);
//				}
//			}
//		}
//	}
//}
