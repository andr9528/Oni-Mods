﻿using HarmonyLib;
using ExplosiveMaterials.buildings;
using ExplosiveMaterials.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace ExplosiveMaterials
{
    class Patches
    {
        [HarmonyPatch(typeof(RocketEngineCluster.StatesInstance))]
        [HarmonyPatch(nameof(RocketEngineCluster.StatesInstance.DoBurn))]

        public static class AddSomeNukesInThere
        {
            public static void Postfix(float dt, RocketEngineCluster.StatesInstance __instance)
            {
                return;
                if (__instance.master.PrefabID() == NuclearPulseEngineConfig.ID)
                {
                    __instance.master.GetComponent<ExhaustDispenser>().exhaustMethod(
                        dt, __instance,
                        (KBatchedAnimController)Traverse.Create(__instance.master).Field("animController").GetValue(),
                        (int)Traverse.Create(__instance).Field("pad_cell").GetValue());
                }
            }
        }

        [HarmonyPatch(typeof(LaunchableRocketCluster.StatesInstance))]
        [HarmonyPatch(nameof(LaunchableRocketCluster.StatesInstance.SetupLaunch))]
        public static class NukeRocketGoesBrr
        {
            public static void Postfix(LaunchableRocketCluster.StatesInstance __instance)
            {
                if (__instance.master.GetEngines().First().PrefabID() == NuclearPulseEngineConfig.ID)
                {
                    Debug.Log("Nuclear Launch initiated");
                    //var bomblet = Util.KInstantiate(Assets.GetPrefab((Tag)BombletNuclearConfig.ID), __instance.master.transform.position, Quaternion.identity);
                    //bomblet.SetActive(true);
                    //bomblet.GetComponent<ExplosiveBomblet>().Detonate(5f);
                }
            }
        }

        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Radiation, BombBuildingStationConfig.ID);

                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Utilities, PlacableExplosiveConfig.ID);

                RocketryUtils.AddRocketModuleToBuildList(NuclearPulseEngineConfig.ID, RocketryUtils.RocketCategory.engines);
            }
        }

        [HarmonyPatch(typeof(SolidConduitDispenser))]
        [HarmonyPatch(nameof(SolidConduitDispenser.ConduitUpdate))]
        public class ConduitDispenserImplementOneElementTag
        {
            private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
                    typeof(Pickupable),
                   "get_PrimaryElement"
               );
            public static Pickupable CheckForTag(Pickupable original)
            {
                //Debug.Log(original.KPrefabID);
                if (original.KPrefabID.HasTag(ModAssets.Tags.SplitOnRail))
                {
                    original = original.Take(1f);
                }
                return original;
            }

            private static readonly MethodInfo PacketSizeHelper = AccessTools.Method(
               typeof(ConduitDispenserImplementOneElementTag),
               nameof(CheckForTag)
            );

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();

                var insertionIndex = code.FindIndex(ci => ci.operand is MethodInfo f && f == SuitableMethodInfo);


                if (insertionIndex != -1)
                {
                    int primaryElementIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, insertionIndex);

                    code.Insert(insertionIndex, new CodeInstruction(OpCodes.Call, PacketSizeHelper));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Stloc_S, primaryElementIndex));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, primaryElementIndex));
                }
                // Debug.Log("DEBUGMETHOD: " + new CodeInstruction(OpCodes.Call, PacketSizeHelper));
                //TranspilerHelper.PrintInstructions(code);
                return code;
            }
        }
    }
}
