using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gex.Library {
	internal static class Util {
		public static int BoolToInt(bool b) {
			return b ? 1 : 0;
		}
	}
}
