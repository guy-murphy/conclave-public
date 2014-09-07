using System.Collections.Generic;
using System.Linq;
using Conclave.Collections;

namespace Conclave.Parsing.Wiki.Model.Model {
	public class ListElement: BlockElement {

		private int _level;

		public int Level {
			get { return _level; }
			set { _level = value; }
		}

		public ListElement(): base("list") {
			
		}

		// not sure what is happening with this method in terms of usage
		public override void Complete() {
			// copy the current list sideways
			// to process and put back
			DataCollection<WikiElement> content = new DataCollection<WikiElement>(this.Children);
			this.ClearChildren();
			Stack<ListElement> context = new Stack<ListElement>();
			context.Push(this); // this is the root element

			foreach (WikiElement element in content) {
				ListItemElement item = element as ListItemElement;
				if (item != null) {
					if (item.Level > context.Count) { // we are entering a new list
						ListElement sub = new ListElement();
						context.Peek().AddChild(sub);
						context.Push(sub);
					} else if (item.Level < context.Count) { // we're leaving a sub list to its parent
						context.Pop();
					}
					context.Peek().AddChild(element);
				}
			}
		}

		/// <summary>
		/// Process the flat list of child items into a
		/// tree of sub lists. This is much easier (for me)
		/// to do here than bending the formal parser
		/// inot arcane shapes.
		/// </summary>
		public void PostProcess() {
			// copy the children sideways
			// and reprocess them into sub lists
			DataCollection<WikiElement> children = new DataCollection<WikiElement>(this.Children);
			Stack<ListElement> lists = new Stack<ListElement>();
			this.ClearChildren(); // move to after the processes

			int currentItemLevel, currentListLevel; // helps with debugging, the routine has been made deliberately verbose
			lists.Push(this); // this is the root element
			foreach (WikiElement element in children) {
				ListItemElement currentItem = element as ListItemElement;
				if (currentItem != null) {
					currentItemLevel = currentItem.Level;
					currentListLevel = lists.Count - 1;
					// do we need to pop any old lists?
					while (currentItemLevel < currentListLevel) {
						lists.Pop();
						currentListLevel--;
					}
					// do we need a new list?
					if (currentItemLevel > currentListLevel) {
						ListElement deeperList = new ListElement();
						// if there is an item in the current list add the new list to that
						// else add the new list to the current list
						if (lists.Peek().Children.Any()) {
							lists.Peek().Children.Last().AddChild(deeperList); // this makes unsafe assuptions, change Children to be a stack
						} else {
							lists.Peek().AddChild(deeperList);
						}
						lists.Push(deeperList);
						currentListLevel++;
					}
					lists.Peek().AddChild(currentItem);
				}
			}
		}

	}
}
