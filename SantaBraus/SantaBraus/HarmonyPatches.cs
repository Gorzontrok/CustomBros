using BroMakerLib.Loggers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SantaBrausMod
{
    public static class HarmonyPatches
    {
        [HarmonyPatch(typeof(Map), "GetNearbyGrenade")]
        [HarmonyPostfix]
        public static void Map_GetNearbyGrenadePatch(float range, float x, float y)
        {
            BMLogger.Warning("SANTA BRAUSSSSSSS");
            //return true;
        }
    }
}
