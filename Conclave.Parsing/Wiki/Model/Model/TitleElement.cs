namespace Conclave.Parsing.Wiki.Model.Model {
	public class TitleElement : TextElement {
		public TitleElement(string original) : base(original) { }

		public override void ToXml(System.Xml.XmlWriter writer) {
			writer.WriteStartElement("title");
			writer.WriteValue(this.Original.Trim());
			writer.WriteEndElement();
		}
	}
}
