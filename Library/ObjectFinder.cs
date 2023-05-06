using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gex.Library {
	public class ObjectFinder : MonoBehaviour {
		public static Camera mainCam;

		public static RoomBackground[] roomBackgrounds = new RoomBackground[0];
		public static BoxCollider2D[] colliders = new BoxCollider2D[0];

		public static List<GameObject> huds = new List<GameObject>();
		public static List<GameObject> hudCameras = new List<GameObject>();
		public static List<GameObject> bitMasks = new List<GameObject>();

		private int counter = 30;
		private bool clear = false;

		void Awake() {
			LevelManager.Instance.onLevelLoaded += OnLevelLoaded;
		}

		private void OnLevelLoaded(string scene) {
			Debug.Log("Loaded scene: " + scene);
			clear = true;
		}

		void FixedUpdate() {
			counter++;
			if (counter >= 30) {
				GetComponentFromGameObject(ref mainCam, "Main Camera");
				GetGameObjectByNamesArray(ref huds, new [] {"InGameHud", "InGameHud(Clone)"});
				GetGameObjectByNamesArray(ref bitMasks, new [] {"8Bit_Mask", "16Bit_Mask"});
				GetGameObjectsByComponent<OverHudCamScissorRect>(ref hudCameras);
				//GetComponents(ref groundColliders, "Combined Collider");
				GetComponents(ref colliders);
				GetComponents(ref roomBackgrounds);
				counter = 0;
				clear = false;
			}
		}

		private void GetComponentFromGameObject<T>(ref T input, string gameObjectName) {
			if (input == null || clear) {
				GameObject go = GameObject.Find(gameObjectName);
			
				if(go != null) {
					input = go.GetComponent<T>();
				}
			}
		}

		private void GetComponentsInChildren<T>(ref T[] input, string gameObjectName) where T : UnityEngine.Object {
			Debug.Log(input.Length);
			if (input.Length != 0 && input.All(component => component != null) && !clear) {
				Debug.Log("returned");
				return;
			}

			var gos = new GameObject[0];
			GetGameObjectsByName(ref gos, gameObjectName);

			var components = new List<T>();
			foreach (var go in gos) {
				components.AddRange(go.GetComponentsInChildren<T>());
			}

			input = components.ToArray();

		}

		private void GetComponents<T>(ref T[] input, string gameObjectName = null) where T : UnityEngine.Object {
			
			if (input.Length != 0 && input.All(component => component != null) && !clear) {
				
				return;
			}

			if (gameObjectName != null) {
				List<T> ret = new List<T>();
				foreach (var gameObj in FindObjectsOfType<GameObject>()) {
                    if (gameObj && gameObj.name == gameObjectName && gameObj.GetComponent<T>() != null) {
						ret.Add(gameObj.GetComponent<T>());
                    }
                }
				input = ret.ToArray();
			} else {
				input = FindObjectsOfType<T>();
			}
		}

		private void GetGameObject(ref GameObject input, string name) {
			if (input == null || clear) {
				input = GameObject.Find(name);
			}
		}

		private void GetGameObjectsByComponent<T>(ref List<GameObject> input) {
			if (input.Count == 0 || clear) {
				input.Clear();
				Component[] components = FindObjectsOfType(typeof(T)) as Component[];
				foreach(Component co in components) {
					input.Add(co.gameObject);
				}
			}
		}

		private void GetGameObjectsByName(ref GameObject[] input, string name) {
			bool refresh = false;

			if (input.Length == 0 || clear) {
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
				input = gos.ToArray();
			}
		}

		private void GetGameObjectByNamesArray(ref List<GameObject> input, string[] names) {
			if (clear) input.Clear();
			input.RemoveAll(go => go == null);
			
			if (names.Length == input.Count) {
				return;
			}

			foreach(string name in names) {
				if (input.FirstOrDefault(go => go.name == name)) {
					continue;
				}

				var go = GameObject.Find(name);
				if (go != null) input.Add(go);
			}
		}
	}
}
