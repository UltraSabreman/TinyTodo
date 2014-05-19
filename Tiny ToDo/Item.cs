using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiny_ToDo {
	public class Item : PropChange {
		public enum PriorityState { Normal, Low, Medium, High };

		private PriorityState priority;
		public PriorityState Priority { get { return priority; } set { priority = value; OnPorpertyChanged("Priority"); } }

		private bool isDone;
		public bool IsDone { get { return isDone; } set { isDone = value; OnPorpertyChanged("IsDone"); } }

		private String itemName;
		public String ItemName { get { return itemName; } set { itemName = value; OnPorpertyChanged("ItemName"); } }
	}

}
