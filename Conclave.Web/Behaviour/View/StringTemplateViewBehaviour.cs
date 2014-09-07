using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using Antlr4.StringTemplate;
using Conclave.Collections;
using Conclave.Process;

namespace Conclave.Web.Behaviour.View {
	public class StringTemplateViewBehaviour: WebBehaviour {

		private readonly string _contentType;

		public StringTemplateViewBehaviour(string message) : this(message, "text/html") { }

		public StringTemplateViewBehaviour(string message, string contentType): base(message) {
			_contentType = contentType;
		}

		private static IEnumerable<string> _possibleTemplates(WebContext context) {
			string area = context.Params["area"];
			string concern = context.Params["concern"];
			string action = String.Format("{0}.st", context.Params["action"]);
			const string @default = "default.st";

			// area/concern/action
			yield return Path.Combine(area, concern, action);
			yield return Path.Combine(area, concern, @default);
			// area/action
			yield return Path.Combine(area, action);
			yield return Path.Combine(area, @default);
			// concern/action
			yield return Path.Combine(concern, action);
			yield return Path.Combine(concern, @default);
			// action
			yield return action;
			yield return @default;

		}

		public override void Action(IEvent ev, WebContext context) {

			if (context.ViewSteps.HasSteps && context.ViewSteps.Last.HasModel) {
				foreach (string templateName in _possibleTemplates(context)) { // check each possible template in turn
					// check if we have the template cached
					string cacheKey = String.Concat("st::", templateName);

					Template template = context.Flags.Contains("nocache") ? null : context.Cache.Get(cacheKey) as Template;
					if (template == null) {
						string templatePath = Path.Combine(context.Application.BaseDirectory, "Resources", "Views", "ST", templateName);
						if (File.Exists(templatePath)) {
							string templateContent = File.ReadAllText(templatePath);
							template = new Template(templateContent, '#', '#');
							//context.Cache.Add(cacheKey, template, new CacheDependency(templatePath), Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), CacheItemPriority.High, null);
						}
					}
					if (template != null) {
						DataDictionary<string> parms = new DataDictionary<string>();
						foreach (KeyValuePair<string, string> parm in context.Params) {
							parms.Add(parm.Key.Replace('-', '_'), parm.Value);
						}
						template.Add("params", parms);
						template.Add("timers", context.Timers);
						template.Add("model", context.ViewSteps.Last.Model);
						context.ViewSteps.CreateStep(templateName, _contentType, template.Render());
						break; // break out of the loop, or we'll just find more templates
					}
				}
			}


		}

	}
}
