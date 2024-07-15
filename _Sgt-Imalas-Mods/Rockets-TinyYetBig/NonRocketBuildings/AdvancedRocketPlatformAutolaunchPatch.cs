﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Rockets_TinyYetBig.OldPatches;
using UtilLibs;
using UnityEngine;
using Rockets_TinyYetBig.Behaviours;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
    [HarmonyPatch(typeof(CraftModuleInterface))]
    [HarmonyPatch(nameof(CraftModuleInterface.EvaluateConditionSet))]
    public class AdvancedRocketPlatformAutolaunchPatch
    {
        public static ProcessCondition.Status ConvertWarningIfNeeded(ProcessCondition condition ,CraftModuleInterface craftModuleInterface, ProcessCondition.ProcessConditionType conditionType  )
        {

            var current = craftModuleInterface.CurrentPad;
            if (current != null && current.TryGetComponent(out AdvancedRocketStatusProvider evaluator))
            {
                ///Only convert if it should launch or bit 3 is set (aka apply condition overrides permanently
                if (evaluator.GetBitMaskValAtIndex(0) || evaluator.GetBitMaskValAtIndex(3))
                {
                    if (evaluator.GetBitMaskValAtIndex(1) && condition.GetType() == typeof(ConditionProperlyFueled) && condition.EvaluateCondition() != ProcessCondition.Status.Failure)
                    {
                        return ProcessCondition.Status.Ready;
                    }
                    if (evaluator.GetBitMaskValAtIndex(2) && (conditionType == ProcessCondition.ProcessConditionType.RocketStorage||condition.GetType() == typeof(ConditionHasRadbolts)) && condition.EvaluateCondition() != ProcessCondition.Status.Failure)
                    {
                        return ProcessCondition.Status.Ready;
                    }
                }
            }
            return condition.EvaluateCondition();
        }

        private static readonly MethodInfo ConverterMethod = AccessTools.Method(
          typeof(AdvancedRocketPlatformAutolaunchPatch),
          nameof(ConvertWarningIfNeeded)
       );


        private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
                typeof(ProcessCondition),
                nameof(ProcessCondition.EvaluateCondition)
           );

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            var insertionIndex = code.FindIndex(ci => ci.operand is MethodInfo f && f == SuitableMethodInfo);


            //InjectionMethods.PrintInstructions(code);
            if (insertionIndex != -1)
            {
                //int evaluatedConditionIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, insertionIndex, false);
                code[insertionIndex] = new CodeInstruction(OpCodes.Call, ConverterMethod);
                code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_1));
                code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_0));
                //code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldloc_S,evaluatedConditionIndex));
                //code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_0));
                //TranspilerHelper.PrintInstructions(code);
            }
            else Debug.LogError("AdvancedRocketPlatformAutolaunchPatch transpiler failed");

            return code;
        }

    }
    //[HarmonyPatch(typeof(HEPEngineConfig))]
    //[HarmonyPatch(nameof(HEPEngineConfig.DoPostConfigureComplete))]
    //public class radboltcheat
    //{
    //    public static void Postfix(GameObject go)
    //    {
    //        if (go.TryGetComponent(out RocketModuleCluster evaluator))
    //        {
    //            evaluator.performanceStats.enginePower = 900;
    //        }
    //    }
    //}

    [HarmonyPatch(typeof(LaunchPad))]
    [HarmonyPatch(nameof(LaunchPad.Sim1000ms))]
    public class AdjustForRibbonInput
    {
        public static int ConvertInputValue(int original)
        {
            //SgtLogger.l(original.ToString(),"INPUT VAL");
            //SgtLogger.l((original%2).ToString(),"OUTPUT VAL");
            return original % 2;
        }

        private static readonly MethodInfo ConverterMethod = AccessTools.Method(
           typeof(AdjustForRibbonInput),
           nameof(ConvertInputValue)
        );


        private static readonly MethodInfo SuitableMethodInfo = AccessTools.Method(
                typeof(LogicPorts),
                nameof(LogicPorts.GetInputValue)
           );

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            var insertionIndex = code.FindIndex(ci => ci.operand is MethodInfo f && f == SuitableMethodInfo);


            //InjectionMethods.PrintInstructions(code);
            if (insertionIndex != -1)
            {
                code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, ConverterMethod));
            }
            else
                Debug.LogError("Rocketry Expanded: Launchpad transpiler failed");

            return code;
        }
    }
}
