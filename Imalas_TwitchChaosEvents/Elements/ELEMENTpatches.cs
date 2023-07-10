﻿using ElementUtilNamespace;
using HarmonyLib;
using ONITwitchLib.Utils;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Elements
{

    public class ELEMENTpatches
    {



        /// <summary>
        /// akis beached 
        /// </summary>
        [HarmonyPatch(typeof(ElementLoader))]
        [HarmonyPatch(nameof(ElementLoader.Load))]
        public class ElementLoader_Load_Patch
        {
            public static void Prefix(Dictionary<string, SubstanceTable> substanceTablesByDlc)
            {
                // Add my new elements
                var list = substanceTablesByDlc[DlcManager.VANILLA_ID].GetList();
                ModElements.RegisterSubstances(list);

                //SgtLogger.l("ElementList length after that method; " + substanceTablesByDlc[DlcManager.VANILLA_ID].GetList().Count);
                //SgtLogger.l("ElementList SO length; " + substanceTablesByDlc[DlcManager.EXPANSION1_ID].GetList().Count);
            }
            public static void Postfix(ElementLoader __instance)
            {
                //SgtLogger.l("ElementList length in postfix; " + ElementLoader.elementTable.Count);
                SgtElementUtil.FixTags();
            }
        }

        // Credit: Heinermann (Blood mod)
        public static class EnumPatch
        {
            [HarmonyPatch(typeof(Enum), "ToString", new Type[] { })]
            public class SimHashes_ToString_Patch
            {
                public static bool Prefix(ref Enum __instance, ref string __result)
                {
                    if (__instance is SimHashes hashes)
                    {
                        return !SgtElementUtil.SimHashNameLookup.TryGetValue(hashes, out __result);
                    }

                    return true;
                }
            }
        }

        //[HarmonyPatch(typeof(GameUtil), "IsEmissionBlocked")]
        //public class GameUtil_IsEmissionBlocked_Patch
        //{
        //    public static bool Prefix(int cell, out bool all_not_gaseous, out bool all_over_pressure)
        //    {
        //        if(Grid.Element[cell].id == ModElements.Creeper)
        //        {
        //            all_not_gaseous = false;
        //            all_over_pressure = false;
        //            return false;
        //        }

        //        all_not_gaseous = true;
        //        all_over_pressure = true;
        //        return true;
        //    }
        //    //public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
        //    //{
        //    //    var codes = orig.ToList();

        //    //    // find injection point
        //    //    var index = codes.FindIndex(ci => ci.opcode == OpCodes.Ldc_R4 && ci.operand is float f && Mathf.Approximately(f,1.8f));

        //    //    if (index == -1)
        //    //    {
        //    //        return codes;
        //    //    }

        //    //    var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(GameUtil_IsEmissionBlocked_Patch), "InjectedMethod");

        //    //    // inject right after the found index
        //    //    codes.InsertRange(index + 1, new[]
        //    //    {
        //    //                new CodeInstruction(OpCodes.Ldloc_3),
        //    //                new CodeInstruction(OpCodes.Call, m_InjectedMethod)
        //    //            });
        //    //    return codes;
        //    //}

        //    //private static float InjectedMethod( float old, Element ele)
        //    //{
        //    //    SgtLogger.l("creep creep ? "+ ele.id);
        //    //    if (ele.id == ModElements.Creeper)
        //    //    {
        //    //        return 1000f;
        //    //    }
        //    //    return old;

        //    //}
        //}

    }
}
