using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace Conclave.Map.Model {
	public sealed class Association: Node, IData, IEquatable<Association> {

		public static bool operator ==(Association a1, Association a2) {
			if (Object.ReferenceEquals(a1, a2)) return true;
			if (((object)a1 == null) || ((object)a2 == null)) return false;
			return a1.Equals(a2);
		}

		public static bool operator !=(Association a1, Association a2) {
			return !(a1 == a2);
		}

		private static readonly Association _blank = new Association(String.Empty, String.Empty, String.Empty, String.Empty, String.Empty);

		/// <summary>
		/// Represents an empty association item as
		/// an alternative to a <b>null</b> value;
		/// </summary>
		public static Association Blank {
			get {
				return _blank;
			}
		}

		private readonly string _parent;
		private readonly string _type;
		private readonly string _scope;
		private readonly string _reference;
		private readonly string _role;

		private int _hashcode = 0;

		public string Parent {
			get { return _parent; }
		}

		public string Type {
			get { return _type; }
		}

		public string Scope {
			get { return _scope; }
		}

		public string Reference {
			get { return _reference; }
		}

		public string Role {
			get { return _role; }
		}

		public Association(string parent, string type, string reference, string role) : this(parent, type, "default", reference, role) { }
		public Association(string parent, string type, string scope, string reference, string role) : this(null, parent, type, scope, reference, role) { }
		public Association(IEnumerable<Metadata> meta, string parent, string type, string scope, string reference, string role) : this(Guid.NewGuid().ToString(), meta, parent, type, scope, reference, role) { }		

		public Association(Association assoc) : this(assoc.Id, assoc.Metadata, assoc.Parent, assoc.Type, assoc.Scope, assoc.Reference, assoc.Role) { }

		public Association(string id, IEnumerable<Metadata> meta, string parent, string type, string scope, string reference, string role): base(id, meta) {
			_parent = parent;
			_type = type;
			_scope = scope;
			_reference = reference;
			_role = role;
		}

		object ICloneable.Clone() {
			return new Association(this);
		}

		public Association Clone() {
			return new Association(this);
		}

		public override bool Equals(object obj) {
			Association other = obj as Association;
			return (other != null) && this.Equals(other);
		}

		public bool Equals(Association other) {
			if (Object.ReferenceEquals(this, other)) return true;
			return (
				base.Equals(other) &&
				this.Parent == other.Parent &&
				this.Type == other.Type &&
				this.Scope == other.Scope &&
				this.Reference == other.Reference &&
				this.Role == other.Role
			);
		}

		public override int GetHashCode() {
			if (_hashcode == 0) {
				_hashcode = base.GetHashCode();
				_hashcode = _hashcode * 31 + this.Parent.GetHashCode();
				_hashcode = _hashcode * 31 + this.Type.GetHashCode();
				_hashcode = _hashcode * 31 + this.Scope.GetHashCode();
				_hashcode = _hashcode * 31 + this.Reference.GetHashCode();
				_hashcode = _hashcode * 31 + this.Role.GetHashCode();
			}
			return _hashcode;
		}

		public Association Mutate(Func<Builder, Association> mutator) {
			Builder builder = new Builder(this);
			return mutator(builder);
		}

		public override void ToXml(XmlWriter writer) {
			writer.WriteStartElement("association");
			writer.WriteAttributeString("parent", this.Parent);
			writer.WriteAttributeString("type", this.Type);
			writer.WriteAttributeString("scope", this.Scope);
			writer.WriteAttributeString("reference", this.Reference);
			writer.WriteAttributeString("role", this.Role);
			base.ContentToXml(writer);
			writer.WriteEndElement();
		}

		public override void ToJson(JsonWriter writer) {
			writer.WriteStartObject();
			writer.WritePropertyName("_type");
			writer.WriteValue("association");
			writer.WritePropertyName("parent");
			writer.WriteValue(this.Parent);
			writer.WritePropertyName("type");
			writer.WriteValue(this.Type);
			writer.WritePropertyName("scope");
			writer.WriteValue(this.Scope);
			writer.WritePropertyName("reference");
			writer.WriteValue(this.Reference);
			writer.WritePropertyName("role");
			writer.WriteValue(this.Role);
			base.ContentToJson(writer);
			writer.WriteEndObject();
		}

		/// <summary>
		/// The builder for <see cref="Association"/>, providing a mutable equivalent
		/// of the concrete, immutable model.
		/// </summary>
		public new class Builder : Node.Builder {

			public static implicit operator Association(Builder builder) {
				return builder.ToAssociation();
			}

			public static implicit operator Builder(Association assoc) {
				return new Builder(assoc);
			}

			public static ImmutableHashSet<Association> CreateImmutableCollection(IEnumerable<Association.Builder> assocs) {
				HashSet<Association> temp = new HashSet<Association>();
				if (assocs != null) {
					foreach (Association item in assocs) {
						temp.Add(item);
					}
				}
				return ImmutableHashSet.Create<Association>(temp.ToArray());
			}

			public string Parent { get; set; }
			public string Type { get; set; }
			public string Scope { get; set; }
			public string Reference { get; set; }
			public string Role { get; set; }

			public Builder() : base() {
				this.Scope = "default";
			}
			public Builder(Association assoc) {
				this.FromAssociation(assoc);
			}

			public Builder(string parent, string type, string scope, string reference, string role) : base() {
				this.Parent = parent;
				this.Type = type;
				this.Scope = scope;
				this.Reference = reference;
				this.Role = role;
			}

			public Builder(IEnumerable<Metadata.Builder> meta, string parent, string type, string scope, string reference, string role): base(meta) {
				this.Parent = parent;
				this.Type = type;
				this.Scope = scope;
				this.Reference = reference;
				this.Role = role;
			}

			public Builder(string id, string parent, string type, string scope, string reference, string role) : this(id, null, parent, type, scope, reference, role) { }

			public Builder(string id, IEnumerable<Metadata.Builder> meta, string parent, string type, string scope, string reference, string role)
				: base(id, meta) {
					this.Parent = parent;
					this.Type = type;
					this.Scope = scope;
					this.Reference = reference;
					this.Role = role;
			}

			public Builder FromAssociation(Association assoc) {
				base.FromNode(assoc);
				this.Parent = assoc.Parent;
				this.Type = assoc.Type;
				this.Scope = assoc.Scope;
				this.Reference = assoc.Reference;
				this.Role = assoc.Role;
				return this;
			}

			public Association ToAssociation() {
				return new Association(this.Id, this.Metadata.ToImmutable(), this.Parent, this.Type, this.Scope, this.Reference, this.Role);
			}

			public override Metadata.Builder AddMetadata(string name, string value) {
				return this.AddMetadata(this.Scope, name, value);
			}

			public override Metadata.Builder AddMetadata(string scope, string name, string value) {
				//if (this.Scope != scope) throw new InvalidOperationException("You may not add metadata to an Association with a different scope.");

				return base.AddMetadata(scope, name, value);
			}
		}
	}
}
