using Conclave.Process;

namespace Conclave.Web.CMS.Behaviour {

	/// <summary>
	/// A behaviour concerned with parsing markdown notation
	/// into HTML.
	/// </summary>
	/// <remarks>
	/// Responds to a messsage of *parse::markdown*, and uses the
	/// parameter *markdown* the value of which it transforms into
	/// HTML which is assigned to the *IEvent.Object*
	/// </remarks>

	public class ParseMarkdownBehaviour : ProcessBehaviour {
		public ParseMarkdownBehaviour(string message) : base(message) { }

		public override bool Condition(Process.IEvent ev) {
			return base.Condition(ev) && ev.Message == "parse::markdown" && ev.HasParams("markdown");
		}

		public override void Action(IEvent ev, ProcessContext context) {
			ev.Object = ev["markdown"].ParseAsMarkdown();
		}
	}
}
