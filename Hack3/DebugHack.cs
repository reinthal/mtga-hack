using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;


namespace Hack3
{
    class MtgaPatcher
    {

        public static void DoPatching()
        {
            Harmony harmony = new Harmony("mtga.wizards.com");
            harmony.PatchAll();
        }
        public static void UndoPatching()
        {
            Harmony harmony = new Harmony("mtga.wizards.com");
            harmony.UnpatchAll();
        }
    }

    [HarmonyPatch(typeof(AccountInformation), "HasRole_Debugging")]
    class EnableDebug
    {

        static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
