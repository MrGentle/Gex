using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static Gex.Library.Drawer;
using static System.Collections.Specialized.BitVector32;

namespace Gex.Library {
	internal class GexGUISkin {
		public static GUISkin gexSkin = new GUISkin();
		public static GUIStyle windowHeader;
		public static GUIStyle windowLabel;
		public static GUIStyle section;
		public static GUIStyle sectionHeader;
		public static GUIStyle label;

		private static Dictionary<string, Color32> colors = new Dictionary<string, Color32>();
		private static Dictionary<string, Texture2D> textureColors = new Dictionary<string, Texture2D>();
		private static Dictionary<string, RectOffset> padding = new Dictionary<string, RectOffset>();
		private static Dictionary<string, RectOffset> margin = new Dictionary<string, RectOffset>();

		public static Rect Window(int id, Rect screenRect, GUI.WindowFunction funct) {
			GUI.skin = gexSkin;
			var rect = GUILayout.Window(id, screenRect, funct, "");
			GUI.skin = null;
			return rect;
		}


		public static void WindowHeader(string text) {
			GUI.skin.label = windowHeader;
			GUILayout.Label(text);
			GUI.skin.label = label;
		}

		public static void WindowLabel(string text) {
			GUI.skin.label = windowLabel;
			GUILayout.Label(text);
			GUI.skin.label = label;
		}

		public static void SectionHeader(string text) {
			GUI.skin.label = sectionHeader;
			GUILayout.Label(text);
			GUI.skin.label = label;
		}

		public static GUISkin InitializeSkin() {
			colors.Add("window", new Color32(3, 0, 28, 230));
			colors.Add("section", new Color32(255, 255, 255, 20));
			colors.Add("primary", new Color32(48, 30, 103, 255));
			colors.Add("header", new Color32(91, 143, 185, 255));
			colors.Add("text", new Color32(182, 234, 218, 255));

			foreach(var color in colors) textureColors.Add(color.Key, Texture2DFromColor(color.Value));

			padding.Add("window", new RectOffset(20, 20, 10, 20));
			padding.Add("windowHeader", new RectOffset(0, 0, 5, 5));
			padding.Add("section", new RectOffset(5, 5, 5, 5));
			padding.Add("sectionHeader", new RectOffset(0, 0, 15, 5));

			margin.Add("window", new RectOffset(20, 20, 10, 20));
			margin.Add("windowHeader", new RectOffset(0, 0, 5, 5));
			margin.Add("section", new RectOffset(10, 10, 10, 10));
			margin.Add("sectionHeader", new RectOffset(0, 0, 0, 0));

			var window = new GUIStyle();
			window.normal.background = textureColors["window"];
			window.border = new RectOffset(50, 50, 50, 50);
			window.normal.textColor = colors["header"];
			window.fontSize = 18;
			gexSkin.window = window;

			windowHeader = new GUIStyle();
			windowHeader.fontSize = 18;
			windowHeader.padding = padding["windowHeader"];
			windowHeader.normal.background = textureColors["primary"];
			windowHeader.alignment = TextAnchor.UpperCenter;
			windowHeader.fontStyle = FontStyle.Bold;
			windowHeader.border = new RectOffset(0, 0, 0, 1);
			windowHeader.normal.textColor = colors["header"];

			windowLabel = new GUIStyle();
			windowLabel.normal.textColor = colors["text"];
			windowLabel.margin = margin["section"];
			windowLabel.fontSize = 14;

			section = new GUIStyle();
			section.normal.background = textureColors["section"];
			section.padding = padding["section"];
			section.margin = margin["section"];

			sectionHeader = new GUIStyle();
			sectionHeader.margin = margin["sectionHeader"];
			sectionHeader.fontSize = 16;
			sectionHeader.fontStyle = FontStyle.Bold;
			sectionHeader.alignment = TextAnchor.UpperCenter;
			sectionHeader.normal.textColor = colors["header"];

			label = new GUIStyle();
			label.normal.textColor = colors["text"];
			label.fontSize = 14;
			gexSkin.label = label;

			var toggle = new GUIStyle("toggle");
			toggle.fontSize = 14;
			toggle.normal.textColor = colors["text"];
			gexSkin.toggle = toggle;

			var button = new GUIStyle("button");
			button.normal.background = textureColors["primary"];
			button.normal.textColor = colors["text"];
			gexSkin.button = button;

			return gexSkin;
		}


		static Texture2D Texture2DFromColor(Color32 color) {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            return tex;
        }
	}
}
