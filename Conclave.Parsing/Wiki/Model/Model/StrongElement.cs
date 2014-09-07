using System.Collections.Generic;
using System.Xml;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class StrongElement: WikiElement {
		public StrongElement() : base() {}
		public StrongElement(string original) : base(original) {}
		public StrongElement(WikiElement element) : base(element) {}
		public StrongElement(string original, WikiElement parent, params WikiElement[] children) : base(original, parent, children) {}
		public StrongElement(string original, WikiElement parent, IEnumerable<WikiElement> children) : base(original, parent, children) {}

		public override void ToXml(XmlWriter writer) {
			writer.WriteStartElement("strong");
			this.ContextToXml(writer);
			writer.WriteEndElement();
		}
	}
}
