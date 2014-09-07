namespace Conclave.Parsing.Wiki.Model.Model {
	public class BlockQuoteElement : BlockElement {
		public BlockQuoteElement() : this("block-quote") { }
		public BlockQuoteElement(string type) : base(type) { }
		public BlockQuoteElement(ContainingElement element) : base(element, "block-quote") { }
		public BlockQuoteElement(ContainingElement element, string type) : base(element, type) { }
	}
}
