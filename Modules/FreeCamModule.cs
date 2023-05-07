using Gex.Library;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Gex.Modules {
    internal class FreeCamModule : MonoBehaviour {
        private bool active = false;

        readonly float BASE_ORTHOGRAPHIC_SIZE = 9;
        readonly Vector3 BASE_LOCAL_SCALE = new Vector3(40, 0, 40);

        Rect wRect = new Rect(20, 300, 300, 50);

        bool toggleCam = false;

        bool showGround = false;
        bool showPlayer = false;
        bool showGPIs = false;
        bool renderPlayerInFront = false;
        bool disableCamOnPlayerTeleport = true;


        private void GUIWindow(int id) {
            GUILayout.Label("Hold left shift and left click to teleport player to cursor");
            GUILayout.Label("Numpad 4-9 to control the free camera");
            GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                    GUILayout.Label("Boxdrawer");
                    showGround = GUILayout.Toggle(showGround, "Ground");
                    showPlayer = GUILayout.Toggle(showPlayer, "Player");
                    showGPIs = GUILayout.Toggle(showGPIs, "Interactables");
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                    GUILayout.Label("Settings");
                    renderPlayerInFront = GUILayout.Toggle(renderPlayerInFront, "Render player in front");
                    disableCamOnPlayerTeleport = GUILayout.Toggle(disableCamOnPlayerTeleport, "Disable cam when placing player");
                GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private void Update() {
            if (active && ObjectFinder.mainCam && ObjectFinder.player) {
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0)) {
                    var mousePos = ObjectFinder.mainCam.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                    ObjectFinder.player.transform.position = new Vector2(mousePos.x, mousePos.y);
                    ObjectFinder.player.SetVelocity(0, 0);
                    ObjectFinder.mainCam.transform.position = mousePos;
                    if (disableCamOnPlayerTeleport) toggleCam = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.F3) || toggleCam) {
                active = !active;
                CursorHandler.Show(active);

                if (ObjectFinder.player != null && !renderPlayerInFront) {
                    //Render player in front of environment
                    foreach (SpriteRenderer renderer in ObjectFinder.player.gameObject.GetComponentsInChildren<SpriteRenderer>()) {
                        renderer.sortingLayerName = active ? "UI" : "Player";
                    }
                }

                if (ObjectFinder.mainCam && ObjectFinder.hud && ObjectFinder.bitMasks.Count > 0 && ObjectFinder.roomBackgrounds.Length > 0) {
                    var RetroCamera = ObjectFinder.mainCam.GetComponent<RetroCamera>();
                    SpriteRenderer[] maincamRenderers = ObjectFinder.mainCam.GetComponentsInChildren<SpriteRenderer>();

                    if (active) {
                        ObjectFinder.hud.HideHud();

                        //Expand the mask to show more of the level - Bad method, must be a better way
                        foreach(var mask in ObjectFinder.bitMasks) { mask.transform.localScale = new Vector3(99999999, 0, 9999999); }
                        //Hide room backgrounds - Probably a better way to do this as well
                        foreach(RoomBackground roomBg in ObjectFinder.roomBackgrounds) { roomBg.gameObject.SetActive(false); }
                    
                        ObjectFinder.mainCam.orthographicSize = 20;
                        
                        RetroCamera.enabled = false;
                        foreach (var renderer in maincamRenderers) renderer.enabled = false;
                    } else {
                        ObjectFinder.hud.ShowHud();

                        //Render player in front of environment
                        if (ObjectFinder.player != null) {
                            foreach (SpriteRenderer renderer in ObjectFinder.player.gameObject.GetComponentsInChildren<SpriteRenderer>()) {
                                renderer.sortingLayerName = renderPlayerInFront ? "UI" : "Player";
                            }
                        }

                        foreach(var mask in ObjectFinder.bitMasks) mask.transform.localScale = BASE_LOCAL_SCALE;
                        foreach(RoomBackground roomBg in ObjectFinder.roomBackgrounds) { roomBg.gameObject.SetActive(true); }

                        ObjectFinder.mainCam.orthographicSize = BASE_ORTHOGRAPHIC_SIZE;
                        foreach (var renderer in maincamRenderers) renderer.enabled = true;
                        RetroCamera.enabled = true;
                        RetroCamera.ChangeScreen(ObjectFinder.player.transform.position);
                        RetroCamera.ForceUpdate = true;
                        RetroCamera.ForceScreenChange = true;
                    }
                }

                toggleCam = false;
            }
        }



        private void OnRenderObject() {
            if(ObjectFinder.mainCam != null && Camera.current == ObjectFinder.mainCam) {
                var drawable8 = new List<string>();
                var drawable16 = new List<string>();

                if (showGround) {
                    drawable8.AddRange(new string[] {"Tilemap_8"});
                    drawable16.AddRange(new string[] {"Tilemap_16"});
                }

                if (showGPIs) {
                    drawable8.AddRange(new string[] {"GPI_8", "GPI_8_and_16"});
                    drawable16.AddRange(new string[] { "GPI_16", "GPI_8_and_16"});
                }

                var tags = new List<string>();
                foreach (var c in ObjectFinder.colliders) {
                    var currentDimension = DimensionManager.Instance.currentDimension;
                    var shouldBeDrawn = false;
                    
                    var parent = c.transform.parent;
                    if (c.gameObject.name == "Player" && showPlayer) shouldBeDrawn = true;

                    while (parent != null) {
                        if (currentDimension == EBits.BITS_8) {
                            if (drawable8.Contains(parent.name)) {
                                shouldBeDrawn = true;
                                break;
                            }
                        }
                        if (currentDimension == EBits.BITS_16) {
                            if (drawable16.Contains(parent.name)) {
                                shouldBeDrawn = true;
                                break;
                            }
                        }
                        parent = parent.parent;
                    }

                    if (c && c.isActiveAndEnabled && shouldBeDrawn) {
                        //Horrible sorting system - Find a better way
                        var pos = new Vector2(c.transform.position.x + c.offset.x, c.transform.position.y + c.offset.y);
                        switch (c.tag) {
                            case "Untagged":
                                Drawer.DrawCube(pos, c.bounds.size, new Color(1, 1, 1, 1f));
                                break;
                            case "NotHookable":
                                Drawer.DrawCube(pos, c.bounds.size, new Color(1, 1, 0, 1f));
                                break;
                            case "NotClimbable":
                                Drawer.DrawCube(pos, c.bounds.size, new Color(0, 1, 1, 1f));
                                break;
                            case "GraplouBounceBack":
                                Drawer.DrawCube(pos, c.bounds.size, new Color(1, 0, 1, 1f));
                                break;
                            case "GraplouTarget":
                                Drawer.DrawCube(pos, c.bounds.size, new Color(0, 0, 0, 1f));
                                break;
                            case "NotHookable_NotClimbable":
                                Drawer.DrawCube(pos, c.bounds.size, new Color(1, 0, 0, 1f));
                                break;
                            case "NotClimbable_GraplouBounceBack":
                                Drawer.DrawCube(pos, c.bounds.size, new Color(0, 0, 1, 1f));
                                break;
                            default:
                                if (!tags.Contains(c.tag)) {
                                    tags.Add(c.tag);
                                }
                                Drawer.DrawCube(pos, c.bounds.size, Color.white);
                                break;

                        }
                    }
                }
            }
        }

        private void FixedUpdate() {
            //TODO: Allow bepin config for keybinds
            if (ObjectFinder.mainCam && active && ObjectFinder.player) {
                if (Input.GetKey(KeyCode.Keypad9)) {
                    ObjectFinder.mainCam.orthographicSize = Math.Max(ObjectFinder.mainCam.orthographicSize - (float)Math.Ceiling(ObjectFinder.mainCam.orthographicSize / 20), BASE_ORTHOGRAPHIC_SIZE);
                }

                if (Input.GetKey(KeyCode.Keypad7)) {
                    ObjectFinder.mainCam.orthographicSize += (int)Math.Ceiling(ObjectFinder.mainCam.orthographicSize / 20);
                }

                if (Input.GetKey(KeyCode.Keypad4)) {
                    ObjectFinder.mainCam.transform.position -= new Vector3((int)Math.Ceiling(ObjectFinder.mainCam.orthographicSize / 20), 0, 0);
                }

                if (Input.GetKey(KeyCode.Keypad8)) {
                    ObjectFinder.mainCam.transform.position += new Vector3(0, (int)Math.Ceiling(ObjectFinder.mainCam.orthographicSize / 30), 0);
                }

                if (Input.GetKey(KeyCode.Keypad6)) {
                    ObjectFinder.mainCam.transform.position += new Vector3((int)Math.Ceiling(ObjectFinder.mainCam.orthographicSize / 20), 0, 0);
                }

                if (Input.GetKey(KeyCode.Keypad5)) {
                    ObjectFinder.mainCam.transform.position -= new Vector3(0, (int)Math.Ceiling(ObjectFinder.mainCam.orthographicSize / 30), 0);
                }
            }
        }

        private void OnGUI() {
            if (active) {
                wRect = GUILayout.Window(7878002, wRect, GUIWindow, "Free cam");
            }
        }

    }
}
