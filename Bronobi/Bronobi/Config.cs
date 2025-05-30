
using UnityEngine;

namespace BronobiMod
{
    internal static class Config
    {
        public static bool addLife = true;
        public static bool refillAmmos = true;
        public static bool givePockettedAmmo = true;
        public static bool giveFlexPower = true;

        public static bool respawnFriend = true;
        public static bool useProcGen = false;
        public static bool spawnDrone = false;
        public static bool spawnPig = false;

        public static void UI()
        {
            GUILayout.Label("Bronobi Ghost Gifts:");
            GUILayout.Label("--Some options may need additionals check--");
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            addLife = GUILayout.Toggle(addLife, new GUIContent("Add Life"));
            refillAmmos = GUILayout.Toggle(refillAmmos, new GUIContent("Refill Special"));
            givePockettedAmmo = GUILayout.Toggle(givePockettedAmmo, new GUIContent("Give Pocketted Ammo"));
            giveFlexPower = GUILayout.Toggle(giveFlexPower, new GUIContent("Give Flex Power"));
            GUILayout.EndHorizontal(); GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
            //respawnFriend = GUILayout.Toggle(spawnPig, new GUIContent("Respawn Patriotic Friend"));
            spawnPig = GUILayout.Toggle(spawnPig, new GUIContent("Spawn Pig"));
            useProcGen = GUILayout.Toggle(useProcGen, new GUIContent("Proc Gen Items"));
           // spawnDrone = GUILayout.Toggle(spawnDrone, new GUIContent("Spawn Drone"));
            GUILayout.EndHorizontal();

        }
    }
}
