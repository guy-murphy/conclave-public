using System.IO;

namespace Conclave.Data.Resolver {
	public class DirectoryResolver: FileResolver {

		private DirectoryInfo _directoryInfo;

		public override string RelativePath {
			set {
				_directoryInfo = null; // reset the directory info
				base.RelativePath = value;
			}
		}

		public DirectoryInfo Info {
			get {
				if (_directoryInfo == null) {
					_directoryInfo = new DirectoryInfo(base.FullPath);
				}
				return _directoryInfo;
			}
		}

		public override bool Exists {
			get {
				return this.Info.Exists;
			}
		}

		public DirectoryResolver(string root) : base(root) { }
		public DirectoryResolver(string applicationPath, string root) : base(applicationPath, root) { }

	}
}
