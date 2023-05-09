using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Gex.Modules {
	internal class TimeScaleModule : MonoBehaviour {
		bool activated = false;
		
		void Update() {
			if (Input.GetKeyDown(KeyCode.Y)) {
				activated = !activated;
				if (activated) {
					Time.timeScale = 0;
				} else Time.timeScale = 1;
			}

			if (activated && Input.GetKeyDown(KeyCode.U)) {
				Time.timeScale = 1;
				StartCoroutine(ResetTimescale());
			}
		}

		void OnGUI() {
			GUILayout.BeginArea(new Rect(20, Screen.height - 300, 300, 280));
			if (PlayerManager.Instance && PlayerManager.Instance.Player && activated) {
				GUILayout.Label("Velocity: " + PlayerManager.Instance.Player.Velocity.ToString());
			}
			GUILayout.EndArea();
		}


		IEnumerator ResetTimescale() {
			yield return new WaitForFixedUpdate();
			
			Time.timeScale = 0;
		}



	}
}
