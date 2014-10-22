using System;
using System.Web;

using Conclave.Process;
using System.Web.Compilation;

namespace Conclave.Web {
	public class WebApplication: HttpApplication {

		private bool _isDisposed = false;
		private readonly IServiceContainer _services;
		private readonly string _baseDirectory;

		public IServiceContainer Services {
			get {
				return _services;
			}
		}

		public string BaseDirectory {
			get {
				return _baseDirectory;
			}
		}

		public WebApplication() {
			//_services = new SpringServiceContainer();
			_services = SpringServiceContainer.Instance;
			_baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
		}

		~WebApplication() {
			this.Dispose(false);
		}

		public sealed override void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (!_isDisposed) {
				if (disposing) {
					// managed resources
					_services.Dispose();
				}
				// unmanaged resource

			}
			_isDisposed = true;
		}

        protected virtual void Application_Start()
        {
            BuildManager.GetReferencedAssemblies();
        }
	}
}
