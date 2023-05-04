using Gex.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using Rotorz.Tile;
using UnityEngine.Experimental.UIElements;

namespace Gex {
	internal class CamController : MonoBehaviour {
		private bool active = false;
		private Camera mainCam;
		GameObject maskObject;
		List<BoxCollider2D> colliders = new List<BoxCollider2D>();

		const float STANDARD_ORTHOGRAPHIC_SIZE = 9;



		private void Update() {
			if (Input.GetKeyDown(KeyCode.F2)) {
				active = !active;
				if (active) {
					if (maskObject) maskObject.transform.localScale = new Vector3(999999, 0, 99999);
					if (mainCam) {
						mainCam.orthographicSize = 20;
						mainCam.gameObject.GetComponent<RetroCamera>().enabled = false;
						colliders.Clear();
						foreach (var gameObj in FindObjectsOfType<GameObject>()) {
							if (gameObj) {
								if(gameObj.name == "Combined Collider") {
									if (gameObj.GetComponent<BoxCollider2D>() != null) colliders.Add(gameObj.GetComponent<BoxCollider2D>());
								}
							}
						}
					}
				} else {
					if (maskObject) maskObject.transform.localScale = new Vector3(40,0,40);
					if (mainCam) {
						mainCam.orthographicSize = STANDARD_ORTHOGRAPHIC_SIZE;
						mainCam.gameObject.GetComponent<RetroCamera>().enabled = true;
					}
				
				}
			}
		}

		

		private void OnRenderObject() {
			foreach(var c in colliders) {
				if (c.isActiveAndEnabled) {
					switch (c.tag) {
						case "Untagged":
							Drawer.DrawCube(c.offset - new Vector2(500, -500), c.bounds.size, new Color(0, 1, 0, 0.5f));
							break;
						case "NotHookable_NotClimbable":
							Drawer.DrawCube(c.offset - new Vector2(500, -500), c.bounds.size, Color.red);
							break;
						case "NotClimbable_GraplouBounceBack":
							Drawer.DrawCube(c.offset - new Vector2(500, -500), c.bounds.size, Color.blue);
							break;
						default:
							Drawer.DrawCube(c.offset - new Vector2(500, -500), c.bounds.size, Color.white);
							break;

					}
				}
			}

			Drawer.DrawCube(mainCam.ScreenToWorldPoint(Input.mousePosition), new Vector2(1, 1), Color.red);
			Drawer.DrawCube(new Vector3(-1000, 1000, 0) + mainCam.transform.position, new Vector2(4, 5), Color.white);
		}

		private void FixedUpdate() {
			if (!mainCam) mainCam = ObjectFinder.mainCam;
			if (!maskObject) maskObject = ObjectFinder.bitMask_8;

			if (mainCam && active) {
				
				if (Input.GetMouseButton((int)MouseButton.LeftMouse)) {
					//ObjectFinder.bitMask_16.transform.position = mainCam.ScreenToWorldPoint(Input.mousePosition);
					if (ObjectFinder.player) {
						
					}
				}

				if (Input.GetKey(KeyCode.Keypad7)) {
					mainCam.orthographicSize = Math.Max(mainCam.orthographicSize - (float)Math.Ceiling(mainCam.orthographicSize/10), STANDARD_ORTHOGRAPHIC_SIZE);
				}

				if (Input.GetKey(KeyCode.Keypad9)) {
					mainCam.orthographicSize += (int)Math.Ceiling(mainCam.orthographicSize/10);
				}

				if (Input.GetKey(KeyCode.Keypad4)) {
					mainCam.transform.position -= new Vector3((int)Math.Ceiling(mainCam.orthographicSize/10), 0, 0);
				}

				if (Input.GetKey(KeyCode.Keypad8)) {
					mainCam.transform.position += new Vector3(0, (int)Math.Ceiling(mainCam.orthographicSize/15), 0);
				}

				if (Input.GetKey(KeyCode.Keypad6)) {
					mainCam.transform.position += new Vector3((int)Math.Ceiling(mainCam.orthographicSize/10), 0, 0);
				}

				if (Input.GetKey(KeyCode.Keypad5)) {
					mainCam.transform.position -= new Vector3(0, (int)Math.Ceiling(mainCam.orthographicSize/15), 0);
				}
			}
		}

		private void OnGUI() {
			if (active) {
				GUILayout.Label("Free cam active");
			}
		}

	}
}
