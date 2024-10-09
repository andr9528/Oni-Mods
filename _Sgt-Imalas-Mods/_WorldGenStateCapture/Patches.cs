﻿using HarmonyLib;
using Klei.CustomSettings;
using ProcGen;
using System.IO;
using static STRINGS.INPUT_BINDINGS;
using UnityEngine.Networking;
using ProcGenGame;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

namespace _WorldGenStateCapture
{
	internal class Patches
	{
		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			private static void OverLoadStrings()
			{
				string code = Localization.GetLocale()?.Code;

				if (code.IsNullOrWhiteSpace()) return;

				string path = System.IO.Path.Combine(ModAssets.ModPath, "translations", Localization.GetLocale().Code + ".po");

				if (File.Exists(path))
				{
					Localization.OverloadStrings(Localization.LoadStringsFile(path, false));
					Debug.Log($"Found translation file for {code}.");
				}
			}
			public static void Postfix()
			{
				var root = typeof(STRINGS);
				Localization.RegisterForTranslation(typeof(STRINGS));
				OverLoadStrings();
				LocString.CreateLocStringKeys(root, null);
				Localization.GenerateStringsTemplate(root, System.IO.Path.Combine(KMod.Manager.GetDirectory(), "strings_templates"));

			}
		}



		#region SkipOrTheGameCrashesDueToNoSave

		[HarmonyPatch(typeof(RetireColonyUtility), nameof(RetireColonyUtility.SaveColonySummaryData))]
		public static class PreventSavingWorldRetiredColonyData
		{
			public static bool Prefix(ref bool __result)
			{
				__result = true;
				return false;
			}
		}
		[HarmonyPatch(typeof(RetireColonyUtility), nameof(RetireColonyUtility.StripInvalidCharacters))]
		public static class PreventCrashOnReveal
		{
			public static bool Prefix(ref string __result, string source)
			{
				__result = source;
				return false;
			}
		}
		[HarmonyPatch(typeof(Timelapser), nameof(Timelapser.RenderAndPrint))]
		public static class PreventCrashOnReveal2
		{
			public static bool Prefix()
			{
				return false;
			}
		}
		[HarmonyPatch(typeof(SaveLoader), nameof(SaveLoader.InitialSave))]
		public static class PreventSavingWorld
		{
			public static bool Prefix(SaveLoader __instance)
			{
				return false;
			}
		}
		#endregion

		[HarmonyPatch(typeof(WattsonMessage), nameof(WattsonMessage.OnDeactivate))]
		public static class QuitGamePt2
		{
			public static void Postfix()
			{

				Debug.Log("gathering world data...");
				foreach (var world in ClusterManager.Instance.m_worldContainers)
				{
					world.isDiscovered = true;
				}


				for (int i = 0; i < SaveGame.Instance.worldGenSpawner.spawnables.Count; i++)
				{
					SaveGame.Instance.worldGenSpawner.spawnables[i].TrySpawn();
				}


				GameScheduler.Instance.ScheduleNextFrame("collect data", (_) =>
				{
					ModAssets.AccumulateSeedData();
				});
			}

		}
		[HarmonyPatch(typeof(WattsonMessage), nameof(WattsonMessage.OnActivate))]
		public static class QuitGamePt1
		{
			public static void Postfix(WattsonMessage __instance)
			{
				__instance.button.SignalClick(KKeyCode.Mouse2);
			}
		}



		[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.OnSpawn))]

		public static class AutoLaunchParser
		{
			[HarmonyPriority(Priority.Low)]
			public static void Postfix(MainMenu __instance)
			{
				bool shouldAutoStart = ShoulDoAutoStartParsing(out _);


				if (ModAssets.ModDilution)
				{
					Debug.LogWarning("other active mods detected, aborting auto world parsing");
					return;
				}


				var config = Config.Instance;
				if (!shouldAutoStart)
				{
					var startParsingBTN = Util.KInstantiateUI(__instance.Button_NewGame.gameObject, __instance.Button_NewGame.transform.parent.gameObject, true);
					startParsingBTN.name = "start parsing";
					if (startParsingBTN.TryGetComponent<KButton>(out var btn))
					{
						btn.ClearOnClick();
						btn.onClick += () =>
						{
							ToggleAutoParse(true);
							ReduceRemainingRuns();
							InitAutoStart(__instance);
						};
					}
					LocText componentInChildren = startParsingBTN.GetComponentInChildren<LocText>();
					componentInChildren.text = STRINGS.STARTPARSING;
				}


				if (config.ContinuousParsing)
				{
					InitAutoStart(__instance);
					return;
				}

				if (shouldAutoStart)
				{
					ReduceRemainingRuns();
					InitAutoStart(__instance);
				}
				else
					ToggleAutoParse(false);
			}
			public static string RegistryKey = "SeedParsing_RemainingRuns";
			public static bool ShoulDoAutoStartParsing(out int remainingRuns)
			{
				remainingRuns = KPlayerPrefs.GetInt(RegistryKey);
				return remainingRuns > 0;
			}
			public static void ReduceRemainingRuns()
			{
				int remaining = KPlayerPrefs.GetInt(RegistryKey);
				remaining--;
				KPlayerPrefs.SetInt(RegistryKey, remaining);

			}
			public static void ToggleAutoParse(bool enable)
			{

				KPlayerPrefs.SetInt(RegistryKey, enable ? Config.Instance.TargetNumber : -1);

			}



			public static void InitAutoStart(MainMenu __instance)
			{
				autoLoadActive = false;
				var clusterPrefix = DlcManager.IsExpansion1Active() ? Config.Instance.TargetCoordinateDLC : Config.Instance.TargetCoordinateBase;

				targetLayout = null;
				Debug.Log("autostarting...");

				if (Config.Instance.RandomizedClusterGen)
				{
					while (targetLayout == null)
					{
						targetLayout = SettingsCache.clusterLayouts.clusterCache.GetRandom().Value;

						//skip empty dev clusters
						if (targetLayout.skip > 0)
						{
							targetLayout = null;
							continue;
						}
						//spaced out loads vanilla terra to the cache, skip that
						if (DlcManager.IsExpansion1Active() && targetLayout.coordinatePrefix == "SNDST-A")
						{
							targetLayout = null;
							continue;
						}
					}
				}
				else
				{
					foreach (string clusterName in SettingsCache.GetClusterNames())
					{
						ClusterLayout clusterData = SettingsCache.clusterLayouts.GetClusterData(clusterName);
						if (clusterData.coordinatePrefix == clusterPrefix)
						{
							targetLayout = clusterData;
							break;
						}
					}
				}

				if (targetLayout == null)
					return;

				Debug.Log("autostart successful");
				autoLoadActive = true;
				clusterCategory = (int)targetLayout.clusterCategory;
				__instance.NewGame();

			}
		}
		static ClusterLayout targetLayout;
		static int clusterCategory = -1;
		static bool autoLoadActive;


		[HarmonyPatch(typeof(ClusterCategorySelectionScreen), nameof(ClusterCategorySelectionScreen.OnSpawn))]
		public static class SelectClusterType
		{
			public static void Postfix(ClusterCategorySelectionScreen __instance)
			{
				if (autoLoadActive && clusterCategory != -1)
				{
					__instance.OnClickOption((ClusterLayout.ClusterCategory)clusterCategory);
				}
			}
		}
		[HarmonyPatch(typeof(ModeSelectScreen), nameof(ModeSelectScreen.OnSpawn))]
		public static class SelectSurvivalSettings
		{
			public static void Postfix(ModeSelectScreen __instance)
			{
				if (autoLoadActive)
				{
					__instance.OnClickSurvival();

				}
			}
		}
		[HarmonyPatch(typeof(ColonyDestinationSelectScreen), nameof(ColonyDestinationSelectScreen.OnSpawn))]
		public static class SelectCluster
		{
			public static void Postfix(ColonyDestinationSelectScreen __instance)
			{
				if (autoLoadActive)
				{
					__instance.newGameSettingsPanel.SetSetting((SettingConfig)CustomGameSettingConfigs.ClusterLayout, targetLayout.filePath);

					Debug.Log("Selected cluster: "+ Strings.Get(targetLayout.name));
					__instance.newGameSettingsPanel.ConsumeStoryTraitsCode("0");

					if (DlcManager.IsContentSubscribed(DlcManager.DLC2_ID))
					{
						//ceres clusters require dlc mixing to be enabled
						if (Config.Instance.RandomMixing == false)
						{
							Debug.Log("Mixing disabled by config");
							//if its a ceres cluster, turn off any mixing, otherwise leave them at default (adjust in the future if klei releases a second mixing dlc, rn ceres comes with everything disabled by default)
							if (!targetLayout.requiredDlcIds.Contains(DlcManager.DLC2_ID))
							{
								__instance.newGameSettingsPanel.ConsumeMixingSettingsCode("0");
							}
						}
						else
						{
							var settingsInstance = CustomGameSettings.Instance;
							foreach (var setting in CustomGameSettings.Instance.MixingSettings.Values)
							{
								if (setting.coordinate_range == -1) //settings that cannot be configured
								{
									continue;
								}
								if (setting is DlcMixingSettingConfig dlcSetting) //FP setting
								{
									Debug.Log("Turning on dlc mixing "+setting.id);
									settingsInstance.SetMixingSetting(dlcSetting, DlcMixingSettingConfig.EnabledLevelId);
									continue;
								}
								else if(setting is MixingSettingConfig mixingSetting)
								{
									//disable if forbidden by cluster
									if(targetLayout.clusterTags.Any(tag => mixingSetting.forbiddenClusterTags.Contains(tag)))
									{
										Debug.Log(setting.id + " is forbidden by the current cluster, disabling it.");
										settingsInstance.SetMixingSetting(mixingSetting, SubworldMixingSettingConfig.DisabledLevelId); //same id for world and subworld mixing setting
									}
									else
									{
										string randomLevel = mixingSetting.levels.GetRandom().id;
										Debug.Log("setting a random value for mixing setting "+setting.id + ": "+ randomLevel);
										settingsInstance.SetMixingSetting(mixingSetting, randomLevel);
									}
								}
							}
						}
					}

					__instance.ShuffleClicked();
					__instance.LaunchClicked();
				}
			}
		}


		/// <summary>
		/// These patches have to run manually or they break translations on certain screens
		/// </summary>
		[HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
		public static class OnASsetPrefabPatch
		{
			public static void Postfix()
			{
				SkipMinionScreen_StartProcess.AssetOnPrefabInitPostfix(Mod.harmonyInstance);
			}
		}

		//[HarmonyPatch(typeof(CharacterSelectionController), nameof(CharacterSelectionController.EnableProceedButton))]
		public static class SkipMinionScreen_StartProcess
		{

			public static void AssetOnPrefabInitPostfix(Harmony harmony)
			{
				var m_TargetMethod = AccessTools.Method("CharacterSelectionController, Assembly-CSharp:EnableProceedButton");
				var m_Postfix = AccessTools.Method(typeof(SkipMinionScreen_StartProcess), "Postfix");

				harmony.Patch(m_TargetMethod, null, new HarmonyMethod(m_Postfix));
			}

			public static void Postfix(CharacterSelectionController __instance)
			{
				if (__instance is MinionSelectScreen)
				{
					__instance.OnProceed();

				}
			}
		}
		[HarmonyPatch(typeof(GameFlowManager.StatesInstance), nameof(GameFlowManager.StatesInstance.CheckForGameOver))]
		public static class SkipGameOverCheck
		{
			public static bool Prefix()
			{
				if (autoLoadActive)
				{
					return false;
				}
				return true;
			}
		}
		[HarmonyPatch(typeof(GameFlowManager), nameof(GameFlowManager.IsGameOver))]
		public static class SkipGameOverCheck2
		{
			public static bool Prefix(ref bool __result)
			{
				if (autoLoadActive)
				{
					__result = false;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(WorldGen), nameof(WorldGen.ReportWorldGenError))]
		public static class RestartOnFailedSeed
		{
			public static void Postfix()
			{
				//potential TODO: send post request with "seed failed germination", then quit
				ModAssets.ClearAndRestart();
			}
		}
	}
}
