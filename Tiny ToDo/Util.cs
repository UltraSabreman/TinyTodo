using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tiny_ToDo {
	static class Util {
		public static Brush ColorToBrush(String HexColor) {
			return (Brush)new BrushConverter().ConvertFromString((HexColor.StartsWith("#") ? "" : "#") + HexColor);
		}

	}
}
