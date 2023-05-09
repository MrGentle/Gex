using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gex.Modules {
	internal class AnnoyanceRemoverModule : MonoBehaviour {
		void Awake() {
		}
	}

    public static class TitleScreenPatches {
        [HarmonyPatch(typeof(TitleScreen), "TrackStart")]
        [HarmonyPrefix]
        static bool TrackStartPatch(TitleScreen __instance) {
            AudioManager audio = AudioManager.Instance;

            audio.PlaySoundEffect(__instance.pressStartSound, false, ESFXStackingOptions.DO_NOTHING, true, -1f, -1.0);
            __instance.pressStartObject.SetActive(false);
			__instance.playModeSelectionObject.gameObject.SetActive(true);
			__instance.normalModeButton.GetComponent<UIObjectAudioHandler>().playAudio = false;
			EventSystem.current.SetSelectedGameObject(null);
			EventSystem.current.SetSelectedGameObject(__instance.normalModeButton);
			__instance.normalModeButton.GetComponent<UIObjectAudioHandler>().playAudio = true;
			DemoManager.Instance.demoMode = false;
			__instance.demoModeButton.SetActive(false);
			__instance.introTimer = 60f;
            return false;
        }

		[HarmonyPatch(typeof(TitleScreen), "NormalModeSelectionCoroutine")]
        [HarmonyPrefix]
        static bool NormalModeSelectionCoroutinePatch(TitleScreen __instance) {
            AudioManager.Instance.PlaySoundEffect(__instance.pressStartSound, false, ESFXStackingOptions.DO_NOTHING, true, -1f, -1.0);
			Manager<UIManager>.Instance.GetView<SaveGameSelectionScreen>().GetInScreen();
			__instance.MoveOffscreen();
            return false;
        }

		[HarmonyPatch(typeof(TitleScreen), "OptionsSelectionCoroutine")]
        [HarmonyPrefix]
        static bool OptionsSelectionCoroutinePatch(TitleScreen __instance) {
            AudioManager.Instance.PlaySoundEffect(__instance.pressStartSound, false, ESFXStackingOptions.DO_NOTHING, true, -1f, -1.0);
			OptionScreen optionScreen = Manager<UIManager>.Instance.GetView<OptionScreen>();
			optionScreen.transitioningIn = true;
			optionScreen.GetInScreen();
			__instance.MoveOffscreen();
            return false;
        }
    }


	public static class IntroManagerPatches {
	    
        [HarmonyPatch(typeof(IntroManager), "Start")]
        [HarmonyPrefix]
        static void StartButtonPatch(IntroManager __instance) {
            SavingScreen view = UIManager.Instance.GetView<SavingScreen>();
			if (view == null) {
				UIManager.Instance.ShowView<SavingScreen>(EScreenLayers.CONSOLE, null, true, AnimatorUpdateMode.UnscaledTime);
			}
			DebugConsole.Log("Intro Manager Start");
			UIManager.Instance.PreloadView<TitleScreen>(false);
			UIManager.Instance.PreloadView<SaveGameSelectionScreen>(false);
			UIManager.Instance.PreloadView<OptionScreen>(false);
			__instance.PlayIntroSequence();
			InputManager.Instance.blockAllInputs = false;
			__instance.OnIntroSkipped();
			
        }

    }
}
