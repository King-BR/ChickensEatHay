using HarmonyLib;
using System;
using UnityEngine;

namespace ChickensEatHay
{
    #region BepInEx
    [BepInEx.BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class ChickensEatHay : BepInEx.BaseUnityPlugin
    {
        public const string pluginGuid = "com.KingBR.ChickensEatHay";
        public const string pluginName = "ChickensEatHay";
        public const string pluginVersion = "1.0";
        private Harmony _harmony;

        public static void Log(string line)
        {
            Debug.Log("[" + pluginName + "]: " + line);
        }

        void Awake()
        {
            try
            {
                _harmony = new Harmony(pluginGuid);
                _harmony.PatchAll();
                Log(pluginName + " succeeded");

            }
            catch (Exception e)
            {

                Log(pluginName + " Failed");
                Log(e.ToString());
            }
        }

        private void OnDestroy()
        {
            _harmony.UnpatchSelf();

            Log(pluginName + " unloaded.");
        }
    }
    #endregion
}
