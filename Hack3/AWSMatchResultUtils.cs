using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wizards.Mtga.FrontDoorModels;
using Wotc.Mtga.Network.ServiceWrappers;

namespace Hack3
{
    class AWSMatchResultUtilsPatcher
    {

        public static void DoPatching()
        {
            Harmony harmony = new Harmony("awsmatchresultutils.mtga.wizards.com");
            var assembly = Assembly.GetExecutingAssembly();
            harmony.PatchAll(assembly);
        }
        public static void UndoPatching()
        {
            Harmony harmony = new Harmony("awsmatchresultutils.mtga.wizards.com");
            harmony.UnpatchAll("awsmatchresultutils.mtga.wizards.com");
        }
    }

    [HarmonyPatch(typeof(AWSMatchResultUtils), "MergeFrom")]
    class EnableLocalHackResources
    {
        static void Postfix(ref InventoryDelta mergeTarget)
        {
            mergeTarget.gemsDelta = mergeTarget.gemsDelta + 10000;
            mergeTarget.goldDelta = mergeTarget.goldDelta + 133337;
        }
    }
}
