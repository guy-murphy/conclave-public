using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace Conclave.Map.Model {
	public sealed class Occurrence : IData, IEquatable<Occurrence> {

		public static bool operator ==(Occurrence o1, Occurrence o2) {
			if (Object.ReferenceEquals(o1, o2)) return true;
			if (((object) o1 == null) || ((object) o2 == null)) return false;
			return o1.Equals(o2);
		}

		public static bool operator !=(Occurrence o1, Occurrence o2) {
			return !(o1 == o2);
		}

		private static readonly Occurrence _blank = new Occurrence( String.Empty, String.Empty, String.Empty, String.Empty, String.Empty);

		public static Occurrence Blank {
			get {
				return _blank;
			}
		}

		private static readonly Encoding _encoding = Encoding.UTF8;

		public static Encoding StringEncoding {
			get { return _encoding; }
		}

		private readonly string _parent;
		private readonly string _scope;
		private readonly string _role;
		private readonly string _behaviour;
		private readonly string _reference;
		private readonly byte[] _data;

		private int _hashcode = 0;
		private string _stringData;
		private volatile IData _resolvedModel;
		
		public string Parent {
			get {
				return _parent;
			}
		}

		public string Scope {
			get {
				return _scope;
			}
		}

		public string Role {
			get { return _role; }
		}

		public string Behaviour {
			get {
				return _behaviour;
			}
		}

		public string Reference {
			get {
				return _reference;
			}
		}

		public byte[] Data {
			get { return _data; }
		}

		public bool HasData {
			get { return _data != null; }
		}

		public string StringData {
			get {
				if (this.HasData && _stringData == null) {
					_stringData = Occurrence.StringEncoding.GetString(this.Data);
				}
				return _stringData ?? String.Empty;
			}
		}

		public IData ResolvedModel {
			get { return _resolvedModel; }
			set {
				if (_resolvedModel == null) {
					_resolvedModel = value;
				} else {
					throw new InvalidOperationException("You may only assign to ResolvedModel the once. Thereafter it is readonly.");
				}
			}
		}

		public Occurrence(): this(Occurrence.Blank) {}

		public Occurrence(string parent, string role, string behaviour, string reference) : this(parent, "default", role, behaviour, reference) { }

		public Occurrence(Occurrence occurrence) : this(occurrence.Parent, occurrence.Scope, occurrence.Role, occurrence.Behaviour, occurrence.Reference, occurrence.Data) { }

		public Occurrence(string parent, string scope, string role, string behaviour, string reference, string data): this(parent, scope, role, behaviour, reference) {
			_data = Occurrence.StringEncoding.GetBytes(data);
		}

		public Occurrence(string parent, string scope, string role, string behaviour, string reference, byte[] data = null) {
			if (parent == null) throw new ArgumentNullException("parent");
			if (scope == null) throw new ArgumentNullException("scope");
			if (role == null) throw new ArgumentNullException("role");
			if (behaviour == null) throw new ArgumentNullException("behaviour");
			if (reference == null) throw new ArgumentNullException("reference");

			_parent = parent;
			_scope = scope;
			_role = role;
			_behaviour = behaviour;
			_reference = reference;
			_data = data;
		}

		object ICloneable.Clone() {
			return new Occurrence(this);
		}

		public Occurrence Clone() {
			return new Occurrence(this);
		}

		public override bool Equals(object obj) {
			Occurrence other = obj as Occurrence;
			return (other != null) && this.Equals(other);
		}

		public bool Equals(Occurrence other) {
			if (Object.ReferenceEquals(this, other)) return true;
			return (
					this.Parent == other.Parent &&
					this.Scope == other.Scope &&
					this.Role == other.Role &&
					this.Behaviour == other.Behaviour &&
					this.Reference == other.Reference
				);
		}

		public override int GetHashCode() {
			if (_hashcode == 0) {
				_hashcode = 1;
				_hashcode = _hashcode * 31 + "Occurrence::".GetHashCode();
				_hashcode = _hashcode * 31 + this.Parent.GetHashCode();
				_hashcode = _hashcode * 31 + this.Scope.GetHashCode();
				_hashcode = _hashcode * 31 + this.Role.GetHashCode();
				_hashcode = _hashcode * 31 + this.Behaviour.GetHashCode();
				_hashcode = _hashcode * 31 + this.Reference.GetHashCode();
			}
			return _hashcode;
		}

		public Occurrence Mutate(Func<Builder, Occurrence> mutator) {
			Builder builder = new Builder(this);
			return mutator(builder);
		}

		public void ToXml(XmlWriter writer) {
			writer.WriteStartElement("occurrence");
			writer.WriteAttributeString("for", this.Parent);
			writer.WriteAttributeString("scope", this.Scope);
			writer.WriteAttributeString("role", this.Role);
			writer.WriteAttributeString("behaviour", this.Behaviour);
			writer.WriteAttributeString("reference", this.Reference);
			writer.WriteStartElement("string-data");
			writer.WriteAttributeString("encoding", Occurrence.StringEncoding.EncodingName);
			writer.WriteCData(this.StringData);
			writer.WriteEndElement(); // string data
			writer.WriteStartElement("resolved-model");
			if (this.ResolvedModel != null) this.ResolvedModel.ToXml(writer);
			writer.WriteEndElement(); // resolved-model
			writer.WriteEndElement(); // occurrence
		}

		public void ToJson(JsonWriter writer) {
			writer.WriteStartObject();
			writer.WritePropertyName("_type");
			writer.WriteValue("occurrence");
			writer.WritePropertyName("for");
			writer.WriteValue(this.Parent);
			writer.WritePropertyName("scope");
			writer.WriteValue(this.Scope);
			writer.WritePropertyName("role");
			writer.WriteValue(this.Role);
			writer.WritePropertyName("behaviour");
			writer.WriteValue(this.Behaviour);
			writer.WritePropertyName("reference");
			writer.WriteValue(this.Reference);
			writer.WritePropertyName("string-data");
			writer.WriteValue(this.StringData);
			writer.WritePropertyName("resolved-model");
			if (this.ResolvedModel != null) {
				this.ResolvedModel.ToJson(writer);
			} else {
				writer.WriteUndefined();
			}
			writer.WriteEndObject();
		}

		public class Builder {

			public static implicit operator Occurrence(Builder builder) {
				return builder.ToOccurrence();
			}

			public static implicit operator Builder(Occurrence occurrence) {
				return new Builder(occurrence);
			}

			public static ImmutableHashSet<Occurrence> CreateImmutableCollection(IEnumerable<Occurrence.Builder> occurrence) {
				HashSet<Occurrence> temp = new HashSet<Occurrence>();
				if (occurrence != null) {
					foreach (Occurrence item in occurrence) {
						temp.Add(item);
					}
				}
				return ImmutableHashSet.Create<Occurrence>(temp.ToArray());
			}

			public string Parent { get; set; }
			public string Scope { get; set; }
			public string Role { get; set; }
			public  string Behaviour { get; set; }
			public string Reference { get; set; }
			public byte[] Data { get; set; }

			public string StringData {
				get {
					if (this.Data != null) {
						return Occurrence.StringEncoding.GetString(this.Data);
					} else {
						return String.Empty;
					}
				}
				set {
					if (value == null) throw new ArgumentNullException("value");
					this.Data = Occurrence.StringEncoding.GetBytes(value);
				}
			}

			public Builder() { }
			public Builder(string parent, string role, string behaviour, string reference): this(parent, "default", role, behaviour, reference) { }

			public Builder(Occurrence occurrence) {
				this.FromOccurrence(occurrence);
			}

			public Builder(string parent, string scope, string role, string behaviour, string reference) {
				this.Parent = parent;
				this.Scope = scope;
				this.Role = role;
				this.Behaviour = behaviour;
				this.Reference = reference;
			}

			public Builder FromOccurrence(Occurrence occurrence) {
				this.Parent = occurrence.Parent;
				this.Scope = occurrence.Scope;
				this.Role = occurrence.Role;
				this.Behaviour = occurrence.Behaviour;
				this.Reference = occurrence.Reference;
				this.Data = occurrence.Data;
				return this;
			}

			public Occurrence ToOccurrence() {
				return new Occurrence(this.Parent, this.Scope, this.Role, this.Behaviour, this.Reference, this.Data);
			}
		}

	}
}
