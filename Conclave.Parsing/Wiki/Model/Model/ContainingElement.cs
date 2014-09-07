using System;
using System.Xml;

namespace Conclave.Parsing.Wiki.Model.Model {
	public abstract class ContainingElement: WikiElement {

		private readonly string _type;

		public string Type {
			get { return _type; }
			//set { _type = value; }
		}

		protected ContainingElement(string type): base((string) String.Empty) {
			_type = type;
		}

		protected ContainingElement(ContainingElement element) : base(element) {
			_type = element.Type;
		}

		protected ContainingElement(ContainingElement element, string type) : base(element) {
			_type = type;
		}

		public override void ToXml(XmlWriter writer) {
			writer.WriteStartElement(this.Type);
			base.ContextToXml(writer);
			writer.WriteEndElement();
		}


	}
}
