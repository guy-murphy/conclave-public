using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conclave.Parsing.Extensions {
	public static class StringEx {
		public static string WikiNameToLabel(this string wikiName) {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < wikiName.Length; i++) {
				if (wikiName[i] == '_') {
					sb.Append(' ');
				} else {
					if (i != 0) { // we dont insert a space before the first letter
						if (i == wikiName.Length - 1) { // it's the last letter
							if (char.IsUpper(wikiName[i]) && char.IsLower(wikiName[i - 1]) && char.IsLetter(wikiName[i - 1])) {
								sb.Append(' ');
							}
						} else {
							if (char.IsUpper(wikiName[i]) && char.IsLower(wikiName[i + 1]) && char.IsLetter(wikiName[i - 1])) {
								sb.Append(' ');
							}
						}
					}
					sb.Append(wikiName[i]);
				}
			}
			return sb.ToString();
		}
	}
}
