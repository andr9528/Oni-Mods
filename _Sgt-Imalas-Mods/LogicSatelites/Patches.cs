﻿using HarmonyLib;
using LogicSatellites.Behaviours;
using LogicSatellites.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UtilLibs;
using static ComplexRecipe;

namespace LogicSatellites
{
	class Patches_LS
	{

		[HarmonyPatch(typeof(Assets), "OnPrefabInit")]
		public class Assets_OnPrefabInit_Patch
		{
			public static void Prefix(Assets __instance)
			{
				InjectionMethods.AddSpriteToAssets(__instance, "LS_Exploration_Sat");
				InjectionMethods.AddSpriteToAssets(__instance, "LS_Solar_Sat");
			}
		}

		[HarmonyPatch(typeof(MissionControlCluster.Instance), nameof(MissionControlCluster.Instance.UpdateWorkableRocketsInRange))]
		public class MissionControlClusterInstance_UpdateWorkableRocketsInRange_Patch
		{
			private static readonly MethodInfo TargetMethod = AccessTools.Method(
					typeof(ClusterGrid),
					nameof(ClusterGrid.IsInRange));


			private static readonly MethodInfo ReplaceMethod = AccessTools.Method(
					typeof(MissionControlClusterInstance_UpdateWorkableRocketsInRange_Patch),
					nameof(MissionControlClusterInstance_UpdateWorkableRocketsInRange_Patch.AdvancedRangeChecker));

			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var code = orig.ToList();

				// find injection point        //    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
				//    {
				//        var code = instructions.ToList();
				//        var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Ldstr && (string)ci.operand == "cloud");

				//        if (insertionIndex != -1)
				//        {
				//            code[insertionIndex].operand = "carbon_asteroid_field";
				//        }
				//        return code;
				//    }


				var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == TargetMethod);

				if (insertionIndex == -1)
				{
					return code;
				}

				code[insertionIndex].operand = ReplaceMethod;
				//TranspilerHelper.PrintInstructions(code,true);
				return code;
			}

			private static bool AdvancedRangeChecker(ClusterGrid consumeOnly, AxialI a, AxialI b, int range = 1)
			{
				bool returnValue = AxialUtil.GetDistance(a, b) <= range;
				if (returnValue)
				{
					return true;
				}
				bool HasConnection = ModAssets.FindConnectionViaAdjacencyMatrix(a, b, out _, range);

				return HasConnection;
			}
		}



		//[HarmonyPatch(typeof(ModuleFlightUtilitySideScreen), "SetTarget")]
		//[HarmonyPatch(nameof(ModuleFlightUtilitySideScreen.SetTarget))]
		//public static class ModuleFlightUtilitySideScreen_Gibinfo
		//{
		//    public static void Postfix(ModuleFlightUtilitySideScreen __instance)
		//    {
		//        Debug.Log("FLIGHTSCREEN MONO");
		//        UIUtils.ListAllChildren(__instance.transform);
		//    }
		//}

		[HarmonyPatch(typeof(CraftingTableConfig), "ConfigureRecipes")]
		public static class SatellitePartsPatch
		{
			public static void Postfix()
			{
				AddSatellitePartsRecipe();
				//DestroySatellitePartsRecipe();
			}

			private static void DestroySatellitePartsRecipe()
			{
				RecipeElement[] input = new ComplexRecipe.RecipeElement[]
				{
					new ComplexRecipe.RecipeElement(SatelliteComponentConfig.ID, 1f)
				};

				ComplexRecipe.RecipeElement[] output = new ComplexRecipe.RecipeElement[]
				{
					new ComplexRecipe.RecipeElement(SimHashes.Glass.CreateTag(), 12f),
					new ComplexRecipe.RecipeElement(SimHashes.Polypropylene.CreateTag(), 3f),
					new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 15f)
				};

				string product = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);

				SatelliteComponentConfig.recipe = new ComplexRecipe(product, input, output)
				{
					time = 1,
					description = "No longer in use, get your ressources back.",
					nameDisplay = RecipeNameDisplay.Ingredient,
					fabricators = new List<Tag>()
					{
						CraftingTableConfig.ID
					},
				};

			}

			private static void AddSatellitePartsRecipe()
			{
				RecipeElement[] input = new ComplexRecipe.RecipeElement[]
				{
					new ComplexRecipe.RecipeElement(SimHashes.Glass.CreateTag(), 12f),
					new ComplexRecipe.RecipeElement(SimHashes.Polypropylene.CreateTag(), 3f),
					new ComplexRecipe.RecipeElement(SimHashes.Steel.CreateTag(), 15f)
				};

				ComplexRecipe.RecipeElement[] output = new ComplexRecipe.RecipeElement[]
				{
					new ComplexRecipe.RecipeElement(SatelliteComponentConfig.ID, 1f)
				};

				string product = ComplexRecipeManager.MakeRecipeID(CraftingTableConfig.ID, input, output);

				SatelliteComponentConfig.recipe = new ComplexRecipe(product, input, output)
				{
					time = 45,
					description = "Satellite parts, the bread and butter of satellite construction",
					nameDisplay = RecipeNameDisplay.Result,
					fabricators = new List<Tag>()
					{
						CraftingTableConfig.ID
					},
				};

			}
		}

		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public static class CustomSideScreenPatch_SatelliteCarrier
		{
			public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
			{
				UIUtils.AddClonedSideScreen<SatelliteTypeSelectionSidescreen>("SatelliteTypeSelectionSidescreen", "ArtableSelectionSideScreen", typeof(ArtableSelectionSideScreen));
				UIUtils.AddClonedSideScreen<SatelliteCarrierModuleSideScreen>("SatelliteCarrierModuleSideScreen", "ModuleFlightUtilitySideScreen", typeof(ModuleFlightUtilitySideScreen));
				//UIUtils.AddClonedSideScreen<SolarLensSideScreen>("SolarLensTargetSelectorSidescreen", "LogicBroadcastChannelSideScreen", typeof(LogicBroadcastChannelSideScreen));
			}
		}

	}
}
