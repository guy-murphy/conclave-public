using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conclave.Data.Resolver {

	/// <summary>
	/// Provides all the functionality of a <see cref="FileResolver"/>
	/// but in addition will help find suitable templates within
	/// a directory structure.
	/// </summary>
	public class TemplateFileResolver: FileResolver {

		protected TemplateFileResolver(string root) : base(root) { }
		protected TemplateFileResolver(string applicationPath, string root): base(applicationPath, root) {	}



	}
}
