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
    public class TokenList : List<Token>
    {
        /// <summary>
        /// 
        /// </summary>
        public TokenList() : base() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        public TokenList(IEnumerable<Token> source) : base(source) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replacement"></param>
        public void Add(string text, string replacement)
        {
            Add(new Token(text, replacement));
        }

        private Token GetFirstToken()
        {
            Token result = null;
            int index = int.MaxValue;
            foreach (Token token in this)
            {
                if (token.Index != -1 && token.Index < index)
                {
                    index = token.Index;
                    result = token;
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Replace(string text)
        {
            StringBuilder result = new StringBuilder();
            foreach (Token token in this)
            {
                token.Index = text.IndexOf(token.Text, System.StringComparison.Ordinal);
            }
            int index = 0;
            Token next;
            while ((next = GetFirstToken()) != null)
            {
                if (index < next.Index)
                {
                    result.Append(text, index, next.Index - index);
                    index = next.Index;
                }
                result.Append(next.Replacement);
                index += next.Text.Length;
                next.Index = text.IndexOf(next.Text, index, System.StringComparison.Ordinal);
            }
            if (index < text.Length)
            {
                result.Append(text, index, text.Length - index);
            }
            return result.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="replacement"></param>
        public void AddOrUpdate(string text, string replacement)
        {
            Token token = this.FirstOrDefault(i => i.Text == text);
            if (token != null)
            {
                token.Replacement = replacement;
            }
            else
            {
                this.Add(text, replacement);
            }
        }
    }
}
