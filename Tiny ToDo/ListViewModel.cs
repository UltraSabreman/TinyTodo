using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiny_ToDo {

	public class PropChange : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPorpertyChanged(String property) {
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(property));
		}
	}

	public class ListViewModel : PropChange {
		private ObservableCollection<Item> itemList;
		public ObservableCollection<Item> ItemList { get { return itemList; } set { itemList = value; OnPorpertyChanged("ItemList"); } }


		private int selected;
		public int Selected { get { return selected; } set { selected = value; OnPorpertyChanged("Selected"); } }
		
	}
}
