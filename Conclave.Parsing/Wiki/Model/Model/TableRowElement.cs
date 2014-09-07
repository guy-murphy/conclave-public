using System.Text;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class TableRowElement: LineElement {
		public TableRowElement() : base("row") {}

		public TableRowElement(LineElement element): base(element, "row") {
			this.Parse();
		}

		public void Parse() {
			StringBuilder sb = new StringBuilder();
			foreach (WikiElement child in this.Children) {
				sb.Append(child.Original);
			}
			this.ParseLine(sb.ToString());
		}

		public void ParseLine(string line) {
			this.ClearChildren();
			foreach (string part in line.Split(':')) {
				this.AddChild(new TableCellElement(part));
			}
		}
	}
}
