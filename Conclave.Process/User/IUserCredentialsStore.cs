using Conclave.Data.Store;

namespace Conclave.Process.User {
	public interface IUserCredentialsStore: IStore {
		bool UserCredentialsExist(string id);
		bool UserCredentialsExist(string name, string pwd);
		UserCredentials GetUserCredentials(string id);
		UserCredentials GetUserCredentials(string name, string pwd);
		void CreateUserCredentials(string id, string name, string email, string pwd, long mask);
		void RemoveUserCredentials(string id);
		void UpdateEmail(string id, string email);
		void UpdatePassword(string id, string pwd);
		void UpdateMask(string id, long mask);
	}
}