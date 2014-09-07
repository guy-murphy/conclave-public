using System.Collections.Generic;
using Conclave.Data.Store;
using Conclave.Map.Model;

namespace Conclave.Map.Store {
	public interface ITopicStore : IStore {
		void RemoveMetadataFor(string parent);
		void RemoveMetadata(string parent, string scope, string name);
		void AddMetadata(IEnumerable<Metadata> metadata);
		void AddMetadata(Metadata meta);
		void SetMetdadata(string parent, string name, string value);
		void SetMetadata(string parent, string scope, string name, string value);
		void SetMetadata(string parent, string scope, string name, string value, bool check);
		/// <summary>
		/// Updates the metadata for all associations pointing
		/// at the provided id.
		/// </summary>
		/// <remarks>This is useful for updating the labels of pointing assocs.</remarks>
		/// <param name="id"></param>
		/// <param name="assocType">The type of assocs to consider.</param>
		/// <param name="assocRole">The role of assocs to consider.</param>
		/// <param name="metaScope">The scope of metadata to consider.</param>
		/// <param name="metaName">The name of metadata to consider.</param>
		/// <param name="metaValue">The value to set for the metadata.</param>
		void UpdateMetadataForPointingAssociations(
			string id,
			string assocType, string assocRole,
			string metaScope, string metaName, string metaValue
			);
		Metadata GetMetadata(string parent, string name);
		Metadata GetMetadata(string parent, string scope, string name);
		IEnumerable<Metadata> GetMetadataFor(string parent);
		void RemoveOccurrencesFor(string parent);
		void RemoveOccurrence(string parent, string scope, string role, string behaviour, string reference);
		void AddOccurrences(IEnumerable<Occurrence> occurrences);
		void AddOccurrence(Occurrence occurrence);
		void SetOccurrence(string parent, string role, string behaviour, string reference);
		Occurrence GetOccurrence(string parent, string role, string behaviour, string reference);

		/// <summary>
		/// Returns any occurrence that matches the provided parameters; or, returns a blank occurrence.
		/// </summary>
		/// <param name="parent">The identity of the parent Topic to which the occurrence belongs.</param>
		/// <param name="scope">The scope of the occurrence.</param>
		/// <param name="role">The role that the occurrence performs.</param>
		/// <param name="behaviour">The behaviour cited to resolve the occurrence.</param>
		/// <param name="reference">The reference to the occurrence backing data to be resolved by the cited behaviour.</param>
		/// <returns>Returns either the occurrence matching the provided parameters; or, returns a blank occurrence.</returns>
		/// <remarks>It is API breaking for this method to return a null reference.</remarks>
		Occurrence GetOccurrence(string parent, string scope, string role, string behaviour, string reference);

		bool OccurrenceExists(string parent, string scope, string role, string behaviour, string reference);
		IEnumerable<Occurrence> GetOccurrencesFor(string parent);

		/// <summary>
		/// Gets all the association ids in the store.
		/// </summary>
		/// <returns>
		/// Returns an an enumerable of all the association ids.
		/// </returns>
		/// <remarks>
		/// I'm really not sure this is a smart idea. It's
		/// a disaster just waiting to happen.
		/// </remarks>
		/// TODO: Breaking. Remove.
		IEnumerable<string> GetAssociationIds();

		/// <summary>
		/// Checks whether the specified association exists in the store.
		/// </summary>
		/// <param name="id">The identity of the association to check for.</param>
		/// <returns>Returns <b>true</b> if the association exists; otherwise returns <b>false</b>.</returns>
		bool AssociationExists(string id);

		Association GetAssociation(string id);
		void RemoveAssociationsFor(string parent);
		void RemovePointingAssociations(string reference);
		void RemoveAssociation(string id);
		void AddAssociations(IEnumerable<Association> associations);
		void AddAssociation(Association association);
		void CreateAssociation(string id, string scope, string type, string role, string parent, string reference);

		/// <summary>
		/// Checks whether the specified topic exists in the store.
		/// </summary>
		/// <param name="topicId">The identity of the topic to check for.</param>
		/// <returns>Returns <b>true</b> if the topic exists; otherwise returns <b>false</b>.</returns>
		bool TopicExists(string topicId);

		/// <summary>
		/// Gets all the topic ids in the store.
		/// </summary>
		/// <returns>
		/// Returns an an enumerable of all the topic ids.
		/// </returns>
		/// <remarks>
		/// I'm really not sure this is a smart idea. It's
		/// a disaster just waiting to happen.
		/// </remarks>
		/// TODO: Breaking. Remove.
		IEnumerable<string> GetTopicIds();

		/// <summary>
		/// Creates a topic with the specified Id.
		/// </summary>
		/// <param name="id">The identity of the topic to create.</param>
		/// <returns>Returns the newly created <see cref="Topic"/></returns>
		/// <exception cref="TopicStoreException">
		/// It is an error to attempt to create a topic that already exists.
		/// </exception>
		void CreateTopic(string id);

		/// <summary>
		/// Removes the specified topic from the store.
		/// </summary>
		/// <param name="topicId">The identity of the topic to remove.</param>
		void RemoveTopic(string topicId);

		/// <summary>
		/// Adds a topic to the store.
		/// </summary>
		/// <param name="topic">The topic to add to the store.</param>
		void AddTopic(Topic topic);

		Topic GetTopic(string id);

		/// <summary>
		/// Gets a topic of the specified identity,
		/// with its members filtered by the scope
		/// provided.
		/// </summary>
		/// <param name="id">The identity of the topic to get.</param>
		/// <param name="scope">
		/// The scope to filter the topics members by.
		/// Spcifying the scope as null indicates no filtering
		/// will take place.
		/// </param>
		/// <returns>
		/// Returns the topic found with the identity provided,
		/// or returns <see cref="Topic.Blank"/>. This method
		/// should not return null.
		/// </returns>
		Topic GetTopic(string id, string scope);
	}
}