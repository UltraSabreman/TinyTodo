using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Tiny_ToDo {


	public class PirorityToForegroundBrush : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			Item.PriorityState p = (Item.PriorityState)value;
			switch (p) {
				case Item.PriorityState.Normal: return Brushes.Black;
				case Item.PriorityState.Low: return Brushes.Green;
				case Item.PriorityState.Medium: return Brushes.Orange;
				case Item.PriorityState.High: return Brushes.Red;
				default: return Brushes.Black;
			}

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			Brush p = (Brush)value;
			if (p == Brushes.Black)
				return Item.PriorityState.Normal;
			else if (p == Brushes.Green)
				return Item.PriorityState.Low;
			else if (p == Brushes.Orange)
				return Item.PriorityState.Medium;
			else if (p == Brushes.Red)
				return Item.PriorityState.High;
			else
				return Item.PriorityState.Normal;
		}
	}

	public class PriorityToBackground : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			Item.PriorityState p = (Item.PriorityState)value;
			switch (p) {
				case Item.PriorityState.Normal: return Brushes.Silver;
				case Item.PriorityState.Low: return Util.ColorToBrush("80FF80");
				case Item.PriorityState.Medium: return Util.ColorToBrush("FFCB80");
				case Item.PriorityState.High: return Util.ColorToBrush("FF8080");
				default: return Brushes.Silver;
			}

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
