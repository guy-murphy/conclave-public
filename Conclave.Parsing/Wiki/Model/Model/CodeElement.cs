namespace Conclave.Parsing.Wiki.Model.Model {
	public class CodeElement : BlockElement {
		public CodeElement() : this("code-block") { }
		public CodeElement(string type) : base(type) { }
		public CodeElement(ContainingElement element) : base(element, "code-block") { }
		public CodeElement(ContainingElement element, string type) : base(element, type) { }
	}
}
