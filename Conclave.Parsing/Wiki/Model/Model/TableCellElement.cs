using System.Collections.Generic;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class TableCellElement : TextElement {
		public TableCellElement(string original) : base(original) {}
		public TableCellElement(WikiElement element) : base(element) {}
		public TableCellElement(string original, WikiElement parent, params WikiElement[] children) : base(original, parent, children) {}
		public TableCellElement(string original, WikiElement parent, IEnumerable<WikiElement> children) : base(original, parent, children) {}

		public override void ToXml(System.Xml.XmlWriter writer) {
			writer.WriteStartElement("td");
			writer.WriteValue(this.Original);
			writer.WriteEndElement();
		}
	}
}