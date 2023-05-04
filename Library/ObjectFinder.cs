using UnityEngine;

namespace Gex.Library {
	public class ObjectFinder : MonoBehaviour {
		public static Camera mainCam;
		public static GameObject bitMask_8;
		public static GameObject bitMask_16;
		public static GameObject player;

		private int counter = 0;
		
		void FixedUpdate() {
			counter++;
			if (counter >= 30) {
				mainCam = GetMainCam();
				GetGameObject("8Bit_Mask", bitMask_8, out bitMask_8);
				GetGameObject("16Bit_Mask", bitMask_16, out bitMask_16);
				GetGameObject("Player", player, out player);
				counter = 0;
			}
		}

		private Camera GetMainCam() {
			if (!mainCam) {
				mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
			}

			return mainCam;
		}

		private void GetGameObject(string name, GameObject input, out GameObject output) {
			if (input == null) {
				output = GameObject.Find(name);
			} else output = input;
		}
	}
}
