using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Conclave.Map.Model;

namespace Conclave.Map.Store {

	/// <summary>
	/// Reads DMoz structure expressed as RDF
	/// into a provided TopicMap.
	/// </summary>
	/// <remarks>
	/// This is a quick and dirty implementation for
	/// the purposes of bulk loading a topic store.
	/// It's not fit for any genuine purpose, but it
	/// might be a useful starting point. There's naff
	/// all error checking, and it's not reading all
	/// aspects of the RDF, it really is just bulk loading
	/// something vaguely topicmap-like.
	/// </remarks>
	public class DmozReader {

		private const string _rdf = @"http://www.w3.org/TR/RDF/";
		private const string _dc = @"http://purl.org/dc/elements/1.0/";

		protected class ReadContext : List<string> {

			public void Push(string item) {
				this.Add(item);
			}

			public string Peek() {
				return this[this.Count - 1];
			}

			// becuase this is handy on a tree context
			public string Peek(int index) {
				return ((index + this.Count) > 0) ? this[this.Count - 1 + index] : null;
			}

			public string Pop() {
				string item = this[this.Count - 1];
				this.RemoveAt(this.Count - 1);
				return item;
			}

		}

		private readonly ITopicStore _store;

		public DmozReader(ITopicStore store) {
			_store = store;
		}

		public void ReadStructure(string path) {
			_store.Start();
			try {
				ReadContext ctx = new ReadContext();
				int count = 0;
				using (XmlTextReader reader = new XmlTextReader(path)) {
					Topic.Builder topic = null;
					Association.Builder assoc = null;
					string id, resource, value, label, role;
					string[] parts;
					while (reader.Read()) {
						switch (reader.NodeType) {
							case XmlNodeType.Element:
								ctx.Push(reader.Name);
								switch (reader.Name) {
									case "Topic":
										if (topic == null) {
											id = reader.GetAttribute("id", _rdf);
											if (!String.IsNullOrWhiteSpace(id)) {
												topic = new Topic.Builder(id);
											}
										}
										break;
									case "related":
										resource = reader.GetAttribute("resource", _rdf);
										if (!String.IsNullOrWhiteSpace(resource)) {
											if (topic != null) {
												label = resource.Split('/').Last().Replace('_', ' ');
												topic.AddAssociation("default", "navigation", "related", resource, label);
											}
										}
										break;
									case "narrow":
										resource = reader.GetAttribute("resource", _rdf);
										if (!String.IsNullOrWhiteSpace(resource)) {
											if (topic != null) {
												label = resource.Split('/').Last().Replace('_', ' ');
												assoc = topic.AddAssociation("default", "category", "narrow", resource, label);
												//Association.Builder broader = new Association.Builder() 
												//{
												//	Type = "category",
												//	Role = "broader",
												//	Parent = assoc.Reference,
												//	Reference = topic.Id
												//};
												//label = topic.Id.Split('/').Last().Replace('_', ' ');
												//broader.AddMetadata("label", label);
												//_store.AddAssociation(broader);
											}
										}
										break;
									case "symbolic":
										resource = reader.GetAttribute("resource", _rdf);
										if (!String.IsNullOrWhiteSpace(resource)) {
											if (topic != null) {
												parts = resource.Split(':');
												role = parts[0];
												if (role.Length > 50) role = role.Substring(0, 50);
												label = resource.Split('/').Last().Replace('_', ' ');
												topic.AddAssociation("default", "similar", role, parts[1], label);
											}
										}
										break;
								}
								break;
							case XmlNodeType.EndElement:
								switch (reader.Name) {
									case "Topic":
										if (topic != null) {
											topic.AddOccurrence("default", "wiki", "markdown", "self", "To be or not to be that is the question.");
											try {
												_store.AddTopic(topic);
											} catch {}
											count++;
											if (count % 100 == 0) Console.WriteLine(count);
											topic = null;
										}
										break;
								}
								ctx.Pop();
								break;
							case XmlNodeType.Text:
								value = reader.Value.Replace('_', ' ');
								if (value.Length > 50) value = value.Substring(0, 50);
								switch (ctx.Peek()) {
									case "d:Title":
										if (ctx.Peek(-1) == "Topic" && topic != null) {
											topic.AddMetadata("label", value);
										}
										break;
									case "d:Description":
										if (ctx.Peek(-1) == "Topic" && topic != null) {
											topic.AddMetadata("description", value);
										}
										break;
									case "lastUpdate":
										if (ctx.Peek(-1) == "Topic" && topic != null) {
											topic.AddMetadata("last-update", value);
										}
										break;
								}
								break;
						}
					}
				}
				Console.WriteLine();
				Console.WriteLine(count);
			} finally {
				_store.Stop();
			}
		}


	}
}
