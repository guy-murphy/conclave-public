using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conclave.Razor.Model
{
    // from http://stackoverflow.com/questions/711753/a-better-way-to-replace-many-strings-obfuscation-in-c-sharp

    /// <summary>
    /// 
    /// </summary>
    public class Token
    {
        /// <summary>
        /// 
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Replacement { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replacement"></param>
        public Token(string text, string replacement)
        {
            Text = text;
            Replacement = replacement;
        }

    }
}
