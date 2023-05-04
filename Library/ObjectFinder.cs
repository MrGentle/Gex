using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gex.Library {
	public class ObjectFinder : MonoBehaviour {
		public static Camera mainCam;
		public static Camera overHudCam;

		public static List<GameObject> huds = new List<GameObject>();
		public static List<GameObject> hudCameras = new List<GameObject>();
		public static RoomBackground[] roomBackgrounds = new RoomBackground[0];
		public static List<GameObject> bitMasks = new List<GameObject>();
		public static GameObject player;

		private int counter = 30;
		
		void FixedUpdate() {
			counter++;
			if (counter >= 30) {
				mainCam = GetMainCam();
				GetGameObjects(new [] {"InGameHud", "InGameHud(Clone)"}, huds, out huds);
				GetGameObjects(new [] {"8Bit_Mask", "16Bit_Mask"}, bitMasks, out bitMasks);
				GetGameObject("Player", player, out player);
				GetRoomBackgrounds();
				GetHudCams();
				counter = 0;
			}
		}

		private Camera GetMainCam() {
			if (!mainCam) {
				mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
			}

			return mainCam;
		}

		private void GetRoomBackgrounds() {
			roomBackgrounds = FindObjectsOfType<RoomBackground>();
		}

		private void GetHudCams() {
			bool refresh = false;
			if (hudCameras.Count == 0) refresh = true;

			foreach(var go in hudCameras) {
				if (go == null) {
					refresh = true;
				}
			}

			if (refresh) {
				hudCameras.Clear();
				foreach (var ohsr in FindObjectsOfType<OverHudCamScissorRect>()) {
					hudCameras.Add(ohsr.gameObject);
				}
			}
		}

		private void GetGameObject(string name, GameObject input, out GameObject output) {
			if (input == null) {
				output = GameObject.Find(name);
			} else output = input;
		}

		private void GetAllGameobjectsWithName(string name, GameObject[] input, GameObject[] output) {
			bool refresh = false;

			if (input.Length == 0) {
				refresh = true;
			}

			foreach(var go in input) {
				if (go == null) refresh = true;
			} 

			if (refresh) {
				List<GameObject> gos = new List<GameObject>();
				foreach (var go in FindObjectsOfType<GameObject>()) {
					if (go.name == name) gos.Add(go);
				}
				output = gos.ToArray();
			} else output = input;
		}

		private void GetGameObjects(string[] names, List<GameObject> input, out List<GameObject> output) {
			input.RemoveAll(go => go == null);
			
			if (names.Length == input.Count) {
				output = input;
				return;
			}

			foreach(string name in names) {
				if (input.FirstOrDefault(go => go.name == name)) {
					continue;
				}

				var go = GameObject.Find(name);
				if (go != null) input.Add(go);
			}

			output = input;
		}
	}
}
