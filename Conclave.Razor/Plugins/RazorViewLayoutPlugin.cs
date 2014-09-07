using Conclave.Collections;
using Conclave.Razor.Model;
using Conclave.Web;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Conclave.Razor.Extensions;

namespace Conclave.Razor.Plugins
{
    /// <summary>
    /// A RazorView plugin that parses a template for the 'this.Layout = @"(path to layout file)"' command.
    /// A matched layout will then be attempted to be compiled, ready for RazorEngine to use it.
    /// The path to the layout file is expected to be relative to the base Resources\Views\Razor folder.
    /// </summary>
    public class RazorViewLayoutPlugin : IRazorViewPlugin
    {
        private static Regex _regexLayout = new Regex("this\\.Layout = \\@\"(.*?)\";", RegexOptions.Compiled);

        private readonly IList<IRazorViewPlugin> _layoutPlugins;

        public RazorViewLayoutPlugin()
        {
            _layoutPlugins = new List<IRazorViewPlugin>();
        }

        public RazorViewLayoutPlugin(IList<IRazorViewPlugin> layoutPlugins)
        {
            _layoutPlugins = layoutPlugins;
        }

        public string Execute(WebContext context, IDictionary<string, string> parameters, string source)
        {
            Match match = _regexLayout.Match(source);

            if (match.Success)
            {
                string layoutName = match.Groups[1].Value;

                string safeLayoutName = layoutName.FixPathSeparatorChars();

                string templateFolder = parameters["templatefolder"];

                string layoutPath = Path.Combine(templateFolder, safeLayoutName);

                ITemplate compiledInclude = RazorEngine.Razor.Resolve(layoutName);

                if (compiledInclude == null || TemplateStatus.TemplateIsFresh(layoutPath))
                { // we'll need to look for the template
                    if (File.Exists(layoutPath))
                    {
                        string layout = File.ReadAllText(layoutPath);
                        IDictionary<string, string> layoutParameters = new Dictionary<string, string>(parameters);
                        layoutParameters["templatename"] = safeLayoutName;
                        layoutParameters["templatepath"] = layoutPath;
                        layout = ExecutePlugins(context, layoutParameters, layout);
                        RazorEngine.Razor.Compile(layout, context.ViewSteps.Last.Model.GetType(), layoutName);
                    }
                }
            }

            return source;
        }

        private string ExecutePlugins(WebContext context, IDictionary<string, string> parameters, string source)
        {
            return _layoutPlugins.Aggregate(source, (current, plugin) => plugin.Execute(context, parameters, current));
        }

    }
}
