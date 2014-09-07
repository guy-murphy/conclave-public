using System.Text.RegularExpressions;
using Conclave.Extensions;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class LinkElement: WikiElement {

		private static readonly Regex UrlPattern = new Regex("(http|https|ftp|ftps)://([^ \\t\\n\\r])+", RegexOptions.Compiled);
		private static readonly Regex ImgPattern = new Regex("([^ \t\r\n])+\\.(jpg|png|gif|jpeg|svg)", RegexOptions.Compiled);

		private string _label;
		private string _link;
		private string _type;
		private string _param;

		public LinkElement(string original) : base(original) {
			this.ParseOriginal();
		}

		private string _inferType(string link) {
			if (UrlPattern.IsMatch(link)) {
				_type = "url";
			} else if (ImgPattern.IsMatch(link)) {
				_type = "image";
			} else {
				_type = "topic";
			}
			return _type;
		}

		private void ParseOriginal() {
			string text = this.Original.Trim().TrimEndsBy(1); // get rid of the enclosing [] brackets
			string[] parts = text.Split('|');

			switch (parts.Length) {
				case 1:
					_link = parts[0];
					_inferType(_link);
					break;
				case 2:
					_label = parts[0];
					_link = parts[1];
					_inferType(_link);
					break;
				case 3:
					_label = parts[0];
					_param = parts[1];
					_link = parts[2];
					_inferType(_link);
					break;
			}

		}

		
	}
}
