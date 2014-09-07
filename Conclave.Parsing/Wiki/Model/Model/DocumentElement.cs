namespace Conclave.Parsing.Wiki.Model.Model {
	public class DocumentElement: ContainingElement {
		public DocumentElement() : this("wiki") {}
		public DocumentElement(string type) : base(type) {}
		public DocumentElement(ContainingElement element) : base(element, "wiki") {}
		public DocumentElement(ContainingElement element, string type) : base(element, type) { }
	}
}
