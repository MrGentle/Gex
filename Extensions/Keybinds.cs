using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gex.Extensions {
	internal static class Keybinds {
		public static bool AnyPressed(this ConfigEntry<KeyboardShortcut>[] keybinds) {
			return keybinds.FirstOrDefault(key => key.Pressed()) != null;
		}

		public static bool Pressed(this ConfigEntry<KeyboardShortcut> keybind) {
			return Input.GetKey(keybind.Value.MainKey);
		}
	}
}
