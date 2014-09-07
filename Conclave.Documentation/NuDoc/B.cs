
namespace ClariusLabs.NuDoc {
	/// <summary>
	/// Represents the <c>b</c> documentation tag.
	/// </summary>
	/// <remarks>
	/// This class seemed to be missing so I added it.
	/// </remarks>
	public class B : Element {
		/// <summary>
		/// Initializes a new instance of the <see cref="B"/> class 
		/// with the given content. 
		/// </summary>
		public B(string content) {
			this.Content = content;
		}

		/// <summary>
		/// Accepts the specified visitor.
		/// </summary>
		public override TVisitor Accept<TVisitor>(TVisitor visitor) {
			visitor.VisitB(this);
			return visitor;
		}

		/// <summary>
		/// Gets the code content.
		/// </summary>
		public string Content { get; private set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		public override string ToString() {
			return "<b>" + base.ToString();
		}
	}
}