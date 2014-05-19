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

namespace Tiny_ToDo {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		ListViewModel Model = new ListViewModel();
		String filePath = "todolist.txt";


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
			using (StreamWriter r = new StreamWriter(File.OpenWrite(filePath))) {
				foreach (Item i in Model.ItemList) {
					r.WriteLine(i.ItemName.Replace("|", "&SEP&") + "|" + i.IsDone.ToString() + "|" + ((int)i.Priority).ToString());
				}
			}
		}

		private void List_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
			Item item = ((ListView)sender).SelectedItem as Item;

			if (item == null) return;

			System.Windows.Controls.ContextMenu menu = new ContextMenu();

			MenuItem d = new MenuItem() { Header = "Delete Item" };
			d.Click += (o, a) => {
				RemoveItem(item);
			};

			menu.Items.Add(d);
			MenuItem p = new MenuItem() { Header = "Set Priority"};

			MenuItem n = new MenuItem() { Header = "Normal", IsChecked = (item.Priority == Item.PriorityState.Normal)};
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
			(sender as TextBox).Focusable = true;
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			AddItem();
		}

		private void RemoveItem(Item r) {
			int pos = Model.ItemList.IndexOf(r);
			if (pos == Model.ItemList.Count - 1)
				pos--;
			else
				pos++;

			if (pos >= 0)
				List.SelectedIndex = pos;

			Model.ItemList.Remove(r);
		}

		private void AddItem() {
			Model.ItemList.Add(new Item() { ItemName = "New Item" });

			List.SelectedIndex = Model.ItemList.Count - 1;
		}

		private void Window_Closed(object sender, EventArgs e) {
			WriteFile();
		}

	}



}
