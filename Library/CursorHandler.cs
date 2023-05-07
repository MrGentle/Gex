using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gex.Library {
	public static class CursorHandler {
		public static int activeCursors = 0;

		public static void Show(bool show) {
			if (show) Activate();
			else Deactivate();
		}

		public static void Activate() {
			activeCursors++;
			if (activeCursors > 0) {
				Cursor.visible = true;
			}
		}

		public static void Deactivate() {
			activeCursors--;
			if (activeCursors <= 0) {
				activeCursors = 0;
				Cursor.visible = false;
			}
		}

	}
}
