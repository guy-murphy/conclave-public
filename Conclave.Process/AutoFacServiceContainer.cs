using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Configuration;
using Autofac.Core;

namespace Conclave.Process {
	public class AutoFacServiceContainer : IServiceContainer {

		private static readonly IServiceContainer _instance = new AutoFacServiceContainer();

		public static IServiceContainer Instance {
			get {
				return _instance;
			}
		}

		private bool _isDisposed;
		private readonly object _syncRoot = new Object();
		private IContainer _container;

		public object this[string name] {
			get { throw new NotImplementedException(); }
		}

		public AutoFacServiceContainer() {
			ContainerBuilder builder = new ContainerBuilder();
			builder.RegisterModule(new ConfigurationSettingsReader("autofac"));
			_container = builder.Build();
		}

		~AutoFacServiceContainer() {
			// ensure unmanaged resources are cleaned up
			this.Dispose(false);
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (!_isDisposed) {
				if (disposing) {
					// managed resource clean-up
					if (_container != null) _container.Dispose();
				}
				// unmanaged resource clean-up
				// ... nothing to do
				// call dispose on base class, and clear data
				// base.Dispose(disposing);
				_container = null;
				// mark disposing as done
				_isDisposed = true;
			}
		}


		public object GetObject(string name) {
			return this.GetObject<object>(name);
		}

		public object GetObject(string name, Type type) {
			return _container.ResolveNamed(name, type);
		}

		public T GetObject<T>(string name) {
			return _container.ResolveNamed<T>(name);
		}

		public void LoadServices() {
			throw new NotImplementedException();
		}

		public void Reload() {
			throw new NotImplementedException();
		}

		public bool ContainsObject(string name) {
			return (this.GetObject(name) != null);
		}
	}
}
