using Gex.Library;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Gex.Modules
{
    internal class UnlocksModule : MonoBehaviour {
        private bool show = false;
        Rect wRect = new Rect(20, 20, 120, 50);

        public static bool godmode = false;
        public static bool noDamage = false;
        public static bool infiniteShurikens = false;

        void Awake() {
            Plugin.harmony.PatchAll(typeof(InvincibilityPatch));
            Plugin.harmony.PatchAll(typeof(ShurikenPatch));
            
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.F2)) {
                show = !show;
                CursorHandler.Show(show);
            }
        }

        private void OnGUI() {
            if (show) {
                wRect = GUILayout.Window(7878001, wRect, ItemWindow, "Items and Unlocks");
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
                    ItemToggle(EItems.SHURIKEN, "Energy shuriken", EShopUpgradeID.SHURIKEN);
                    ItemToggle(EItems.ATTACK_PROJECTILES, "Attack projectiles", EShopUpgradeID.ATTACK_PROJECTILE);
                    ItemToggle(EItems.AIR_RECOVER, "Air recover", EShopUpgradeID.AIR_RECOVER);
                    ItemToggle(EItems.SWIM_DASH, "Swim dash", EShopUpgradeID.SWIM_DASH);
                    ItemToggle(EItems.GLIDE_ATTACK, "Glide attack", EShopUpgradeID.GLIDE_ATTACK);
                    ItemToggle(EItems.CHARGED_ATTACK, "Charged attack", EShopUpgradeID.CHARGED_ATTACK);
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                    GUILayout.Label("Cheats");
                    godmode = GUILayout.Toggle(godmode, "Godmode");
                    noDamage = GUILayout.Toggle(noDamage, "No Damage");
                    infiniteShurikens = GUILayout.Toggle(infiniteShurikens, "Infinite shurikens");
                    if (GUILayout.Button("Gain 10000 time shards")) {
                        InventoryManager.Instance.AddItem(EItems.TIME_SHARD, 10000);
                        ObjectFinder.hud.RefreshTimeshards();
                    }
                GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        // Creates a checkbox for the selected item
        private void ItemToggle(EItems item, string label, EShopUpgradeID? shopUpgradeId = null) {
            bool hasItem = HasItem(item);
            if (GUILayout.Toggle(hasItem, label)) {
                if (!hasItem) GiveItem(item, shopUpgradeId);
            } else {
                if (hasItem) RemoveAllItemsOfType(item, shopUpgradeId);
            };
        }

        private bool HasItem(EItems item) {
            return Manager<InventoryManager>.Instance.GetItemQuantity(item) > 0;
        }

        //Gives the player an item and unlocks it in the shop
        private void GiveItem(EItems item, EShopUpgradeID? shopUpgradeId = null, int amount = 1) {
            Manager<InventoryManager>.Instance.AddItem(item, amount);
            if (shopUpgradeId.HasValue) {
                InventoryManager.Instance.SetShopUpgradeAsUnlocked(shopUpgradeId.Value);
            }
            Plugin.Log.LogInfo($"Gave {item} to player");
        }

        //Gives the player an item and removes the unlock from the shop
        private void RemoveItem(EItems item, EShopUpgradeID? shopUpgradeId = null, int amount = 1) {
            Manager<InventoryManager>.Instance.RemoveItem(item, amount);
            if (shopUpgradeId.HasValue && InventoryManager.Instance.HasBoughtShopUpgrade(shopUpgradeId.Value)) {
                InventoryManager.Instance.shopUpgradeUnlocked.Remove(shopUpgradeId.Value);
            }
            Plugin.Log.LogInfo($"Removed {item} from player");
        }

        private void RemoveAllItemsOfType(EItems item, EShopUpgradeID? shopUpgradeId = null) {
            RemoveItem(item, shopUpgradeId, InventoryManager.Instance.GetItemQuantity(item));
        }


    }

    public static class InvincibilityPatch {
        [HarmonyPatch(typeof(PlayerController), "ReceiveHit")]
        [HarmonyPrefix]
        static bool ReceiveHitPrefix(ref HitData __0) {
            if (UnlocksModule.godmode) {
                return false;
            } else {
                if (UnlocksModule.noDamage) __0.damage = 0;
                return true;
            }
        }

        [HarmonyPatch(typeof(PlayerController), "Kill")]
        [HarmonyPrefix]
        static bool KillPrefix() {
            return !UnlocksModule.godmode;
        }
    }

    public static class ShurikenPatch {
        [HarmonyPatch(typeof(PlayerController), "ThrowShuriken")]
        [HarmonyPrefix]
        static bool ThrowShurikenPrefix() {
            PlayerManager.Instance.PlayerShurikens++;
            return true;
        }
    }
}
