using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conclave.Razor.Extensions
{
    public static class StringEx
    {
        public static string FixPathSeparatorChars(this string path)
        {
            return path.Replace('\\', System.IO.Path.DirectorySeparatorChar);
        }
    }
}
