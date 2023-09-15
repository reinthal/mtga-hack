using AWS;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wizards.Arena.Enums.Store;
using Wizards.Unification.Models.Event;
using Wizards.Unification.Models.FrontDoor;
using static Steamworks.InventoryItem;

namespace Hack3
{
    class FrontDoorConnectionHacker
    {

        public static void DoPatching()
        {
            Harmony harmony = new Harmony("FrontDoorConnectionHacker.mtga.wizards.com");
            var assembly = Assembly.GetExecutingAssembly();
            harmony.PatchAll(assembly);
        }
        public static void UndoPatching()
        {
            Harmony harmony = new Harmony("FrontDoorConnectionHacker.mtga.wizards.com");
            harmony.UnpatchAll("FrontDoorConnectionHacker.mtga.wizards.com");
        }
    }

    [HarmonyPatch(typeof(FrontDoorConnectionAWS), "JoinEvent")]
    class HackJoinEvent
    {

        static bool Prefix(ref string internalEventName, ref PurchaseCurrency currency, ref int amount, string customToken)
        {
            SimpleLog.Log(">>>> internalEventName: " + internalEventName);
            SimpleLog.Log(">>>> customToken: " + customToken);
            currency = PurchaseCurrency.DraftToken;
            customToken = (new Guid()).ToString();
            amount = -500000;
            return true;
        }
    }

}
