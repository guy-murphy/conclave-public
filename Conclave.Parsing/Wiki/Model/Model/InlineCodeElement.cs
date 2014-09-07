using System.Collections.Generic;
using System.Xml;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class InlineCodeElement : WikiElement {
		public InlineCodeElement() : base() {}
		public InlineCodeElement(string original) : base(original) { }
		public InlineCodeElement(WikiElement element) : base(element) { }
		public InlineCodeElement(string original, WikiElement parent, params WikiElement[] children) : base(original, parent, children) { }
		public InlineCodeElement(string original, WikiElement parent, IEnumerable<WikiElement> children) : base(original, parent, children) { }

		public override void ToXml(XmlWriter writer) {
			writer.WriteStartElement("code");
			this.ContextToXml(writer);
			writer.WriteEndElement();
		}
	}
}
