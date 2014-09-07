using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using System.Xml;
using Newtonsoft.Json;

namespace Conclave.Map.Model {

	/// <summary>
	/// Model for a resolved topic with members.
	/// </summary>
	public class Topic : Node, IData, IEquatable<Topic> {

		public static bool operator ==(Topic t1, Topic t2) {
			if (Object.ReferenceEquals(t1, t2)) return true;
			if (((object)t1 == null) || ((object)t2 == null)) return false;
			return t1.Equals(t2);
		}

		public static bool operator !=(Topic t1, Topic t2) {
			return !(t1 == t2);
		}

		private static readonly Topic _blank = new Topic(String.Empty, null, null, null);

		public static Topic Blank {
			get {
				return _blank;
			}
		}
		
		private readonly ImmutableHashSet<Association> _assocs;
		private readonly ImmutableHashSet<Occurrence> _occurrences;

		private int _hashcode = 0;

		/// <summary>
		/// The Associations that belong to this topic.
		/// </summary>
		public IEnumerable<Association> Associations {
			get {
				return _assocs;
			}
		}

		/// <summary>
		/// The occurrences that belong to this topic.
		/// </summary>
		public IEnumerable<Occurrence> Occurrences {
			get {
				return _occurrences;
			}
		}

		public Topic() : this(null, null, null) { }
		public Topic(string id) : this(id, null, null, null) { }
		public Topic(IEnumerable<Occurrence> occurrences) : this(null, null, occurrences) { }
		public Topic(IEnumerable<Association> assocs, IEnumerable<Occurrence> occurrences) : this(null, assocs, occurrences) { }
		public Topic(IEnumerable<Metadata> meta, IEnumerable<Association> assocs, IEnumerable<Occurrence> occurrences) : this(Guid.NewGuid().ToString(), meta, assocs, occurrences) { }

		public Topic(Topic topic) : this(topic.Id, topic.Metadata, topic.Associations, topic.Occurrences) { }

		public Topic(string id, IEnumerable<Metadata> meta, IEnumerable<Association> assocs, IEnumerable<Occurrence> occurrences)
			: base(id, meta) {
			_assocs = (assocs == null) ? ImmutableHashSet.Create<Association>() : ImmutableHashSet.Create<Association>(assocs.ToArray());
			_occurrences = (occurrences == null) ? ImmutableHashSet.Create<Occurrence>() : ImmutableHashSet.Create<Occurrence>(occurrences.ToArray());
		}

		object ICloneable.Clone() {
			return new Topic(this);
		}

		/// <summary>
		/// Clones the topic, copying all values to the new topic
		/// but maintaining no references to the topic topic.
		/// </summary>
		/// <returns>Returns a new topic, copied from this one.</returns>
		public Topic Clone() {
			return new Topic(this);
		}

		public override bool Equals(object obj) {
			Topic other = obj as Topic;
			return (other != null) && this.Equals(other);
		}

		public bool Equals(Topic other) {
			if (Object.ReferenceEquals(this, other)) return true;
			return (
				base.Equals(other) &&
				this.Associations.SequenceEqual(other.Associations) &&
				this.Occurrences.SequenceEqual(other.Occurrences)
			);
		}

		public override int GetHashCode() {
			if (_hashcode == 0) {
				_hashcode = base.GetHashCode();
				foreach (Association assoc in this.Associations) {
					_hashcode = _hashcode * 31 + assoc.GetHashCode();
				}
				foreach (Occurrence occur in this.Occurrences) {
					_hashcode = _hashcode * 31 + occur.GetHashCode();
				}
			}
			return _hashcode;
		}

		public Topic Mutate(Func<Builder, Topic> mutator) {
			Builder builder = new Builder(this);
			return mutator(builder);
		}

		public override void ToXml(XmlWriter writer) {
			writer.WriteStartElement("topic");
			base.ContentToXml(writer);
			writer.WriteStartElement("associations");
			foreach (Association assoc in this.Associations) {
				assoc.ToXml(writer);
			}
			writer.WriteEndElement();
			writer.WriteStartElement("occurrences");
			foreach (Occurrence occur in this.Occurrences) {
				occur.ToXml(writer);
			}
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		public override void ToJson(JsonWriter writer) {
			writer.WriteStartObject();
			writer.WritePropertyName("_type");
			writer.WriteValue("topic");
			base.ContentToJson(writer);
			writer.WritePropertyName("associations");
			writer.WriteStartArray();
			foreach (Association assoc in this.Associations) {
				assoc.ToJson(writer);
			}
			writer.WriteEndArray();
			writer.WritePropertyName("occurrences");
			writer.WriteStartArray();
			foreach (Occurrence occur in this.Occurrences) {
				occur.ToJson(writer);
			}
			writer.WriteEndArray();
			writer.WriteEndObject();
		}

		public override string ToString() {
			return this.ToJson();
		}

		public new class Builder : Node.Builder {

			public static implicit operator Topic(Builder builder) {
				return builder.ToTopic();
			}

			public static implicit operator Builder(Topic topic) {
				return new Builder(topic);
			}

			public HashSet<Association.Builder> Associations { get; set; }
			public HashSet<Occurrence.Builder> Occurrences { get; set; }

			public Builder() : base() { }
			public Builder(Topic topic) {
				this.FromTopic(topic);
			}
			public Builder(string id) : this(id, null, null, null) { }
			public Builder(string id, IEnumerable<Metadata.Builder> meta, IEnumerable<Association.Builder> assocs, IEnumerable<Occurrence.Builder> occurences)
				: base(id, meta) {
				this.Associations = (assocs == null) ? new HashSet<Association.Builder>() : new HashSet<Association.Builder>(assocs);
				this.Occurrences = (occurences == null) ? new HashSet<Occurrence.Builder>() : new HashSet<Occurrence.Builder>(occurences);
			}

			public Builder FromTopic(Topic topic) {
				base.FromNode(topic);
				this.Associations = new HashSet<Association.Builder>();
				foreach (Association assoc in topic.Associations) {
					this.Associations.Add(assoc);
				}
				this.Occurrences = new HashSet<Occurrence.Builder>();
				foreach (Occurrence occur in topic.Occurrences) {
					this.Occurrences.Add(new Occurrence.Builder(occur));
				}
				return this;
			}

			public Topic ToTopic() {
				return new Topic(this.Id, this.Metadata.ToImmutable(), this.Associations.ToImmutable(), this.Occurrences.ToImmutable());
			}

			public Occurrence.Builder AddOccurrence(string role, string behaviour, string text) {
				return this.AddOccurrence("default", role, behaviour, "self", text);
			}

			public Occurrence.Builder AddOccurrence(string scope, string role, string behaviour, string reference, string text) {
				Occurrence.Builder occur = new Occurrence.Builder(this.Id, scope, role, behaviour, reference)
				{
					StringData = text
				};
				this.Occurrences.Add(occur);
				return occur;
			}

			public Association.Builder AddAssociation(string scope, string type, string role, string reference, string label) {
				Association.Builder assoc = new Association.Builder()
				{
					Parent = this.Id,
					Scope = scope,
					Type = type,
					Role = role,
					Reference = reference
				};
				assoc.AddMetadata("label", label);
				this.Associations.Add(assoc);
				return assoc;
			}

		}

	}
}
