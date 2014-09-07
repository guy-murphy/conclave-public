using System.IO;

namespace Conclave.Data.Resolver {
	public class TextFileResolver: FileResolver {

		public string Text {
			get {
				return this.ReadText();
			}
			set {
				this.WriteText(value);
			}
		}

		public TextFileResolver(string root) : base(root) { }
		public TextFileResolver(string applicationPath, string root) : base(applicationPath, root) { }

		public virtual TextReader OpenTextFile() {
			return File.OpenText(this.FullPath);
		}

		public virtual Stream OpenStream() {
			return File.Open(this.FullPath, FileMode.Open);
		}

		public virtual string ReadText() {
			return File.ReadAllText(this.FullPath);
		}

		public virtual void WriteText(string text) {
			File.WriteAllText(this.FullPath, text);
		}

		public virtual void AppendText(string text) {
			File.AppendAllText(this.FullPath, text);
		}
	}
}
