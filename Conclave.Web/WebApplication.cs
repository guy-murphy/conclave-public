using System;
using System.Web;

using Conclave.Process;
using System.Web.Compilation;

namespace Conclave.Web {
	public class WebApplication: HttpApplication {

		private bool _isDisposed = false;
		
		private readonly string _baseDirectory;

        private static Func<IServiceContainer> _serviceContainerFactory;
        public static Func<IServiceContainer> ServiceContainerFactory
        {
            set
            {
                if (_serviceContainerFactory != null)
                {
                    throw new ApplicationException("ServiceContainerFactory already set");
                }
                _serviceContainerFactory = value;
            }
        }

        private static readonly object _servicesLock = new object();

        private static IServiceContainer _services;

		public static IServiceContainer Services {
			get {
                // use a double synchronisation lock as Services may be under contention
                if(_services == null)
                {
                    lock(_servicesLock)
                    {
                        if(_services == null)
                        {
                            // check if we have a factory set up
                            if(_serviceContainerFactory == null)
                            {
                                // use spring as a default
                                _serviceContainerFactory = () => SpringServiceContainer.Instance;
                            }
                            _services = _serviceContainerFactory();
                        }
                    }
                }
				return _services;
			}
		}

		public string BaseDirectory {
			get {
				return _baseDirectory;
			}
		}

		public WebApplication() {
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
