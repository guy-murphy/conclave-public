using System;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class EmptyLineElement: WikiElement {
		public EmptyLineElement() : base((string) String.Empty) {}

		public override void ToXml(System.Xml.XmlWriter writer) {
			writer.WriteElementString("empty-line", String.Empty);
		}
	}
}
