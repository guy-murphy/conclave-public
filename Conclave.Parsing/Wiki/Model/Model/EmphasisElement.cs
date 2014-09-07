using System.Collections.Generic;
using System.Xml;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class EmphasisElement: WikiElement {
		public EmphasisElement() : base() {}
		public EmphasisElement(string original) : base(original) {}
		public EmphasisElement(WikiElement element) : base(element) {}
		public EmphasisElement(string original, WikiElement parent, params WikiElement[] children) : base(original, parent, children) {}
		public EmphasisElement(string original, WikiElement parent, IEnumerable<WikiElement> children) : base(original, parent, children) {}

		public override void ToXml(XmlWriter writer) {
			writer.WriteStartElement("emph");
			this.ContextToXml(writer);
			writer.WriteEndElement();
		}
	}
}
