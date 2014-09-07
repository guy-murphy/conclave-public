using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json;

namespace Conclave.Parsing.Wiki {
	public class AcumenWikiParser : IData {

		/// <summary>
		/// A helper to produce labels from wiki name type ids.
		/// </summary>
		/// <param name="wikiName">The wiki name to produce a label from.</param>
		/// <returns>The label.</returns>

		public static string WikiNameToLabel(string wikiName) {
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < wikiName.Length; i++) {
				if (wikiName[i] == '_') {
					sb.Append(' ');
				} else {
					if (i != 0) { // we dont insert a space before the first letter
						if (i == wikiName.Length - 1) { // it's the last letter
							if (char.IsUpper(wikiName[i]) && char.IsLower(wikiName[i - 1]) && char.IsLetter(wikiName[i - 1])) {
								sb.Append(' ');
							}
						} else {
							if (char.IsUpper(wikiName[i]) && char.IsLower(wikiName[i + 1]) && char.IsLetter(wikiName[i - 1])) {
								sb.Append(' ');
							}
						}
					}
					sb.Append(wikiName[i]);
				}
			}
			return sb.ToString();
		}

		public static string CleanText(string text) {
			// returns
			text = text.Replace("\r", "");
			// quotes
			text = text.Replace("“", "\"");
			text = text.Replace("”", "\"");
			text = text.Replace("’", "'");
			// elipses
			text = text.Replace("…", "...");
			// dashes
			text = text.Replace("–", "-");
			// numbers
			text = text.Replace("½", "1/2");
			text = text.Replace("¼", "1/4");
			// marks
			text = text.Replace("©", "(c)");
			text = text.Replace("®", "(r)");
			return text;
		}

		public static void MakeSafeForXml(ref string text) {
			text = Regex.Replace(text, "&(?!(amp;))", "&amp;");
			text = text.Replace("<", "&lt;");
			text = text.Replace(">", "&gt;");
		}

		/// <summary>
		/// A helper to write elements to an <see cref="XmlWriter"/>.
		/// </summary>
		/// <param name="writer">The <see cref="XmlWriter"/> to write to.</param>
		/// <param name="elementName">The name of the elemeent to write.</param>
		/// <param name="className">The CSS class of the element.</param>
		/// <param name="content">The content of the element</param>
		/// <remarks>
		/// The content of the element is <b>not</b> written out as raw XML
		/// and so only regular text is safe.
		/// </remarks>

		public static void WriteElement(XmlWriter writer, string elementName, string className, string content) {
			writer.WriteStartElement(elementName);
			writer.WriteAttributeString("class", className);
			writer.WriteString(content);
			writer.WriteEndElement();
		}

		// these are the regular expressions used in splitting, macthing and recognising text.

		private static Dictionary<string, Regex> _blockPatterns;
		private static Dictionary<string, Regex> _linePatterns;
		//private static Dictionary<string, Regex> _titlePatterns;

		// characters
		private static string StrongMarkup = @"\*";
		private static string EmMarkup = @"\^";
		private static string CodeMarkup = @"`";
		//private static string SupMarkup = @"\^";
		private static string SubMarkup = @"¦";
		private static string BoxMarkup = @"\*";
		private static string QuoteMarkup = @"~";
		private static string CodeBlockMarkup = @"#";
		private static string TitleMarkup = @"\=";
		private static string TableBlockMarkup = @"\-";
		private static string FormBlockMarkup = @"\+"; //by james
		// masks
		private static string InlineMask = @"(?<element>{0}(?<text>[^{0}]+){0})";
		private static string BlockMask = @"{0}{0}({0})+.*{0}{0}({0})+$";
		private static string TitleMask = @"^(?<element>{0}{{{1},}})(?<text>[^\{0}]+)({0}{{{1},}})";
		// page
		public static Regex BlockSplit = new Regex(@"(\A|\r\n\r\n|\n\n|\Z)", RegexOptions.Multiline);
		public static Regex LineSplit = new Regex(@"\n");
		// block
		public static Regex BoxMatch = new Regex(String.Format(BlockMask, BoxMarkup), RegexOptions.Singleline | RegexOptions.Compiled);
		public static Regex QuoteMatch = new Regex(String.Format(BlockMask, QuoteMarkup), RegexOptions.Singleline | RegexOptions.Compiled);
		public static Regex CodeBlockMatch = new Regex(String.Format(BlockMask, CodeBlockMarkup), RegexOptions.Singleline | RegexOptions.Compiled);
		public static Regex TableBlockMatch = new Regex(String.Format(BlockMask, TableBlockMarkup), RegexOptions.Singleline | RegexOptions.Compiled);
		public static Regex FormBlockMatch = new Regex(String.Format(BlockMask, FormBlockMarkup), RegexOptions.Singleline | RegexOptions.Compiled); //by James
		public static Regex ListMatch = new Regex(@"($|\n)(?<level>[\*]+)[^\n\*]+(\n\n|$)", RegexOptions.Singleline | RegexOptions.Compiled);
		public static Regex ListLevelMatch = new Regex(@"(?<level>[\*]+)(?<text>[^\n]+)", RegexOptions.Singleline | RegexOptions.Compiled);
		// inline
		public static Regex StrongMatch = new Regex(String.Format(InlineMask, StrongMarkup), RegexOptions.Singleline | RegexOptions.Compiled);
		public static Regex EmMatch = new Regex(String.Format(InlineMask, EmMarkup), RegexOptions.Singleline | RegexOptions.Compiled);
		public static Regex CodeMatch = new Regex(String.Format(InlineMask, CodeMarkup), RegexOptions.Singleline | RegexOptions.Compiled);
		//public static Regex SupMatch = new Regex(String.Format(InlineMask, SupMarkup), RegexOptions.Singleline);
		public static Regex SubMatch = new Regex(String.Format(InlineMask, SubMarkup), RegexOptions.Singleline | RegexOptions.Compiled);
		// links
		public static Regex AnchorMatch = new Regex(@"(?<anchor><<((?<anchor_text>[^\|>]+)\|)?(?<anchor_name>[\w>]+)>>)", RegexOptions.Singleline | RegexOptions.Compiled);
		public static Regex LinkMatch = new Regex(@"(?<link>\[(?<link_text>((?<part>[^\|\]]+)(\|)?)+)\])", RegexOptions.Compiled);
		public static Regex MacroMatch = new Regex(@"(?<macro>\{(?<macro_name>((?<part>[^\|\}]+)(\|)?)+)\})", RegexOptions.Compiled);
		// titles
		public static Regex H1Match = new Regex(String.Format(TitleMask, TitleMarkup, 5), RegexOptions.Singleline | RegexOptions.Compiled);
		public static Regex H2Match = new Regex(String.Format(TitleMask, TitleMarkup, 4), RegexOptions.Singleline | RegexOptions.Compiled);
		public static Regex H3Match = new Regex(String.Format(TitleMask, TitleMarkup, 3), RegexOptions.Singleline | RegexOptions.Compiled);
		public static Regex H4Match = new Regex(String.Format(TitleMask, TitleMarkup, 2), RegexOptions.Singleline | RegexOptions.Compiled);
		// misc identifiers
		public static Regex ImageRef = new Regex(@"[^\.]+\.(gif|jpg|jpeg|png|svg)", RegexOptions.Compiled);
		// public static Regex UrlRef = new Regex(@"(.)*http(s)?://(.)+");

		// Regular expression modified by BK (070320) to accommodate Subversion (svn) links.
		public static Regex UrlRef = new Regex(@"(.)*(http(s)|svn)?://(.)+", RegexOptions.Compiled);
		public static Regex WikipediaRef = new Regex(@"wikipedia:", RegexOptions.Compiled);
		public static Regex TransclusionRef = new Regex(@"trans:", RegexOptions.Compiled);
		public static Regex MailRef = new Regex(@"(\S+@).+((\.com)|(\.net)|(\.edu)|(\.mil)|(\.gov)|(\.org)|(\..{2,2}))", RegexOptions.Compiled);

		// code types supported for syntax highlighting
		public static string[] CodeTypes = new string[] { "c#", "c-sharp", "csharp", "vb", "vb.net", "delphi", "pascal", "js", "jscript", "javascript", "php", "py", "python", "sql", "xml", "xhtml", "xslt", "html", "xhtml" };

		private string _text;
		private bool _processingXml;

		/// <summary>
		/// A reference dictionary of the block patterns used
		/// to process block elements.
		/// </summary>

		public static Dictionary<string, Regex> BlockPatterns {
			get {
				if (_blockPatterns == null) {
					_blockPatterns = new Dictionary<string, Regex>();
					_blockPatterns["box"] = BoxMatch;
					_blockPatterns["quote"] = QuoteMatch;
					_blockPatterns["code"] = CodeBlockMatch;
					_blockPatterns["table"] = TableBlockMatch;
					_blockPatterns["form"] = FormBlockMatch;
				}
				return _blockPatterns;
			}
		}

		/// <summary>
		/// A reference dictionary of the line patterns used
		/// to process inline elements.
		/// </summary>

		public static Dictionary<string, Regex> LinePatterns {
			get {
				if (_linePatterns == null) {
					_linePatterns = new Dictionary<string, Regex>();
					_linePatterns["strong"] = StrongMatch;
					_linePatterns["em"] = EmMatch;
					_linePatterns["code"] = CodeMatch;
					//_linePatterns["sup"] = SupMatch;
					_linePatterns["sub"] = SubMatch;
					_linePatterns["h1"] = H1Match;
					_linePatterns["h2"] = H2Match;
					_linePatterns["h3"] = H3Match;
					_linePatterns["h4"] = H4Match;
				}
				return _linePatterns;
			}
		}


		/// <summary>
		/// The input text to be processed.
		/// </summary>

		public string Text {
			get { return this._text; }
			set {
				_text = CleanText(value);
			}
		}

		private XmlWriter _writer;

		/// <summary>
		/// The <see cref="XmlTextWriter"/> to use to write the results to.
		/// </summary>

		public System.Xml.XmlWriter Writer {
			get { return this._writer; }
			set { this._writer = value; }
		}

		private Stack<string> _context;

		/// <summary>
		/// Tracks the current markup context of the process.
		/// </summary>

		public Stack<string> Context {
			get {
				if (_context == null) {
					_context = new Stack<string>();
				}
				return _context;
			}
		}

		/// <summary>
		/// Instantiates a new Wiki parser with an Empty string set as its initial text.
		/// </summary>

		public AcumenWikiParser() : this(String.Empty) { }
		public AcumenWikiParser(string text) {
			this.Text = text;
			_processingXml = false;
		}

		object ICloneable.Clone() {
			throw new NotImplementedException("This method is not yet implemented.");
		}

		/// <summary>
		/// Produces an XML output from the inputed text, processing
		/// the text as Wiki-like markup.
		/// </summary>
		/// <param name="writer">The <see cref="XmlTextWriter"/> to write the resulting XML to.</param>

		public void ToXml(XmlWriter writer) {
			if (_text != null) {
				this.Writer = writer;
				// split into blocks
				string[] blocks = BlockSplit.Split(_text);
				this.Writer.WriteStartElement("div");
				this.Writer.WriteAttributeString("class", "content");
				foreach (string block in blocks) {
					this.ProcessBlock(block);
				}
				this.Writer.WriteEndElement();
			}
		}

		public void ToJson(JsonWriter writer) {
			throw new NotImplementedException();
		}

		//public override void ToJson(Newtonsoft.Json.JsonWriter writer) {
		//	throw new NotImplementedException();
		//}

		/// <summary>
		/// Trims away blank lines from a block.
		/// </summary>
		/// <param name="lines">The block of lines to be trimmed.</param>

		protected static void TrimBlockLines(List<string> lines) {
			// trim front
			while (lines.Count > 0 && !Regex.IsMatch(lines[0], @"")) {
				lines.RemoveAt(0);
			}
			// trim back
			while (lines.Count > 0 && !Regex.IsMatch(lines[lines.Count - 1], @"")) {
				lines.RemoveAt(lines.Count - 1);
			}
		}

		/// <summary>
		/// When processing a block of lines we work from the outermost lines
		/// inward. This method snips away the first and last line of a block of
		/// lines, pressumably after they are no longer of interest.
		/// </summary>
		/// <param name="lines">A list of lines representing a block.</param>

		protected static void SnipBlockLines(List<string> lines) {
			if (lines.Count > 0) {
				lines.RemoveAt(0);
			}
			if (lines.Count > 0) {
				lines.RemoveAt(lines.Count - 1);
			}
		}

		/// <summary>
		/// Processes the beginning of a block of text for title elements.
		/// </summary>
		/// <param name="lines">A list of lines representing a block of text.</param>
		/// <remarks><b>Note:</b> For code blocks, titles are used to denote the code type.</remarks>
		/// <seealso cref="CodeTypes"/>

		protected void ProcessBlockTitle(List<string> lines) {
			while (lines.Count > 0 && lines[0].Length > 0 && lines[0][0] == '.') {
				string title = lines[0].Substring(1);
				if (!String.IsNullOrWhiteSpace(title)) { // ensure that it's not an empty title
					if (this.Context.Peek() == "code") {
						if (title == "xml" || title == "literal") {
							_processingXml = true; // we want to ensure we don't escape entities
						}
						// it's a code block that we're processing and we
						// use the title to let us know which type of code it is
						this.Writer.WriteAttributeString("name", "code");
						this.Writer.WriteAttributeString("class", title);
					} else if (this.Context.Peek() == "form") {
						this.Writer.WriteAttributeString("action", title);
					} else if (this.Context.Peek() == "table") {
						this.Writer.WriteElementString("caption", title);
					} else {
						WriteElement(this.Writer, "div", "block-title", title);
					}
				}
				lines.RemoveAt(0);
			}
		}

		/// <summary>
		/// Processes the end of a block of text for footer elements.
		/// </summary>
		/// <param name="lines">The lines comprising the block of text.</param>

		protected void ProcessBlockFoot(List<string> lines) {
			while (lines.Count > 0 && !String.IsNullOrEmpty(lines[lines.Count - 1]) && lines[lines.Count - 1][0] == '.') {
				WriteElement(Writer, "div", "foot", lines[lines.Count - 1].Substring(1));
				lines.RemoveAt(lines.Count - 1);
			}
		}

		/// <summary>
		/// Processes a piece of text as a block.
		/// </summary>
		/// <param name="blockText">The text to be processed.</param>

		protected void ProcessBlock(string blockText) {
			// if the block isn't empty
			if (blockText.Length > 0 && Regex.IsMatch(blockText, @"\w")) {
				// split into lines
				List<string> lines = new List<string>();
				lines.AddRange(LineSplit.Split(blockText));
				TrimBlockLines(lines); // remove insignificant whitespace... do we actually want to be doing this?
				// what type of block is it?
				// by default it's a simple block
				string blockType = "block";
				foreach (string name in BlockPatterns.Keys) {
					if (BlockPatterns[name].IsMatch(blockText)) {
						blockType = name;
						SnipBlockLines(lines);
					}
				}
				// start of primary block block
				if (ListMatch.IsMatch(blockText)) { // is it a list?
					blockType = "list";
					this.Writer.WriteStartElement("ul");
				} else if (blockType == "code") { // is it a code block?
					this.Writer.WriteStartElement("textarea");
				} else if (blockType == "form") { // form block
					this.Writer.WriteStartElement("form");
				} else if (blockType == "table") { // is it a table ?
					this.Writer.WriteStartElement("table");
					this.Writer.WriteAttributeString("class", "table");
				} else {
					this.Writer.WriteStartElement("div");
					this.Writer.WriteAttributeString("class", blockType);
				}
				// actual block
				this.Context.Push(blockType);
				this.ProcessBlockTitle(lines);

				// do we have a sub block
				string subText = String.Join("\n", lines.ToArray());
				if (ListMatch.IsMatch(subText) && blockType != "list") { // is it a list?
					this.ProcessBlock(subText);
					lines.Clear();
				}
				// process the lines
				foreach (string line in lines) {
					this.ProcessLine(line);
				}
				// end of block
				this.ProcessBlockFoot(lines);
				this.Writer.WriteEndElement();
				this.Context.Pop();
				// we should always flag as not processing xml
				// by the time we pop out of a code block
				// NOTE: this means it is only ever safe to use this
				// flag for code blocks
				_processingXml = false;
			}
		}

		/// <summary>
		/// Processes a piece of text as a line, and write the result to the
		/// curremt <see cref="XmlTextWriter"/>.
		/// </summary>
		/// <param name="lineText">The line of text to be processed.</param>

		protected void ProcessLine(string lineText) {
			if (!_processingXml) {
				MakeSafeForXml(ref lineText);
			}
			if (lineText.Length == 0) {
				this.Writer.WriteRaw("<br />");
			} else {
				// process inline markup
				switch (this.Context.Peek()) {
					case "code":
						// do nothing
						break;
					default:
						ProcessAnchors(ref lineText);
						ProcessLinks(ref lineText);
						ProcessMacros(ref lineText);
						break;
				}
				if (lineText[0] != '.') {
					// start of line
					switch (this.Context.Peek()) {
						case "list":
							//this.Writer.WriteStartElement("item");
							//this.Writer.WriteAttributeString("level", StripLevelFromListItem(ref lineText));
							this.Writer.WriteStartElement("li");
							StripLevelFromListItem(ref lineText);
							break;
						case "code":
							// do nothing
							// we don't want to wrap code lines in elements
							break;
						case "table":
							this.Writer.WriteStartElement("tr");
							break;
						case "form":
							this.Writer.WriteStartElement("div");
							this.Writer.WriteAttributeString("class", "line");
							break;
						default:
							this.Writer.WriteStartElement("div");
							this.Writer.WriteAttributeString("class", "line");
							break;
					}
					// actual line output
					switch (this.Context.Peek()) {
						case "code":
							this.Writer.WriteRaw(lineText);
							break;
						case "table":
							string[] cells = lineText.Split(':');
							for (int i = 0; i < cells.Length; i++) {
								string tag = (i == 0) ? "th" : "td";
								this.Writer.WriteStartElement(tag);
								this.Writer.WriteRaw(cells[i]);
								this.Writer.WriteEndElement();
							}
							break;
						case "form":
							string[] column = lineText.Split(':');
							//make sure it is long enough
							if (column.Length == 4) {
								this.Writer.WriteStartElement("label");
								this.Writer.WriteAttributeString("for", column[1]);
								this.Writer.WriteRaw(column[0]);
								this.Writer.WriteEndElement();

								if (column[2] == "select") { //type of text select
									this.Writer.WriteStartElement("select");
									this.Writer.WriteAttributeString("name", column[1]);
									this.Writer.WriteAttributeString("selected", column[3]);
									//need to create sub elements from xsl
									this.Writer.WriteEndElement();
								} else if (column[2] == "textarea") { //type of text area
									this.Writer.WriteStartElement("textarea");
									this.Writer.WriteAttributeString("name", column[1]);
									this.Writer.WriteRaw(column[3]);
									this.Writer.WriteEndElement();
								} else {
									this.Writer.WriteStartElement("input");
									this.Writer.WriteAttributeString("type", column[2]);
									this.Writer.WriteAttributeString("name", column[1]);
									this.Writer.WriteAttributeString("value", column[3]);
									this.Writer.WriteEndElement();
								}
							} else { // We're just gonner output the raw text if there aren't enough fields
								this.Writer.WriteStartElement("span");
								this.Writer.WriteAttributeString("class", "description");
								this.Writer.WriteRaw(column[0]);
								this.Writer.WriteEndElement();
							}
							break;
						default:
							MarkupLine(ref lineText);
							this.Writer.WriteRaw(lineText);
							break;
					}
					// end of line
					switch (this.Context.Peek()) {
						case "code":
							this.Writer.WriteString("\n");
							break;
						case "table":
							this.Writer.WriteEndElement();
							break;
						case "form":
							this.Writer.WriteEndElement();
							break;
						default:
							this.Writer.WriteEndElement();
							break;
					}
				}
			}
		}

		/// <summary>
		/// Processes a line for anchors.
		/// </summary>
		/// <param name="line">The reference to the line of text to be changed.</param>

		protected static void ProcessAnchors(ref string line) {
			line = AnchorMatch.Replace(line, delegate(Match match) { // delegate to handle inline replacement of matches
				Group anchorName = match.Groups["anchor_name"];
				Group anchorText = match.Groups["anchor_text"];
				string name = anchorName.Value;
				if (anchorText.Success) {
					return String.Format("<a href=\"#{0}\">{1}</a>", name, anchorText.Value);
				} else {
					return String.Format("<a name=\"{0}\"></a>", name);
				}
			});
		}

		/// <summary>
		/// Processes a line for links.
		/// </summary>
		/// <param name="line">The reference to the line of text to be changed.</param>

		protected static void ProcessLinks(ref string line) {
			// safety check to prevent uneuqal brackets from crashing
			// the regular expression engine
			int openCount = 0;
			int closeCount = 0;
			foreach (char c in line) {
				if (c == '[') {
					openCount++;
				} else if (c == ']') {
					closeCount++;
				}
			}
			// the actual regular expression processing
			if ((openCount > 0 && closeCount > 0) && (openCount == closeCount)) {
				line = LinkMatch.Replace(line, delegate(Match match) { // delegate to handle inline replacement of matches
					Group parts = match.Groups["part"];

					string reference = (parts.Captures.Count == 1) ? parts.Captures[0].Value : parts.Captures[parts.Captures.Count - 1].Value;

					string link = null;
					if (parts.Captures.Count == 1) {
						// this is a simple link with no additional parameters
						if (ImageRef.IsMatch(reference)) {
							if (UrlRef.IsMatch(reference)) { // this is an external image
								link = String.Format("<img src=\"{0}\" class=\"external-image\" />", reference);
							} else { // this is an internal/local image
								link = String.Format("<img src=\"$ImageBaseUrl${0}\" class=\"content-image\" />", reference);
							}
						} else if (WikipediaRef.IsMatch(reference)) {
							string wikiName = reference.Substring(10);
							string wikiUrl = String.Format("http://en.wikipedia.org/wiki/{0}", wikiName);
							string template = "<a href=\"#\" onClick=\"Behaviour.Transclusion(this); return false\" url=\"{1}\"	parameters=\"\" target=\"container::{0}\" completion=\"new Effect.Highlight(targetObj)\">wikipedia: {0}</a><div class=\"wikipedia-container\" id=\"container::{0}\"></div>";
							link = String.Format(template, WikiNameToLabel(wikiName), wikiUrl);
						} else if (TransclusionRef.IsMatch(reference)) {
							string transName = reference.Substring(6);
							string transUrl = transName;
							string template = "<a href=\"#\" onClick=\"Behaviour.Transclusion(this); return false\" url=\"{1}\"	parameters=\"\" target=\"container::{0}\" completion=\"new Effect.Highlight(targetObj)\">fetch: {0}</a><div class=\"wikipedia-container\" id=\"container::{0}\"></div>";
							link = String.Format(template, transName, transUrl);
						} else if (UrlRef.IsMatch(reference)) {
							link = String.Format("<a href=\"{0}\">{0}</a>", reference);
						} else if (MailRef.IsMatch(reference)) {
							link = String.Format("<a href=\"mailto:{0}\">{0}</a>", reference);
						} else {
							if (reference.Contains("::")) {
								// this reference contains a map specification to use
								string[] referenceParts = reference.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
								if (referenceParts.Length == 2) {
									string mapSpec = referenceParts[0];
									reference = referenceParts[1];
									link = String.Format("<a href=\"$TopicBaseUrl${0}&amp;!map={2}\">{1}</a>", reference, WikiNameToLabel(reference), mapSpec);
								}
							} else {
								// this reference contains no map specification
								link = String.Format("<a href=\"$TopicBaseUrl${0}\">{1}</a>", reference, WikiNameToLabel(reference));
							}
						}
					} else {
						// this is a complex link with additional parameters
						if (ImageRef.IsMatch(reference)) {
							link = String.Format("<img src=\"$ImageBaseUrl${0}\" class=\"content-image\" align=\"{1}\" alt=\"{2}\" title=\"{2}\" />", reference, parts.Captures[1].Value, parts.Captures[0].Value);
						} else if (UrlRef.IsMatch(reference)) {
							//Edited by J.Graham
							if (parts.Captures.Count > 0) {
								link = String.Format("<a href=\"{0}\" target=\"{2}\">{1}</a>", reference, parts.Captures[0].Value, parts.Captures[1].Value);
							} else {
								link = String.Format("<a href=\"{0}\">{1}</a>", reference, parts.Captures[0].Value);
							}
						} else if (MailRef.IsMatch(reference)) {
							link = String.Format("<a href=\"mailto:{0}\">{1}</a>", reference, parts.Captures[0].Value);
						} else if (WikipediaRef.IsMatch(reference)) {
							string wikiName = reference.Substring(10);
							string wikiUrl = String.Format("http://en.wikipedia.org/wiki/{0}", wikiName);
							string wikiLabel = parts.Captures[0].Value;
							string template = "<a href=\"#\" onClick=\"Behaviour.Transclusion(this); return false\" url=\"{1}\"	parameters=\"\" target=\"container::{0}\" completion=\"new Effect.Highlight(targetObj)\">wikipedia: {2}</a><div class=\"wikipedia-container\" id=\"container::{0}\"></div>";
							link = String.Format(template, WikiNameToLabel(wikiName), wikiUrl, wikiLabel);
						} else if (TransclusionRef.IsMatch(reference)) {
							string transName = parts.Captures[0].Value;
							string transUrl = reference.Substring(6);
							string template = "<a href=\"#\" onClick=\"Behaviour.Transclusion(this); return false\" url=\"{1}\"	parameters=\"\" target=\"container::{0}\" completion=\"new Effect.Highlight(targetObj)\">fetch: {0}</a><div class=\"wikipedia-container\" id=\"container::{0}\"></div>";
							link = String.Format(template, transName, transUrl);
						} else {
							if (reference.Contains("::")) {
								// this reference contains a map specification to use
								string[] referenceParts = reference.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
								if (referenceParts.Length == 2) {
									string mapSpec = referenceParts[0];
									reference = referenceParts[1];
									link = String.Format("<a href=\"$TopicBaseUrl${0}&amp;!map={2}\">{1}</a>", reference, parts.Captures[0].Value, mapSpec);
								}
							} else {
								link = String.Format("<a href=\"$TopicBaseUrl${0}\">{1}</a>", reference, parts.Captures[0].Value);
							}
						}
					}
					return link;
				});
			} else if (openCount > 0) {
				line = "<b>[unsafe link not displayed]</b>";
			}
		}

		/// <summary>
		/// Processes the line provided for macros.
		/// </summary>
		/// <param name="line">The reference to the line of text to be changed.</param>
		/// <remarks>This facility may become depracated as Wikis now support StringTemplate processing.</remarks>

		protected static void ProcessMacrosOld(ref string line) {
			line = MacroMatch.Replace(line, delegate(Match match) { // delegate to handle inline replacement of macros
				Group parts = match.Groups["part"];
				string macroName = (parts.Captures.Count == 1) ? parts.Captures[0].Value : parts.Captures[parts.Captures.Count - 1].Value;
				string result;
				if (parts.Captures.Count == 1) { // it's a simple macro
					result = String.Format("<macro name=\"{0}\" />", macroName);
				} else { // the macro has parameters
					string[] parms = new string[parts.Captures.Count - 1];
					for (int i = 0; i < parts.Captures.Count - 1; i++) {
						parms[i] = parts.Captures[i].Value;
					}
					result = String.Format("<macro name=\"{0}\" parameters=\"{1}\" />", macroName, String.Join("|", parms));
				}
				return result;
			});
		}

		protected static void ProcessMacros(ref string line) {
			line = MacroMatch.Replace(line, (Match m) =>
			{
				Group parts = m.Groups["part"];
				string macroName = (parts.Captures.Count == 1) ? parts.Captures[0].Value : parts.Captures[parts.Captures.Count - 1].Value;
				string result;
				if (parts.Captures.Count == 1) { // it's a simple macro
					result = String.Format("<macro name=\"{0}\" />", macroName);
				} else { // the macro has parameters
					string[] parms = new string[parts.Captures.Count - 1];
					for (int i = 0; i < parts.Captures.Count - 1; i++) {
						parms[i] = parts.Captures[i].Value;
					}
					result = String.Format("<macro name=\"{0}\" parameters=\"{1}\" />", macroName, String.Join("|", parms));
				}
				return result;
			});
		}

		/// <summary>
		/// Strips the list notation from a line of text and determines the depth of the list element.
		/// </summary>
		/// <param name="line">The reference to the line of text to be changed.</param>
		/// <returns>The depth of the list element.</returns>

		protected static string StripLevelFromListItem(ref string line) {
			Match match = ListLevelMatch.Match(line);
			line = match.Groups["text"].Value;
			return match.Groups["level"].Length.ToString();
		}

		/// <summary>
		/// Marks up a line of text transforming it from Wiki notation to XML.
		/// This is achieved by matching the line against the patterns in <see cref="LinePatterns"/>,
		/// and only marks up inline elements. Block level markup is handled as a different process.
		/// </summary>
		/// <param name="line">The reference to the line of text to be marked up.</param>

		protected static void MarkupLine(ref string line) {
			foreach (string name in LinePatterns.Keys) {
				line = LinePatterns[name].Replace(line, String.Concat("<", name, ">${text}</", name, ">"));
			}
		}

	}
}
