using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Conclave.Map.Model {
	public static class EnumerableEx {
		public static ImmutableHashSet<Metadata> ToImmutable(this IEnumerable<Metadata.Builder> self) {
			return Metadata.Builder.CreateImmutableCollection(self);
		}

		public static ImmutableHashSet<Occurrence> ToImmutable(this IEnumerable<Occurrence.Builder> self) {
			return Occurrence.Builder.CreateImmutableCollection(self);
		}

		public static Occurrence GetOccurrence(this IEnumerable<Occurrence> self,  string scope, string role, string behaviour, string reference) {
			return
				self.FirstOrDefault(o => o.Scope == scope && o.Role == role && o.Behaviour == behaviour && o.Reference == reference);
		}

		public static ImmutableHashSet<Association> ToImmutable(this IEnumerable<Association.Builder> self) {
			return Association.Builder.CreateImmutableCollection(self);
		}
	}
}
