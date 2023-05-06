using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Gex.Modules
{
    internal class ItemGUIModule : MonoBehaviour {
        private bool show = false;
        Rect wRect = new Rect(20, 20, 120, 50);

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F1)) {
                show = !show;
                Cursor.visible = show;
                Debug.Log(show);
                //Manager<InventoryManager>.Instance.RemoveAllItems();
            }
        }

        private void OnGUI() {
            if (show) {
                wRect = GUILayout.Window(7878001, wRect, ItemWindow, "Unlocks");
            }
        }

        private void ItemWindow(int id) {
            GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                    GUILayout.Label("Gear");
                    ItemToggle(EItems.CLIMBING_CLAWS, "Climbing claws");
                    ItemToggle(EItems.WINGSUIT, "Wingsuit");
                    ItemToggle(EItems.GRAPLOU, "Rope dart");
                    ItemToggle(EItems.CANDLE, "Candle");
                    ItemToggle(EItems.MAGIC_BOOTS, "Lightfoot Tabi");
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                    GUILayout.Label("Upgrades");
                    ItemToggle(EItems.SHURIKEN, "Energy shuriken");
                    ItemToggle(EItems.ATTACK_PROJECTILES, "Attack projectiles");
                    ItemToggle(EItems.AIR_RECOVER, "Air recover");
                    ItemToggle(EItems.SWIM_DASH, "Swim dash");
                    ItemToggle(EItems.GLIDE_ATTACK, "Glide attack");
                    ItemToggle(EItems.CHARGED_ATTACK, "Charged attack");
                GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        // Creates a checkbox for the selected item
        private void ItemToggle(EItems item, string label) {
            bool hasItem = HasItem(item);
            if (GUILayout.Toggle(hasItem, label)) {
                GiveItem(item);
            } else {
                RemoveAllItemsOfType(item);
            };
        }

        private bool HasItem(EItems item) {
            return Manager<InventoryManager>.Instance.GetItemQuantity(item) > 0;
        }

        //Gives the player an item
        //TODO: Unlock upgrades in the shop aswell
        private void GiveItem(EItems item, int amount = 1) {
            Manager<InventoryManager>.Instance.AddItem(item, amount);
        }

        private void RemoveItem(EItems item, int amount = 1) {
            Manager<InventoryManager>.Instance.RemoveItem(item, amount);
        }

        private void RemoveAllItemsOfType(EItems item) {
            RemoveItem(item, Manager<InventoryManager>.Instance.GetItemQuantity(item));
        }
    }
}
