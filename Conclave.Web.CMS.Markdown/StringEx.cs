using System;
using MarkdownDeep;

namespace Conclave.Web.CMS {

	/// <summary>
	/// Extends the string type with markdown
	/// functionality.
	/// </summary>
	public static class StringEx {

		/// <summary>
		/// Parse a string self as markdown and return
		/// a HTML representation as a string.
		/// </summary>
		/// <param name="self">The text to be parsed.</param>
		/// <returns>The HTML representation of the text.</returns>
		public static string ParseAsMarkdown(this string self) {
			Markdown parser = new Markdown {
				                               ExtraMode = true,
				                               AutoHeadingIDs = true,
				                               PrepareLink = _resolveTopicLinks
			                               };
			return parser.Transform(self);
		}

		private static bool _resolveTopicLinks(HtmlTag tag) {
			if (tag.attributes.ContainsKey("href")) {
				string url = tag.attributes["href"];
				if (url.StartsWith("#") && url.Length > 1) {
					string topidId = url.Substring(1);
					tag.attributes["href"] = String.Format("?id={0}", topidId);
				}
			}
			return true;
		}

	}
}
