using System;
using System.Xml;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class HeadingElement: WikiElement {

		private readonly int _size;

		public HeadingElement(int size) {
			_size = size;
		}

		public override void ToXml(XmlWriter writer) {
			writer.WriteStartElement(String.Concat("h", _size.ToString()));
			this.ContextToXml(writer);
			writer.WriteEndElement();
		}

	}
}
