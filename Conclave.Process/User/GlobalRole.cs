using System;

namespace Conclave.Process.User {

	// this could be a ulong but I'm not sure of the support over a range of backing stores, 
	// we loose a bit in the name of caution, if you need more bits for roles
	// you're already ripping this apart or ignoring it for your own thing

	[Flags]
	public enum GlobalRole: long { 
		anon = 0,
		user = 1,
		trusted = 2,
		moderator = 4,
		admin = 8,
		developer = 16,
		owner = 32,

		all = anon | user | trusted | moderator | admin | developer | owner
	}
}
