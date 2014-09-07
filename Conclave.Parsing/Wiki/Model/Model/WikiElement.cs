using System;
using System.Collections.Generic;
using System.Xml;
using Conclave.Collections;
using Newtonsoft.Json;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class WikiElement : IData, IEquatable<WikiElement> {

		public static bool operator ==(WikiElement e1, WikiElement e2) {
			if (Object.ReferenceEquals(e1, e2)) return true;
			if (((object)e1 == null) || ((object)e2 == null)) return false;
			return e1.Equals(e2);
		}

		public static bool operator !=(WikiElement e1, WikiElement e2) {
			return !(e1 == e2);
		}

		private readonly string _original;
		private readonly DataCollection<WikiElement> _children;
		private WikiElement _parent;

		public string Original {
			get { return _original ?? String.Empty; }
		}

		public IEnumerable<WikiElement> Children {
			get { return _children; }
			//set { _children = value; }
		}

		public WikiElement Parent {
			get { return _parent; }
			set {
				if (this.Parent != null) {
					this.Parent.RemoveChild(this);
				}
				_parent = value;
			}
		}

		public WikiElement() : this(String.Empty) {}
		public WikiElement(string original) : this(original, null, null) {}
		public WikiElement(WikiElement element) : this(element.Original, element.Parent, element.Children) {}

		public WikiElement(string original, WikiElement parent, params WikiElement[] children) {
			_original = original;
			_parent = parent;
			_children = (children == null) ? new DataCollection<WikiElement>() : new DataCollection<WikiElement>(children);
		}

		public WikiElement(string original, WikiElement parent, IEnumerable<WikiElement> children) {
			_original = original;
			_parent = parent;
			_children = (children == null) ? new DataCollection<WikiElement>() : new DataCollection<WikiElement>(children);
		}
		
		public override bool Equals(object obj) {
			WikiElement other = obj as WikiElement;
			return (other != null) && this.Equals(other);
		}

		public bool Equals(WikiElement other) {
			if (object.ReferenceEquals(this, other)) return true;
			//return (
			//	this.Parent == other.Parent &&
			//	this.Original == other.Original &&
			//	this.Children.SequenceEqual(other.Children)
			//);
			return this.Original == other.Original;
		}

		/// <summary>
		/// Returns the hashcode for this WikiElement.
		/// </summary>
		/// <returns>Rturns an int as the hash for this element.</returns>
		/// <remarks>
		/// This is a pretty terrible hash production. It's a mutable
		/// class, and so shouldn't be relying upon a hash anyway. It's here for
		/// completeness, not because it's a good idea.
		/// </remarks>
		public override int GetHashCode() {
			int hash = "WikiElement".GetHashCode();
			hash = hash*31 + this.Parent.GetHashCode();
			hash = hash*31 + this.Original.GetHashCode();
			foreach (WikiElement child in this.Children) {
				hash = hash*31 + child.GetHashCode();
			}
			return hash;
		}

		public WikiElement Clone() {
			return new WikiElement(this);
		}

		object ICloneable.Clone() {
			return this.Clone();
		}

		public WikiElement AddChild(WikiElement element) {
			element.Parent = this;
			_children.Add(element);
			return element;
		}

		public void AddChildren(IEnumerable<WikiElement> children) {
			foreach (WikiElement child in children) {
				this.AddChild(child);
			}
		}

		public WikiElement RemoveChild(WikiElement element) {
			_children.Remove(element);
			return element;
		}

		public void ClearChildren() {
			_children.Clear();
		}

		public virtual void Complete() {
			// nothin to do in most cases
		}

		public override string ToString() {
			return String.IsNullOrWhiteSpace(this.Original) ? base.ToString() : this.Original;
		}

		public virtual void ContextToXml(XmlWriter writer) {
			foreach (WikiElement child in Children) {
				child.ToXml(writer);
			}
		}

		public virtual void ToXml(XmlWriter writer) {
			writer.WriteStartElement("wiki-element");
			this.ContextToXml(writer);
			writer.WriteEndElement();
		}

		public void ToJson(JsonWriter writer) {
			throw new NotImplementedException();
		}
	}
}
