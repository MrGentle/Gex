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

namespace Gex.Modules
{
    internal class FreeCamModule : MonoBehaviour
    {
        private bool active = false;
        private Camera mainCam;

        readonly float BASE_ORTHOGRAPHIC_SIZE = 9;
        readonly Vector3 BASE_LOCAL_SCALE = new Vector3(40, 0, 40);

        Rect wRect = new Rect(20, 20, 300, 50);

        bool showGround = false;
        bool showPlayer = false;
        bool showGPIs = false;

        private void ItemWindow(int id)
        {
            GUILayout.Label("Hold left shift and left click to teleport player to cursor");
            GUILayout.Label("Numpad 4-9 to control the free camera");
            GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                    GUILayout.Label("Boxdrawer");
                    showGround = GUILayout.Toggle(showGround, "Ground");
                    showPlayer = GUILayout.Toggle(showPlayer, "Player");
                    showGPIs = GUILayout.Toggle(showGPIs, "Interactables");
                GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F2)) {
                active = !active;
                Cursor.visible = active;
                if (active) {
                    foreach(var mask in ObjectFinder.bitMasks) mask.transform.localScale = new Vector3(99999999, 0, 9999999);
                    foreach(var hud in ObjectFinder.huds) hud.SetActive(false);

                    Debug.Log(ObjectFinder.roomBackgrounds.Length);
                    foreach(RoomBackground roomBg in ObjectFinder.roomBackgrounds) { roomBg.gameObject.SetActive(false); }
                    foreach(var hudCam in ObjectFinder.hudCameras) hudCam.SetActive(false);

                    if (mainCam) {
                        mainCam.orthographicSize = 20;
                        mainCam.gameObject.GetComponent<RetroCamera>().enabled = false;
                    }
                }
                else
                {
                    if (mainCam)
                    {
                        foreach(var mask in ObjectFinder.bitMasks) mask.transform.localScale = BASE_LOCAL_SCALE;
                        foreach(var hud in ObjectFinder.huds) hud.SetActive(true);

                        foreach(RoomBackground roomBg in ObjectFinder.roomBackgrounds) roomBg.gameObject.SetActive(true);
                        foreach(var hudCam in ObjectFinder.hudCameras) hudCam.SetActive(true);

                        mainCam.orthographicSize = BASE_ORTHOGRAPHIC_SIZE;
                        mainCam.gameObject.GetComponent<RetroCamera>().enabled = true;
                    }

                }
            }
        }



        private void OnRenderObject() {
            if(mainCam != null && Camera.current == mainCam) {
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
                    if (c && c.isActiveAndEnabled && shouldBeDrawn)
                    {
                        switch (c.tag)
                        {
                            case "Untagged":
                                Drawer.DrawCube(new Vector2(c.transform.position.x + c.offset.x, c.transform.position.y + c.offset.y), c.bounds.size, new Color(1, 1, 1, 0.5f));
                                break;
                            case "NotHookable":
                                Drawer.DrawCube(new Vector2(c.transform.position.x + c.offset.x, c.transform.position.y + c.offset.y), c.bounds.size, new Color(1, 1, 0, 0.5f));
                                break;
                            case "NotClimbable":
                                Drawer.DrawCube(new Vector2(c.transform.position.x + c.offset.x, c.transform.position.y + c.offset.y), c.bounds.size, new Color(0, 1, 1, 0.5f));
                                break;
                            case "GraplouBounceBack":
                                Drawer.DrawCube(new Vector2(c.transform.position.x + c.offset.x, c.transform.position.y + c.offset.y), c.bounds.size, new Color(1, 0, 1, 0.5f));
                                break;
                            case "GraplouTarget":
                                Debug.Log(c.gameObject.name);
                                Drawer.DrawCube(new Vector2(c.transform.position.x + c.offset.x, c.transform.position.y + c.offset.y), c.bounds.size, new Color(0, 0, 0, 0.5f));
                                break;
                            case "NotHookable_NotClimbable":
                                Drawer.DrawCube(new Vector2(c.transform.position.x + c.offset.x, c.transform.position.y + c.offset.y), c.bounds.size, new Color(1, 0, 0, 0.5f));
                                break;
                            case "NotClimbable_GraplouBounceBack":
                                Drawer.DrawCube(new Vector2(c.transform.position.x + c.offset.x, c.transform.position.y + c.offset.y), c.bounds.size, new Color(0, 0, 1, 0.5f));
                                break;
                            default:
                                if (!tags.Contains(c.tag)) {
                                    Debug.Log(c.tag);
                                    tags.Add(c.tag);
                                }
                                Drawer.DrawCube(new Vector2(c.transform.position.x + c.offset.x, c.transform.position.y + c.offset.y), c.bounds.size, Color.white);
                                break;

                        }
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (!mainCam) mainCam = ObjectFinder.mainCam;
            //TODO: Allow bepin config for keybinds
            if (mainCam && active)
            {
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButton(0)) {
                    var mousePos = mainCam.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                    PlayerManager.Instance.Player.transform.position = new Vector2(mousePos.x, mousePos.y);
                }

                if (Input.GetKey(KeyCode.Keypad9))
                {
                    mainCam.orthographicSize = Math.Max(mainCam.orthographicSize - (float)Math.Ceiling(mainCam.orthographicSize / 10), BASE_ORTHOGRAPHIC_SIZE);
                }

                if (Input.GetKey(KeyCode.Keypad7))
                {
                    mainCam.orthographicSize += (int)Math.Ceiling(mainCam.orthographicSize / 10);
                }

                if (Input.GetKey(KeyCode.Keypad4))
                {
                    mainCam.transform.position -= new Vector3((int)Math.Ceiling(mainCam.orthographicSize / 10), 0, 0);
                }

                if (Input.GetKey(KeyCode.Keypad8))
                {
                    mainCam.transform.position += new Vector3(0, (int)Math.Ceiling(mainCam.orthographicSize / 15), 0);
                }

                if (Input.GetKey(KeyCode.Keypad6))
                {
                    mainCam.transform.position += new Vector3((int)Math.Ceiling(mainCam.orthographicSize / 10), 0, 0);
                }

                if (Input.GetKey(KeyCode.Keypad5))
                {
                    mainCam.transform.position -= new Vector3(0, (int)Math.Ceiling(mainCam.orthographicSize / 15), 0);
                }
            }
        }

        private void OnGUI() {
            if (active) {
                wRect = GUILayout.Window(7878002, wRect, ItemWindow, "Free cam");
            }
        }

    }
}
