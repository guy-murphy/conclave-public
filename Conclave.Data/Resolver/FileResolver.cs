using System;
using System.IO;

namespace Conclave.Data.Resolver
{
	/// <summary>
	/// A base class used to resolve
	/// filesystem like resources.
	/// </summary>
	/// <threadsafety>
	/// This class is not threadsafe, especially with
	/// regard to <see cref="RelativePath"/> and the changing
	/// location of the underlying resource being pointed to.
	/// </threadsafety>
    public abstract class FileResolver: IResolver
    {
		private readonly string _appPath;
		private readonly string _rootPath;
		private string _relativePath;

		/// <inheritdoc />
		public string ApplicationPath {
			get {
				return _appPath;
			}
		}

		/// <inheritdoc />
		public string RootPath {
			get {
				return _rootPath;
			}
		}

		/// <inheritdoc />
		public virtual string RelativePath {
			get {
				return _relativePath;
			}
			set {
				_relativePath = value;
			}
		}

		/// <inheritdoc />
		public string FullPath {
			get {
				return Path.Combine(this.ApplicationPath, this.RootPath, this.RelativePath); 
			}
		}

		/// <inheritdoc />
		public virtual bool Exists {
			get {
				return File.Exists(this.FullPath);
			}
		}

		protected FileResolver(string root) : this(AppDomain.CurrentDomain.BaseDirectory, root) { }

		protected FileResolver(string applicationPath, string root) {
			_appPath = applicationPath;
			_rootPath = root;
		}

		/// <inheritdoc />
		public bool Create() {
			if (!this.Exists) {
				// get the path to what would be the parent directory
				string directoryPath = this.FullPath.Remove(this.FullPath.LastIndexOf(Path.DirectorySeparatorChar));
				// rather than checking if the parent directory exists, simply create it
				// if it already exists then nothing happens
				Directory.CreateDirectory(directoryPath);
				// then create the actual resource
				using (File.Create(this.FullPath)) { /* ensure the resulting stream is disposed */ }
				return true;
			} else {
				return false;
			}
		}

		/// <inheritdoc />
		public virtual void Remove() {
			File.Delete(this.FullPath);
		}

    }
}
