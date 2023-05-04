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
        List<BoxCollider2D> colliders = new List<BoxCollider2D>();

        readonly float BASE_ORTHOGRAPHIC_SIZE = 9;
        readonly Vector3 BASE_LOCAL_SCALE = new Vector3(40, 0, 40);



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                active = !active;
                if (active)
                {
                    var masks = ObjectFinder.bitMasks;
                    if (masks.Count > 0) foreach(var mask in masks) mask.transform.localScale = new Vector3(99999999, 0, 9999999);

                    var huds = ObjectFinder.huds;
                    if (huds.Count > 0) foreach(var hud in huds) hud.SetActive(false);

                    foreach(RoomBackground roomBg in ObjectFinder.roomBackgrounds) roomBg.gameObject.SetActive(false);
                    foreach(var hudCam in ObjectFinder.hudCameras) hudCam.SetActive(false);

                    if (mainCam)
                    {
                        mainCam.orthographicSize = 20;
                        mainCam.gameObject.GetComponent<RetroCamera>().enabled = false;
                        colliders.Clear();
                        foreach (var gameObj in FindObjectsOfType<GameObject>())
                        {
                            if (gameObj)
                            {
                                if (gameObj.name == "Combined Collider")
                                {
                                    if (gameObj.GetComponent<BoxCollider2D>() != null) colliders.Add(gameObj.GetComponent<BoxCollider2D>());
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (mainCam)
                    {
                        var masks = ObjectFinder.bitMasks;
                        if (masks.Count > 0) {
                            foreach(var mask in masks) mask.transform.localScale = BASE_LOCAL_SCALE;
                        }

                        var huds = ObjectFinder.huds;
                        if (huds.Count > 0) foreach(var hud in huds) hud.SetActive(true);

                        foreach(RoomBackground roomBg in ObjectFinder.roomBackgrounds) roomBg.gameObject.SetActive(true);
                        foreach(var hudCam in ObjectFinder.hudCameras) hudCam.SetActive(true);

                        mainCam.orthographicSize = BASE_ORTHOGRAPHIC_SIZE;
                        mainCam.gameObject.GetComponent<RetroCamera>().enabled = true;
                    }

                }
            }
        }



        private void OnRenderObject()
        {
            if(mainCam != null && Camera.current == mainCam) {
                foreach (var c in colliders)
                {
                    var currentDim = DimensionManager.Instance.currentDimension == EBits.BITS_8 ? "Middleground_8" : "Middleground_16";

                    if (c.isActiveAndEnabled && c.transform.parent.name == currentDim)
                    {
                        switch (c.tag)
                        {
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
        }

        private void FixedUpdate()
        {
            if (!mainCam) mainCam = ObjectFinder.mainCam;

            Debug.Log(PlayerManager.Instance.Player.transform.position);
            //TODO: Allow bepin config for keybinds
            if (mainCam && active)
            {
                if (Input.GetMouseButton(0)) {
                    var mousePos = mainCam.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
                    PlayerManager.Instance.Player.transform.position = new Vector2(mousePos.x, mousePos.y);
                    
                }

                if (Input.GetKey(KeyCode.Keypad7))
                {
                    mainCam.orthographicSize = Math.Max(mainCam.orthographicSize - (float)Math.Ceiling(mainCam.orthographicSize / 10), BASE_ORTHOGRAPHIC_SIZE);
                }

                if (Input.GetKey(KeyCode.Keypad9))
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

        private void OnGUI()
        {
            if (active)
            {
                GUILayout.Label("Free cam active");
            }
        }

    }
}
