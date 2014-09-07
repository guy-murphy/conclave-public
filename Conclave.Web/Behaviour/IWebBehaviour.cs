using Conclave.Process;

namespace Conclave.Web.Behaviour {
	public interface IWebBehaviour: IProcessBehaviour {
		void Action(IEvent ev, WebContext context);
	}
}