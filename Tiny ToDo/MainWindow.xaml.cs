using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.IO;
using System.Windows.Threading;

namespace Tiny_ToDo {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		ListViewModel Model = new ListViewModel();
		String filePath = "todolist.txt";
		bool shiftHeld = false;
		bool editing = false;

		public MainWindow() {
			InitializeComponent();
			ObservableCollection<Item> ItemList = new ObservableCollection<Item>();

			List.DataContext = Model;
			Model.ItemList = ItemList;

			ReadFile();

		}

		public void ReadFile() {
			if (!File.Exists(filePath)) return;
			try {
				using (StreamReader r = File.OpenText(filePath)) {
					String line;
					while ((line = r.ReadLine()) != null) {
						String [] stuff = line.Split('|');

						Model.ItemList.Add(new Item() { ItemName = stuff [0].Replace("&SEP&", "|"), IsDone = Boolean.Parse(stuff [1]), Priority = (Item.PriorityState)Int32.Parse(stuff [2]) });
					}
				}
			} catch (Exception) {
				MessageBoxResult r = MessageBox.Show("Failed to read the todo list file.\nClick ok to continue or cancel to exit.", "Error reading file", MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.OK);
				if (r == MessageBoxResult.Cancel)
					Application.Current.Shutdown();
			}

		}

		public void WriteFile() {
			File.Delete(filePath);

			using (StreamWriter r = new StreamWriter(File.OpenWrite(filePath))) {
				foreach (Item i in Model.ItemList) {
					r.WriteLine(i.ItemName.Replace("|", "&SEP&") + "|" + i.IsDone.ToString() + "|" + ((int)i.Priority).ToString().Trim());
				}
			}
		}

		private void List_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			Item item = ((ListView)sender).SelectedItem as Item;

			if (item == null) return;

			System.Windows.Controls.ContextMenu menu = new ContextMenu();

			MenuItem clear = new MenuItem() { Header = "Remove Finished" };
			clear.Click += (o, a) => {
				List<Item> toRemove = new List<Item>();
				foreach (Item i in List.Items)
					if (i.IsDone)
						toRemove.Add(i);

				toRemove.ForEach(X => List.Items.Remove(X));
			};
			menu.Items.Add(clear);
			menu.Items.Add(new Separator());

			MenuItem d = new MenuItem() { Header = "Delete Item" };
			d.Click += (o, a) => {
				RemoveItem(item);
			};

			menu.Items.Add(d);
			MenuItem p = new MenuItem() { Header = "Set Priority" };

			MenuItem n = new MenuItem() { Header = "Normal", IsChecked = (item.Priority == Item.PriorityState.Normal) };
			n.Click += (o, a) => {
				item.Priority = Item.PriorityState.Normal;
			};
			p.Items.Add(n);

			MenuItem l = new MenuItem() { Header = "Low", IsChecked = (item.Priority == Item.PriorityState.Low) };
			l.Click += (o, a) => {
				item.Priority = Item.PriorityState.Low;
			};
			p.Items.Add(l);

			MenuItem m = new MenuItem() { Header = "Medium", IsChecked = (item.Priority == Item.PriorityState.Medium) };
			m.Click += (o, a) => {
				item.Priority = Item.PriorityState.Medium;
			};
			p.Items.Add(m);

			MenuItem h = new MenuItem() { Header = "High", IsChecked = (item.Priority == Item.PriorityState.High) };
			h.Click += (o, a) => {
				item.Priority = Item.PriorityState.High;
			};
			p.Items.Add(h);

			menu.Items.Add(p);
			menu.IsOpen = true;
		}

		private void TextBox_KeyUp(object sender, KeyEventArgs e) {
			TextBox box = sender as TextBox;
			if (e.Key == Key.Enter) {
				box.Focusable = false;
			}

		}

		private void TextBox_LostFocus(object sender, RoutedEventArgs e) {
			editing = false;
			(sender as TextBox).Focusable = true;
			WriteFile();
		}

		private void ItemBox_GotFocus(object sender, RoutedEventArgs e) {
			editing = true;
		}


		private void Button_Click(object sender, RoutedEventArgs e) {
			AddItem();
			EditItem();
		}

		private void RemoveItem(Item r) {
			int pos = Model.ItemList.IndexOf(r);
			if (pos == Model.ItemList.Count - 1)
				pos--;
			else
				pos++;

			if (pos >= 0)
				Model.Selected = pos;

			Model.ItemList.Remove(r);
			WriteFile();
		}

		private void AddItem() {
			Model.ItemList.Add(new Item() { ItemName = "New Item" });

			Model.Selected = Model.ItemList.Count - 1;
			WriteFile();
		}

		private void EditItem() {
			ItemContainerGenerator generator = List.ItemContainerGenerator;
			ListViewItem selectedItem = (ListViewItem)generator.ContainerFromIndex(Model.Selected);
			TextBox tbFind = GetDescendantByType(selectedItem, typeof(TextBox), "ItemBox") as TextBox;
			if (tbFind != null) {
				tbFind.Focus();
				tbFind.SelectAll();
				editing = true;
				WriteFile();
			}
		}

		private void Window_Closed(object sender, EventArgs e) {
			WriteFile();
		}

		private void List_KeyUp(object sender, KeyEventArgs e) {
			Item item = ((ListView)sender).SelectedItem as Item;
			int index = Model.ItemList.IndexOf(item);

			if (e.Key == Key.LeftShift || e.Key == Key.RightShift) {
				shiftHeld = false;
				return;
			}

			if (editing) return;
			switch(e.Key) {
				case Key.Delete:
				case Key.D:
					RemoveItem(item);
				break;

				case Key.E:
					EditItem();
				break;

				case Key.A:
					AddItem();
					EditItem();
				break;

				case Key.Space:
					item.IsDone = !item.IsDone;
				break;

				case Key.NumPad0:
				case Key.D0:
					item.Priority = Item.PriorityState.Normal;
				break;

				case Key.NumPad1:
				case Key.D1:
					item.Priority = Item.PriorityState.Low;
				break;

				case Key.NumPad2:
				case Key.D2:
					item.Priority = Item.PriorityState.Medium;
				break;

				case Key.NumPad3:
				case Key.D3:
					item.Priority = Item.PriorityState.High;
				break;

				case Key.Up:
					if (shiftHeld && index != -1) {
						int newPos = (index - 1 >= 0 ? index - 1 : (Model.ItemList.Count - 1));
						Item temp = Model.ItemList[newPos];
						Model.ItemList[newPos] = item;
						Model.ItemList[index] = temp;
						Model.Selected = newPos;
						//List.SelectedIndex = newPos;
					}
				break;

				case Key.Down:
					if (shiftHeld && index != -1) {
						int newPos = (index + 1 < Model.ItemList.Count ? index + 1 : 0);
						Item temp = Model.ItemList [newPos];
						Model.ItemList [newPos] = item;
						Model.ItemList [index] = temp;
						Model.Selected = newPos;

						//List.SelectedIndex = newPos;
					}
				break;
			}
			WriteFile();
		}

		private void List_KeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
				shiftHeld = true;
		}
		//found here:
		//http://social.msdn.microsoft.com/Forums/vstudio/en-US/1e9a98c3-b4be-40af-99bd-828491ddcd69/set-focus-of-textbox-in-listview?forum=wpf
		public static Visual GetDescendantByType(Visual element, Type type, string name) {
			if (element == null) return null;
			if (element.GetType() == type) {
				FrameworkElement fe = element as FrameworkElement;
				if (fe != null && fe.Name == name) {
					return fe;
				}
			}

			Visual foundElement = null;
			if (element is FrameworkElement)
				(element as FrameworkElement).ApplyTemplate();

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++) {
				Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
				foundElement = GetDescendantByType(visual, type, name);

				if (foundElement != null)
					break;
			}
			return foundElement;
		}

		private void List_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
			EditItem();
		}

		private void testChecker_Checked(object sender, RoutedEventArgs e) {
			WriteFile();
		}

	}

}
