using Gex.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.GridLayoutGroup;

namespace Gex.Modules {
    internal class PlayerStateModule : MonoBehaviour {
        
        private bool show = false;
        Rect wRect = new Rect(20, 20, 120, 50);
        Vector2 savedPosition = new Vector2(-1, -1);
        Queue<string> lastStates = new Queue<string>();

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F3)) {
                show = !show;
                Cursor.visible = show;
                Debug.Log(show);
                //Manager<InventoryManager>.Instance.RemoveAllItems();
            }


            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) {
                SavePosition();
            } else if (Input.GetKeyDown(KeyCode.R)) LoadPosition();
        }

        private void FixedUpdate() {
            GetPlayerState();
        }

        private void OnGUI() {
            if (show) {
                wRect = GUILayout.Window(7878003, wRect, ItemWindow, "State manager");
            }
        }

        private void ItemWindow(int id)
        {
            GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                    GUILayout.Label("General");
                    if (GUILayout.Button("Change Dimension")) {
                        DimensionManager.Instance.SetDimension(DimensionManager.Instance.CurrentDimension == EBits.BITS_8 ? EBits.BITS_16 : EBits.BITS_8);
                    }

                    if (GUILayout.Button("Reload Level")) {
                        FullCheckpointReload();
                    }

                    GUILayout.Space(30);

                    GUILayout.Label("Player state");
                    GUILayout.Label(lastStates.Count.ToString());
                    foreach(string state in lastStates) {
                        GUILayout.Label(state);
                    }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                    GUILayout.Label("Load");
                    if (GUILayout.Button("Checkpoint")) {
                        ReloadCheckpoint();
                    }
                    if (GUILayout.Button("Position")) {
                        LoadPosition();
                    }
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                    GUILayout.Label("Load level");

                    GUILayout.BeginHorizontal();
                        GUILayout.BeginVertical();
                        if (GUILayout.Button("Autumn Hills")) { LoadStage(ELevel.Level_02_AutumnHills); }
                        if (GUILayout.Button("Forlorn Temple")) { LoadStage(ELevel.Level_03_ForlornTemple); }
                        if (GUILayout.Button("Dark Cave")) { LoadStage(ELevel.Level_04_B_DarkCave); }
                        if (GUILayout.Button("Catacombs")) { LoadStage(ELevel.Level_04_Catacombs); }
                        if (GUILayout.Button("Riviere Turquoise")) { LoadStage(ELevel.Level_04_C_RiviereTurquoise); }
                        if (GUILayout.Button("Howling Grotto")) { LoadStage(ELevel.Level_05_A_HowlingGrotto); }
                        if (GUILayout.Button("Sunken Shrine")) { LoadStage(ELevel.Level_05_B_SunkenShrine); }
                        if (GUILayout.Button("Bamboo Creek")) { LoadStage(ELevel.Level_06_A_BambooCreek); }
                        if (GUILayout.Button("Quillshroom Marsh")) { LoadStage(ELevel.Level_07_QuillshroomMarsh); }            
                        GUILayout.EndVertical();

                        GUILayout.BeginVertical();
                        if (GUILayout.Button("Searing Crags")) { LoadStage(ELevel.Level_08_SearingCrags); }
                        if (GUILayout.Button("Glacial Peak")) { LoadStage(ELevel.Level_09_A_GlacialPeak); }
                        if (GUILayout.Button("Elemental Skylands")) { LoadStage(ELevel.Level_09_B_ElementalSkylands); }
                        if (GUILayout.Button("Tower Of Time")) { LoadStage(ELevel.Level_10_A_TowerOfTime); }
                        if (GUILayout.Button("Cloud Ruins")) { LoadStage(ELevel.Level_11_A_CloudRuins); }
                        if (GUILayout.Button("Music Box")) { LoadStage(ELevel.Level_11_B_MusicBox); }
                        if (GUILayout.Button("Under World")) { LoadStage(ELevel.Level_12_UnderWorld); }
                        if (GUILayout.Button("Tower Of Time HQ")) { LoadStage(ELevel.Level_13_TowerOfTimeHQ); }
                        GUILayout.EndVertical();
                        
                        GUILayout.BeginVertical();
                        if (GUILayout.Button("Corrupted Future")) { LoadStage(ELevel.Level_14_CorruptedFuture); }
                        if (GUILayout.Button("Surf")) { LoadStage(ELevel.Level_15_Surf); }
                        if (GUILayout.Button("Beach")) { LoadStage(ELevel.Level_16_Beach); }
                        if (GUILayout.Button("Volcano")) { LoadStage(ELevel.Level_17_Volcano); }
                        if (GUILayout.Button("Volcano Chase")) { LoadStage(ELevel.Level_18_Volcano_Chase); }
                        if (GUILayout.Button("Ending")) { LoadStage(ELevel.Level_Ending); }
                        if (GUILayout.Button("Ending PP")) { LoadStage(ELevel.Level_Ending_PP); }
                        if (GUILayout.Button("Surf Credits")) { LoadStage(ELevel.Level_Surf_Credits); }
                        GUILayout.EndVertical();

                    GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        void FullCheckpointReload() {
            LevelLoadingInfo levelLoadingInfo = new LevelLoadingInfo(Manager<LevelManager>.Instance.CurrentSceneName, false, true, LoadSceneMode.Single, ELevelEntranceID.NONE, Manager<DimensionManager>.Instance.currentDimension);
            Manager<AudioManager>.Instance.StopMusic();
            Manager<LevelManager>.Instance.LoadLevel(levelLoadingInfo);
        }

        private void ReloadCheckpoint() {
            AudioManager.Instance.StopMusic();
            LevelManager.Instance.LoadToLastCheckpoint(true, true);
        }

        private void SavePosition() {
            savedPosition = PlayerManager.Instance.Player.transform.position;
        }

        private void LoadPosition() {
            if (savedPosition.x == -1 && savedPosition.y == -1) SavePosition();
            PlayerManager.Instance.Player.transform.position = savedPosition; 
        }

        private void LoadStage(ELevel level) {
            AudioManager.Instance.StopMusic();
            LevelLoadingInfo i = new LevelLoadingInfo(level.ToString() + "_Build", false, true, LoadSceneMode.Single, ELevelEntranceID.NONE, DimensionManager.Instance.currentDimension);
            i.showLevelIntro = false;

			i.levelInitializerParams = new LevelInitializerParams();
            i.levelInitializerParams.entranceCutsceneType = null;
            LevelManager.Instance.LoadLevel(i);
        }

        private void GetPlayerState() {
            var currentState = PlayerManager.Instance.Player.StateMachine.CurrentState;
            if (lastStates.Count > 0) {
                if (lastStates.Last() != currentState.name) {
                    lastStates.Enqueue(currentState.name);
                } 
            } else lastStates.Enqueue(currentState.name);

            if (lastStates.Count > 5) {
                lastStates.Dequeue();
            }
        }
    }
}
