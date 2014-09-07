using System.Xml;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class ListItemElement: LineElement {

		private int _level;

		public int Level {
			get { return _level; }
			set { _level = value; }
		}

		public ListItemElement() : base("item") {}

		public override void ContextToXml(XmlWriter writer) {
			writer.WriteStartElement(this.Type);
			writer.WriteAttributeString("level", this.Level.ToString());
			this.ContextToXml(writer);
			writer.WriteEndElement();
		}
	}
}