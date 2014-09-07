namespace Conclave.Parsing.Wiki.Model.Model {
	public class LineElement: ContainingElement {
		public LineElement() : this("line") {}
		public LineElement(string type) : base(type) {}
		public LineElement(ContainingElement element) : base(element, "line") {}
		public LineElement(ContainingElement element, string type) : base(element, type) {}
	}
}
