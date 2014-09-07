using System.Security.Principal;

namespace Conclave.Process.User {
	public interface IUserCredentials : IData, IIdentity {
		string Id { get; }
		// string Name { get; } is present on IIdentity
		string Password { get; }
		long Mask { get; }
		string Email { get; }
	}
}