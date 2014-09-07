using Conclave.Collections;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class TableElement: BlockElement {
		public TableElement() : base("table") {}

		public override void Complete() {
			DataCollection<WikiElement> rows = new DataCollection<WikiElement>();
			foreach (WikiElement child in this.Children) {
				LineElement line = child as LineElement;
				if (line != null) {
					TableRowElement row = new TableRowElement(line);
					rows.Add(row);
				}
			}
			this.ClearChildren();
			this.AddChildren(rows);
		}
	}
}
