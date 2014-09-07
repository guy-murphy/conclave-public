using System;
using System.Collections.Generic;
using System.Linq;

namespace Conclave.Process.User {
	public static class GlobalRoleEx {

		public static IEnumerable<GlobalRole> GetRoles(this GlobalRole self) {
			return Enum.GetValues(self.GetType()).Cast<GlobalRole>().Where(r => self.HasFlag(r) && Convert.ToInt64(r) != 0);
		}

		public static string[] GetRoleNames(this GlobalRole self) {
			return self.GetRoles().Select(r => r.ToString()).ToArray();
		}
	}
}
