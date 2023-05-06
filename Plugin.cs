using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Gex.Library;
using Gex.Modules;
using HarmonyLib;
using UnityEngine;

namespace Gex
{

    [BepInPlugin("no.gentle.plugin.gex", "Gex", "1.0.0")]
    public class Plugin : BaseUnityPlugin {
        internal static Plugin Instance { get; private set; } = null;
        internal static ManualLogSource Log { get; private set; } = null;
        internal static ConfigFile ConfigInstance { get; private set; } = null;
        public static Harmony harmony = new(PluginInfo.PLUGIN_NAME);
        public static BepInEx.PluginInfo pluginInfo;

        private ItemGUIModule itemGui;
        private FreeCamModule camController;
        private ObjectFinder objectFinder;
        private PlayerStateModule playerStateModule;
        
        public ConfigEntry<bool> configBlazeLBUpdates;

        private void Awake() {
            //Internals;
            Instance = this;
            Log = Logger;
            pluginInfo = Info;
            ConfigInstance = Config;

            //Initialize custom components
            itemGui = Instance.gameObject.AddComponent<ItemGUIModule>();
            camController = Instance.gameObject.AddComponent<FreeCamModule>();
            objectFinder = Instance.gameObject.AddComponent<ObjectFinder>();
            playerStateModule = Instance.gameObject.AddComponent<PlayerStateModule>();

            //Initialize statics
            Drawer.InitializeDrawer();
        
            //We loaded
            Log.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        //Cleanup for bepinex script system
        private void OnDestroy() {
            Destroy(itemGui);
            Destroy(camController);
            Destroy(objectFinder);
            Destroy(playerStateModule);
        }

        private void OnGUI() {
            GUILayout.BeginVertical();
            GUILayout.Label("F1 - Unlocks");
            GUILayout.Label("F2 - Free cam");
            GUILayout.Label("F3 - Statemanager");
            GUILayout.EndVertical();
        }
    }

}
