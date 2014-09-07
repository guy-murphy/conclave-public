namespace Conclave.Data.Resolver {

	/// <summary>
	/// 
	/// </summary>
	public interface IResolver {
		/// <summary>
		/// The path of the application.
		/// </summary>
		string ApplicationPath { get; }
		/// <summary>
		/// The root of the resource.
		/// </summary>
		string RootPath { get; }
		/// <summary>
		/// The relative path to the resource.
		/// </summary>
		string RelativePath { get; set; }
		/// <summary>
		/// The full absolute path to the resource.
		/// </summary>
		string FullPath { get; }
		/// <summary>
		/// Determines if the resource exists.
		/// </summary>
		bool Exists { get; }
		/// <summary>
		/// Creates the resource if it doesn't exist.
		/// </summary>
		bool Create();
		/// <summary>
		/// Removes the resource.
		/// </summary>
		void Remove();
	}
}
