using HarmonyLib;
using Klei.AI;
using KnastoronOniMods;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UtilLibs;

namespace RoboRockets
{
	public class RoboRocketPatches
	{
		/// <summary>
		/// Add the tag to the entity so it works for recipes
		/// </summary>
		[HarmonyPatch(typeof(GeneShufflerRechargeConfig), nameof(GeneShufflerRechargeConfig.CreatePrefab))]
		public static class AddTagToItem
		{
			public static void Postfix(ref GameObject __result)
			{
				var prefab = __result.GetComponent<KPrefabID>();
				prefab.AddTag(GeneShufflerRechargeConfig.tag);
			}
		}

		/// <summary>
		/// Extend the limitOneCommandModule condition to include AI modules as command modules
		/// </summary>
		[HarmonyPatch(typeof(LimitOneCommandModule), nameof(LimitOneCommandModule.EvaluateCondition))]
		public static class LimitOneCommandModule_AI_Patch
		{
			public static bool Prefix(bool __result,
				GameObject existingModule,
				SelectModuleCondition.SelectionContext selectionContext)
			{

				if (existingModule == null || !existingModule.TryGetComponent<AttachableBuilding>(out _))
				{
					__result = true;
					return false;
				}


				foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(existingModule.GetComponent<AttachableBuilding>()))
				{
					if (
						(selectionContext != SelectModuleCondition.SelectionContext.ReplaceModule || !(gameObject == existingModule.gameObject))
						&& (
						gameObject.TryGetComponent<RocketCommandConditions>(out _) ||
						gameObject.TryGetComponent<RocketAiConditions>(out _) ||
						gameObject.TryGetComponent<BuildingUnderConstruction>(out var underConstruction) && underConstruction.Def.BuildingComplete.TryGetComponent<RocketCommandConditions>(out _) ||
						gameObject.TryGetComponent<BuildingUnderConstruction>(out var complete) && complete.Def.BuildingComplete.TryGetComponent<RocketAiConditions>(out _)
						)
					 )
					{
						__result = false;
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Skip Passenger check for AI modules
		/// </summary>
		[HarmonyPatch(typeof(PassengerRocketModule), nameof(PassengerRocketModule.CheckPassengersBoarded))]
		public class PassengerRocketModule_CheckPassengersBoarded_Patch
		{
			public static void Postfix(PassengerRocketModule __instance, ref bool __result)
			{
				if (__instance.GetType() == typeof(AIPassengerModule))
				{
					__result = true;
				}
			}
		}

		/// <summary>
		/// Skip access check for AI modules
		/// </summary>
		[HarmonyPatch(typeof(PassengerRocketModule), nameof(PassengerRocketModule.RefreshAccessStatus))]
		public class PassengerRocketModule_RefreshAccessStatus_Patch
		{
			[HarmonyPriority(Priority.VeryHigh)]
			public static bool Prefix(PassengerRocketModule __instance)
			{
				if (__instance.GetType() == typeof(AIPassengerModule))
				{
					return false;
				}
				return true;
			}
		}
		/// <summary>
		/// Hide Crew screen for ai rockets
		/// </summary>
		[HarmonyPatch(typeof(RequestCrewSideScreen), nameof(RequestCrewSideScreen.IsValidForTarget))]
		public class RequestCrewSideScreen_IsValidForTarget_Patch
		{
			public static bool Prefix(GameObject target, ref bool __result)
			{
				if (target.TryGetComponent<PassengerRocketModule>(out var passengerRocketModule) && passengerRocketModule is AIPassengerModule)
				{
					__result = false;
					return false;
				}
				return true;
			}
		}

		/// <summary>
		/// skip pilot check for ai rockets
		/// </summary>
		[HarmonyPatch(typeof(PassengerRocketModule), nameof(PassengerRocketModule.CheckPilotBoarded))]
		public class PassengerRocketModule_CheckPilotBoarded_Patch
		{
			public static void Postfix(PassengerRocketModule __instance, ref bool __result)
			{
				if (__instance.GetType() == typeof(AIPassengerModule))
				{
					__result = true;
				}
			}
		}

		[HarmonyPatch(typeof(RocketControlStation.States), nameof(RocketControlStation.States.CreateChore))]
		public class RocketControlStation_CreateChore_Patch
		{
			public static void Postfix(RocketControlStation.StatesInstance smi, ref Chore __result)
			{
				if (smi.master.GetType() == typeof(RocketControlStationNoChorePrecondition))
				{
					Workable component = (Workable)smi.master.GetComponent<RocketControlStationLaunchWorkable>();
					WorkChore<RocketControlStationIdleWorkable> chore =
						new WorkChore<RocketControlStationIdleWorkable>(Db.Get().ChoreTypes.RocketControl,
						(IStateMachineTarget)component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Work,
						allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
					chore.AddPrecondition(ChorePreconditions.instance.ConsumerHasTrait, AiBrainConfig.ROVER_BASE_TRAIT_ID);
					__result = (Chore)chore;
				}
			}
		}

		[HarmonyPatch(typeof(RocketControlStation.States), nameof(RocketControlStation.States.CreateLaunchChore))]
		public class RocketControlStation_CreateLaunchChore_Patch
		{
			public static void Postfix(RocketControlStation.StatesInstance smi, ref Chore __result)
			{
				if (smi.master.GetType() == typeof(RocketControlStationNoChorePrecondition))
				{
					Workable component = (Workable)smi.master.GetComponent<RocketControlStationLaunchWorkable>();
					WorkChore<RocketControlStationLaunchWorkable> launchChore =
						new WorkChore<RocketControlStationLaunchWorkable>(Db.Get().ChoreTypes.RocketControl,
						(IStateMachineTarget)component, ignore_schedule_block: true, allow_prioritization:
						false, priority_class: PriorityScreen.PriorityClass.topPriority);
					launchChore.AddPrecondition(ChorePreconditions.instance.ConsumerHasTrait, AiBrainConfig.ROVER_BASE_TRAIT_ID);
					__result = (Chore)launchChore;
				}
			}
		}
		[HarmonyPatch(typeof(RocketControlStation.StatesInstance), nameof(RocketControlStation.StatesInstance.SetPilotSpeedMult))]
		public class RocketControlStation_SetPilotSpeedMult_Patch
		{
			public static bool Prefix(Worker pilot, RocketControlStation.StatesInstance __instance)
			{
				AttributeConverter pilotingSpeed = Db.Get().AttributeConverters.PilotingSpeed;
				if (pilot.GetComponent<AttributeConverters>().GetConverter(pilotingSpeed.Id) == null)
				{
#if DEBUG
                    SgtLogger.l("skippingNormalSpeedSetter in Legacy AI Rocket");
#endif
					__instance.pilotSpeedMult = Config.Instance.NoBrainRockets;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(HabitatModuleSideScreen), nameof(HabitatModuleSideScreen.RefreshModulePanel))]
		public class DisableViewInteriorSpace_Patch
		{
			public static void Postfix(PassengerRocketModule module, HabitatModuleSideScreen __instance)
			{
				bool allowed = !module.gameObject.TryGetComponent<AIPassengerModule>(out _);

				HierarchyReferences component = __instance.GetComponent<HierarchyReferences>();
				KButton reference = component.GetReference<KButton>("button");
				if (!allowed)
					reference.ClearOnClick();
				reference.isInteractable = allowed;
			}
		}

		[HarmonyPatch(typeof(CameraController), nameof(CameraController.ActiveWorldStarWipe))]
		[HarmonyPatch(new System.Type[] { typeof(int), typeof(System.Action) })]
		public class DisableViewInteriorWorldSelector_OnClickStarmap_Patch
		{
			public static bool Prefix(int id)
			{
				if (ModAssets.ForbiddenInteriorIDs.ContainsKey(id))
				{
					SgtLogger.l("WorldID is forbidden to look into: " + id);
					return false;
				}
				return true;
			}
		}
		[HarmonyPatch(typeof(WorldSelector), nameof(WorldSelector.OnWorldRowClicked))]
		public class DisableViewInteriorWorldSelector_Patch
		{
			public static bool Prefix(int id)
			{

				if (ModAssets.ForbiddenInteriorIDs != null && ModAssets.ForbiddenInteriorIDs.ContainsKey(id))
				{
#if DEBUG
                    SgtLogger.l("WorldID is forbidden to look into: " + id);
#endif
					AIPassengerModule Module = ModAssets.ForbiddenInteriorIDs[id];

					if (Module != null && Module.gameObject != null && Module.TryGetComponent<KPrefabID>(out var prefabID) && !prefabID.HasTag(GameTags.RocketOnGround))
					{
						if (ClusterMapScreen.Instance != null && !ClusterMapScreen.Instance.gameObject.activeSelf && ManagementMenu.Instance != null)
							ManagementMenu.Instance.ToggleClusterMap();

						if (Module.TryGetComponent<RocketModuleCluster>(out var module) && module.CraftInterface.TryGetComponent<ClusterGridEntity>(out var entity))
						{
							ClusterMapScreen.Instance.SetTargetFocusPosition(entity.Location, 0);
							// ClusterMapScreen.Instance.SelectEntity(entity, true);
						}
					}
					else
					{
						if (Module != null && Module.gameObject != null)
						{

							if (ClusterMapScreen.Instance != null && ClusterMapScreen.Instance.gameObject.activeSelf && ManagementMenu.Instance != null)
								ManagementMenu.Instance.ToggleClusterMap();
							System.Action OnSelect = null;
							if (Module.TryGetComponent<KSelectable>(out var selectable))
							{
								OnSelect = () => SelectTool.Instance.Select(selectable);
							}

							CameraController.Instance.ActiveWorldStarWipe(ClusterManager.Instance.GetWorld(id).ParentWorldId, true, Module.gameObject.transform.position, 10f, OnSelect);
						}
					}

					return false;
				}
				return true;
			}
		}


		[HarmonyPatch(typeof(ClustercraftExteriorDoor), nameof(ClustercraftExteriorDoor.OnSpawn))]
		public class AddInteriorToForbiddenListIfAI
		{
			public static void Postfix(ClustercraftExteriorDoor __instance)
			{
				if (__instance.gameObject.TryGetComponent<AIPassengerModule>(out var aiModue)
					&& __instance.gameObject.TryGetComponent<RocketModuleCluster>(out RocketModuleCluster rocketModuleCluster))
				{
					rocketModuleCluster.CraftInterface.TryGetComponent<WorldContainer>(out var craftWorld);
					int worldRefID = craftWorld.id;
					if (!aiModue.variableSpeed)
					{
#if DEBUG
                        SgtLogger.l("AI Module added; adjusting automated Speed to " + Config.Instance.NoBrainRockets);
#endif
						rocketModuleCluster.CraftInterface.m_clustercraft.AutoPilotMultiplier = Config.Instance.NoBrainRockets;
					}

					SgtLogger.l("World forbidden to look into: " + worldRefID);

					if (!ModAssets.ForbiddenInteriorIDs.ContainsKey(worldRefID))
						ModAssets.ForbiddenInteriorIDs.Add(worldRefID, aiModue);
				}
			}
		}
		[HarmonyPatch(typeof(ClustercraftExteriorDoor), nameof(ClustercraftExteriorDoor.OnCleanUp))]
		public class RemoveInteriorFromForbiddenListIfAI
		{
			public static void Prefix(ClustercraftExteriorDoor __instance)
			{
				if (__instance.gameObject.GetComponent<AIPassengerModule>() != null)
				{
					int worldRefID = (int)typeof(ClustercraftExteriorDoor).GetField("targetWorldId", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

					if (ModAssets.ForbiddenInteriorIDs.ContainsKey(worldRefID))
						ModAssets.ForbiddenInteriorIDs.Remove(worldRefID);
				}
			}
		}

		[HarmonyPatch(typeof(Game), nameof(Game.DestroyInstances))]
		public class Clear_ForbiddenList
		{
			public static void Prefix()
			{
				ModAssets.ForbiddenInteriorIDs.Clear();
			}
		}

		[HarmonyPatch(typeof(ClustercraftExteriorDoor), nameof(ClustercraftExteriorDoor.HasTargetWorld))]
		public class DisableViewInterior_Patch
		{
			public static void Postfix(ClustercraftExteriorDoor __instance, ref bool __result)
			{
				if (__instance.gameObject.GetComponent<AIPassengerModule>() != null)
				{
					__result = false;
				}

			}
		}

		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.RequestLaunch))]
		public class TriggerLaunchForAIRocketsPatch
		{
			public static bool Prefix(Clustercraft __instance)
			{
				if (__instance == null)
					return true;
				foreach (Ref<RocketModuleCluster> clusterModule in __instance.ModuleInterface.ClusterModules)
				{
					if (clusterModule.Get().TryGetComponent<AIPassengerModule>(out _))
					{
						if (__instance.ModuleInterface.CheckPreppedForLaunch())
							__instance.Launch();
						return false;
					}
				}
				return true;
			}
		}


		/// <summary>
		/// Set interior size to bare minimum
		/// </summary>
		[HarmonyPatch(typeof(ClusterManager), nameof(ClusterManager.CreateRocketInteriorWorld))]
		public class ClusterManager_CreateRocketInteriorWorld_Patch
		{

			public static Vector2I ConditionForSize(Vector2I original, string templateString)
			{
				if (templateString.Contains("habitat_robo"))
					original = new Vector2I(8, 8);

				else if (templateString.Contains("AIRocketV2"))
					original = new Vector2I(1, 2);

				return original;
			}

			private static readonly MethodInfo InteriorSizeHelper = AccessTools.Method(
			   typeof(ClusterManager_CreateRocketInteriorWorld_Patch),
			   nameof(ConditionForSize)
			);


			private static readonly FieldInfo SizeFieldInfo = AccessTools.Field(
				typeof(TUNING.ROCKETRY),
				"ROCKET_INTERIOR_SIZE"
			);

			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			{
				var code = instructions.ToList();
				var insertionIndex = code.FindIndex(ci => ci.operand is FieldInfo f && f == SizeFieldInfo);

				if (insertionIndex != -1)
				{
					code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldarg_2));
					code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, InteriorSizeHelper));
				}

				return code;
			}
		}
	}
}
