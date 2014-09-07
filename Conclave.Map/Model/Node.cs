using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml;
using Newtonsoft.Json;

namespace Conclave.Map.Model {

	/// <summary>
	/// The base class for entities with a strong identity
	/// and metadata.
	/// </summary>
	public abstract class Node {
		private readonly string _id;
		private readonly ImmutableHashSet<Metadata> _meta;

		private int _hashcode = 0;

		public string Id {
			get {
				return _id;
			}
		}

		public IEnumerable<Metadata> Metadata {
			get {
				return _meta;
			}
		}

		public string Label {
			get {
				Metadata label = this.Metadata.FirstOrDefault(m => m.Name == "label");
				return label == null ? this.Id : label.Value;
			}
		}
		
		protected Node(Node node) : this(node.Id, node.Metadata) { }

		protected Node(string id, IEnumerable<Metadata> meta) {
			_id = id;
			_meta = (meta == null) ? ImmutableHashSet.Create<Metadata>() : ImmutableHashSet.Create<Metadata>(meta.ToArray());
		}

		protected Node(string id, IEnumerable<Metadata.Builder> meta) {
			_id = id;
			_meta = meta.ToImmutable();
		}

		protected Node(string id) {
			_id = id;
			_meta = ImmutableHashSet.Create<Metadata>();
		}

		public override bool Equals(object obj) {
			Node other = obj as Node;
			return (other != null) && this.Equals(other);
		}

		public bool Equals(Node other) {
			if (Object.ReferenceEquals(this, other)) return true;
			return (
				this.Id == other.Id &&
				this.Metadata.SequenceEqual(other.Metadata)
			);
		}

		public override int GetHashCode() {
			if (_hashcode == 0) {
				_hashcode = "Node".GetHashCode();
				_hashcode = _hashcode * 31 + this.Id.GetHashCode();
				foreach (Metadata meta in this.Metadata) {
					_hashcode = _hashcode + 31 + meta.GetHashCode();
				}
			}
			return _hashcode;
		}

		protected void ContentToXml(XmlWriter writer) {
			writer.WriteAttributeString("id", this.Id);
			writer.WriteAttributeString("label", this.Label);
			writer.WriteStartElement("metadata");
			foreach (Metadata meta in this.Metadata) {
				meta.ToXml(writer);
			}
			writer.WriteEndElement();
		}

		public virtual void ToXml(XmlWriter writer) {
			writer.WriteStartElement("node");
			this.ContentToXml(writer);
			writer.WriteEndElement();
		}

		protected void ContentToJson(JsonWriter writer) {
			writer.WritePropertyName("id");
			writer.WriteValue(this.Id);
			writer.WritePropertyName("label");
			writer.WriteValue(this.Label);
			writer.WritePropertyName("metadata");
			writer.WriteStartArray();
			foreach (Metadata metadata in this.Metadata) {
				metadata.ToJson(writer);
			}
			writer.WriteEndArray();
		}

		public virtual void ToJson(JsonWriter writer) {
			writer.WriteStartObject();
			writer.WritePropertyName("_type");
			writer.WriteValue("node");
			this.ContentToJson(writer);
			writer.WriteEndObject();
		}
		
		public class Builder {
			public  string Id	{ get; set; }
			public  HashSet<Metadata.Builder> Metadata { get; set; }

			protected Builder() : this(Guid.NewGuid().ToString(), null) { }
			protected Builder(Node.Builder node) : this(node.Id, node.Metadata) { }
			protected Builder(IEnumerable<Metadata.Builder> metadata) : this(Guid.NewGuid().ToString(), metadata) { }
			protected Builder(string id) : this(id, null) { }

			protected Builder(string id, IEnumerable<Metadata.Builder> metadata) {
				this.Id = id;
				this.Metadata = (metadata == null) ? new HashSet<Metadata.Builder>() : new HashSet<Metadata.Builder>(metadata);
			}

			protected Builder FromNode(Node node) {
				this.Id = node.Id;
				this.Metadata = new HashSet<Metadata.Builder>();
				foreach (Metadata meta in node.Metadata) {
					this.Metadata.Add(meta);
				}
				return this;
			}

			public virtual Metadata.Builder AddMetadata(string name, string value) {
				return this.AddMetadata("default", name, value);
			}

			public virtual Metadata.Builder AddMetadata(string scope, string name, string value) {
				if (String.IsNullOrWhiteSpace(this.Id)) throw new InvalidOperationException("You maye not add metadata to a node which does not yet have an Id.");

				Metadata.Builder meta = new Metadata.Builder(this.Id, scope, name, value);
				this.Metadata.Add(meta);
				return meta;
			}

			public void AddMetadata(Metadata.Builder metadata) {
				this.AddMetadata(metadata.Scope, metadata.Name, metadata.Value);
			}
		}
	}
}
