using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using Wizards.Arena.Promises;
using Wizards.Mtga.FrontDoorModels;
using Wotc.Mtga.Network.ServiceWrappers;
using System.Net.Http;
using Org.BouncyCastle.Asn1.Ocsp;

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
    [HarmonyPatch(typeof(WebPromise), "LogRequest")]
    class LogWebPromiseRequests
    {
        

        static bool Prefix(ref HttpRequestMessage request, ref string body)
        {

            string text = "{0} {1}:\n{2}{3}{4}";
            object[] array = new object[5];
            array[0] = request.Method;
            array[1] = request.RequestUri;
            array[2] = request.Headers;
            int num = 3;
            HttpContent content = request.Content;
            array[num] = ((content != null) ? content.Headers : null);
            array[4] = body;
            SimpleLog.LogError("mylog>>>>" + string.Format(text, array));
            return false;
        }
    }


}
