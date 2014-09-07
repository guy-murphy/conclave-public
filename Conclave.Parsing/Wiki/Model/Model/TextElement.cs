using System.Collections.Generic;
using System.Xml;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class TextElement: WikiElement {
		public TextElement(string original) : base(original) {}
		public TextElement(WikiElement element) : base(element) {}
		public TextElement(string original, WikiElement parent, params WikiElement[] children) : base(original, parent, children) {}
		public TextElement(string original, WikiElement parent, IEnumerable<WikiElement> children) : base(original, parent, children) {}

		public override void ToXml(XmlWriter writer) {
			writer.WriteValue(this.Original);
		}
	}
}