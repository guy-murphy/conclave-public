namespace Conclave.Parsing.Wiki.Model.Model {
	public class BlockElement: ContainingElement {
		public BlockElement() : this("block") {}
		public BlockElement(string type) : base(type) {}
		public BlockElement(ContainingElement element) : base(element, "block") {}
		public BlockElement(ContainingElement element, string type) : base(element, type) { }
	}
}
