using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace Conclave.Process.User {
	public sealed class UserCredentials : IUserCredentials, IEquatable<UserCredentials> {

		public static bool operator ==(UserCredentials u1, UserCredentials u2) {
			if (Object.ReferenceEquals(u1, u2)) return true;
			if (((object)u1 == null) || ((object)u2 == null)) return false;
			return u1.Equals(u2);
		}

		public static bool operator !=(UserCredentials u1, UserCredentials u2) {
			return !(u1 == u2);
		}

		private static readonly UserCredentials _blank = new UserCredentials();

		public static UserCredentials Blank {
			get {
				return _blank;
			}
		}

		private readonly string _id;	
		private readonly string _name;
		private readonly string _email; // this is a convenience, but is perhaps inappropriate here
		// whether or not _pwd is a hash is implementation specific
		// while remembering that storing clear-text passwords in a database
		// makes the little baby jesus cry
		private readonly string _pwd;
		private readonly long _mask;

		private int _hashcode = 0;

		public string Id {
			get { return _id; }
		}

		public string Name {
			get { return _name; }
		}

		public string Email {
			get { return _email; }
		}

		/// <summary>
		/// A password used in conjunction with a name
		/// for authentication. Whether or not this is a hash is
		/// implementation specific, while remembering that
		/// storing clear-text passwords in a database
		/// makes the Little Baby Jesus cry.
		/// </summary>
		public string Password {
			get { return _pwd; }
		}

		public long Mask {
			get { return _mask; }
		}

		public string AuthenticationType {
			get { return "Conclave"; }
		}

		public bool IsAuthenticated {
			get { return true; }
		}

		public UserCredentials() {
			_id = "anon";
			_name = "anon";
			_email = String.Empty;
			_pwd = String.Empty;
			_mask = 0;
		}

		public UserCredentials(string name, string email, string pwd, long roles)
			: this(Guid.NewGuid().ToString(), name, email, pwd, roles) {}

		public UserCredentials(IUserCredentials credentials)
			: this(credentials.Id, credentials.Name, credentials.Email, credentials.Password, credentials.Mask) {}

		public UserCredentials(string id, string name, string email, string pwd, long mask) {
			_id = id;
			_name = name;
			_email = email;
			_pwd = pwd;
			_mask = mask;
		}

		object ICloneable.Clone() {
			return new UserCredentials(this);
		}

		public UserCredentials Clone() {
			return new UserCredentials(this);
		}

		public override bool Equals(object obj) {
			UserCredentials other = obj as UserCredentials;
			return (other != null) && this.Equals(other);
		}

		public bool Equals(UserCredentials other) {
			if (Object.ReferenceEquals(this, other)) return true;
			return (
					this.Id == other.Id &&
					this.Name == other.Name &&
					this.Email == other.Email &&
					this.Password == other.Password &&
					this.Mask== other.Mask
				);
		}

		public override int GetHashCode() {
			// the use of a mutable _hash here is safe as
			// we're dealing with an immutable structure
			// once we have the hash neither it nor
			// the structure can change, we're memoizing
			if (_hashcode == 0) {
				_hashcode = 1;
				_hashcode = _hashcode * 31 + "UserCredentials::".GetHashCode();
				_hashcode = _hashcode * 31 + this.Id.GetHashCode();
				_hashcode = _hashcode * 31 + this.Name.GetHashCode();
				_hashcode = _hashcode * 31 + this.Email.GetHashCode();
				// if the other information items are visible in the system
				// including the password hash here can potentially allow
				// sniffing for collisions, so if it's a banking app you're building
				// this isn't appropriate... in most of the rest of the world this
				// is the very least of your concerns, I note it merely for those it would matter to
				_hashcode = _hashcode * 31 + this.Password.GetHashCode();
				_hashcode = _hashcode * 31 + this.Mask.GetHashCode();
			}
			return _hashcode;
		}

		public UserCredentials Mutate(Func<Builder, UserCredentials> mutator) {
			Builder builder = new Builder(this);
			return mutator(builder);
		}

		public void ToXml(XmlWriter writer) {
			writer.WriteStartElement("credentials");
			writer.WriteAttributeString("id", this.Id);
			writer.WriteAttributeString("name", this.Name);
			// in theory it should be safe to expose the password
			// as XML to the render process, but it practice given how
			// easily data is dumped by Conclave as XML it's probably
			// not a good idea to expose the password here in this way
			// writer.WriteAttributeString("pwd", this.Password);
			writer.WriteAttributeString("mask", this.Mask.ToString());
			writer.WriteEndElement();
		}

		public void ToJson(JsonWriter writer) {
			writer.WriteStartObject();
			writer.WritePropertyName("_type");
			writer.WriteValue("credentials");
			writer.WritePropertyName("id");
			writer.WriteValue(this.Id);
			writer.WritePropertyName("name");
			writer.WriteValue(this.Name);
			writer.WritePropertyName("mask");
			writer.WriteValue(this.Mask.ToString());
			writer.WriteEndObject();
		}

		public class Builder {

			public static implicit operator UserCredentials(Builder builder) {
				return builder.ToUserCredentials();
			}

			public static implicit operator Builder(UserCredentials credentials) {
				return new Builder(credentials);
			}

			public static ImmutableHashSet<UserCredentials> CreateImmutableCollection(IEnumerable<UserCredentials.Builder> credentials) {
				HashSet<UserCredentials> temp = new HashSet<UserCredentials>();
				if (credentials != null) {
					foreach (UserCredentials item in credentials) {
						temp.Add(item);
					}
				}
				return ImmutableHashSet.Create<UserCredentials>(temp.ToArray());
			}

			public string Id { get; set; }
			public string Name { get; set; }
			public string Email { get; set; }
			public string Password { get; set; }
			public long Mask { get; set; }

			public Builder() {}
			public Builder(UserCredentials credentials) {
				this.FromUserCredentials(credentials);
			}

			public Builder(string id, string name, string email, string password, long mask) {
				this.Id = id;
				this.Name = name;
				this.Email = email;
				this.Password = password;
				this.Mask = mask;
			}

			public Builder FromUserCredentials(UserCredentials credentials) {
				this.Id = credentials.Id;
				this.Name = credentials.Name;
				this.Email = credentials.Email;
				this.Password = credentials.Password;
				this.Mask = credentials.Mask;

				return this;
			}

			public UserCredentials ToUserCredentials() {
				return new UserCredentials(this.Id, this.Name, this.Email, this.Password, this.Mask);
			}
		}
	}
}
