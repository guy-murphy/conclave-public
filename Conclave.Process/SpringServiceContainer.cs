using System;
using System.Configuration;
using Spring.Context;
using Spring.Context.Support;
using System.Collections.Generic;
using System.Linq;

using Conclave.Extensions;

namespace Conclave.Process {
	public class SpringServiceContainer : IServiceContainer {

        private static IServiceContainer _instance = CreatePlatformContainer(); // new SpringServiceContainer();

		public static IServiceContainer Instance {
			get {
                return _instance;
			}
            set {
                if(_instance != null)
                {
                    throw new ApplicationException("Service container already set");
                }
                _instance = value;
            }
		}

		private readonly IApplicationContext _container;

        public static IServiceContainer CreatePlatformContainer()
        {
            // if no platform indicator then provide a standard SpringServiceContainer
            if(String.IsNullOrEmpty(ConfigurationManager.AppSettings["spring-platform"]))
            {
                return new SpringServiceContainer();
            }

            // grab platform and spring file container
            string platformConfigFilename = ConfigurationManager.AppSettings["spring-platform"];
            string springFilesSetting = String.IsNullOrEmpty(ConfigurationManager.AppSettings["spring"]) ?
                String.Empty : ConfigurationManager.AppSettings["spring"].RemoveWhitespace();

            // add the platform file to the config list
            List<string> configFilenames = new List<string>();
            configFilenames.Add(platformConfigFilename);

            // check if we have a config discovery folder
            string springFileFolder = ConfigurationManager.AppSettings["spring-folder"];

            if (!String.IsNullOrEmpty(springFileFolder))
            {
                springFileFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, springFileFolder);
                // add any .xml files in the folder into the config list
                configFilenames.AddRange(new System.IO.DirectoryInfo(springFileFolder).EnumerateFiles("*.xml").Select(fi => fi.FullName));
            }
            else
            {
                // otherwise, add the comma separated list of files into the config list
                configFilenames.AddRange(springFilesSetting.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            }

            IApplicationContext context = new XmlApplicationContext(configFilenames.ToArray());

            ContextRegistry.RegisterContext(context);

            return new SpringServiceContainer(context);
        }

		public object this[string name] {
			get {
				return this.GetObject(name);
			}
		}

		public SpringServiceContainer() {
			_container = ContextRegistry.GetContext();
		}

        public SpringServiceContainer(IApplicationContext container)
        {
            _container = container;
        }

		public void Dispose() {
			//
		}

		public object GetObject(string name) {
			return _container.GetObject(name);
		}

		public object GetObject(string name, Type type) {
			return _container.GetObject(name, type);
		}

		public T GetObject<T>(string name) {
			return (T)this.GetObject(name, typeof(T));
		}

		public bool ContainsObject(string name) {
			return _container.ContainsObject(name);
		}
		
	}
}
