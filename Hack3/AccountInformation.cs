using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Wizards.Mtga.FrontDoorModels;
using Wotc.Mtga.Network.ServiceWrappers;

namespace Hack3
{
    class AccountInformationPatcher
    {

        public static void DoPatching()
        {
            Harmony harmony = new Harmony("accountinfo.mtga.wizards.com");
            var assembly = Assembly.GetExecutingAssembly();
            harmony.PatchAll(assembly);
        }
        public static void UndoPatching()
        {
            Harmony harmony = new Harmony("accountinfo.mtga.wizards.com");
            harmony.UnpatchAll("accountinfo.mtga.wizards.com");
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
    [HarmonyPatch(typeof(AccountInformation), "HasRole_MythicOrange")]
    class EnableWotcRep
    {

        static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }


}
