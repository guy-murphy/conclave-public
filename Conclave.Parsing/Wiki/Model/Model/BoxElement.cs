namespace Conclave.Parsing.Wiki.Model.Model {
	public class BoxElement : BlockElement {
		public BoxElement() : this("box") { }
		public BoxElement(string type) : base(type) { }
		public BoxElement(ContainingElement element) : base(element, "box") { }
		public BoxElement(ContainingElement element, string type) : base(element, type) { }
	}
}
