using Conclave.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conclave.Razor.Plugins
{
    public interface IRazorViewPlugin
    {
        string Execute(WebContext context, IDictionary<string, string> parameters, string source);
    }
}
