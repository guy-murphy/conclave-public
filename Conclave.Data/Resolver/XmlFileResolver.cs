using System.Xml;

namespace Conclave.Data.Resolver {
	public class XmlFileResolver: TextFileResolver {

		public XmlFileResolver(string root) : base(root) { }
		public XmlFileResolver(string applicationPath, string root) : base(applicationPath, root) { }

		public XmlReader GetXmlReader() {
			XmlTextReader reader = new XmlTextReader(this.FullPath);
			return reader;
		}

		public XmlDocument GetXmlDocument() {
			XmlDocument xml = new XmlDocument();
			xml.Load(this.FullPath);
			return xml;
		}

	}
}
