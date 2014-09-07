using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conclave.Razor
{
    public static class TemplateStatus
    {
        private static readonly object _templateCreationLock = new object();
        private static readonly IDictionary<string, long> _templateLastModified = new Dictionary<string, long>();

        public static bool TemplateIsFresh(string templatePath)
        {
            if (!File.Exists(templatePath))
            {
                return false;
            }

            FileInfo templateFileInfo = new FileInfo(templatePath);

            if (!_templateLastModified.ContainsKey(templatePath))
            {
                lock (_templateCreationLock)
                {
                    if (!_templateLastModified.ContainsKey(templatePath))
                    {
                        _templateLastModified.Add(templatePath, templateFileInfo.LastWriteTimeUtc.Ticks);
                        return true;
                    }
                }
            }

            bool fresh = (_templateLastModified[templatePath] < templateFileInfo.LastWriteTimeUtc.Ticks);

            if (fresh)
            {
                lock (_templateCreationLock)
                {
                    _templateLastModified.Remove(templatePath);
                    _templateLastModified.Add(templatePath, templateFileInfo.LastWriteTimeUtc.Ticks);
                }
            }

            return fresh;
        }

    }
}
