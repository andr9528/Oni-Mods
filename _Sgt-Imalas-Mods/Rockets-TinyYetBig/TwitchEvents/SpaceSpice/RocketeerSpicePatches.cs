﻿using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.TwitchEvents.SpaceSpice
{
	public class RocketeerSpicePatches
	{
		[HarmonyPatch(typeof(Edible), "StopConsuming")]
		public static class PatchDroppingOfTincans
		{
			public static void Prefix(Worker worker, Edible __instance)
			{
				var rocketSpice = __instance.spices.Find((s) => s.Id == "PILOTING_SPICE");
				if (!rocketSpice.Equals(default(SpiceInstance)))
				{
					if (worker.TryGetComponent<SpiceEyes>(out var eyes))
					{
						eyes.AddEyeDuration(rocketSpice.StatBonus.duration);
					}
				}
			}
		}

		[HarmonyPatch(typeof(AccessorySlots), MethodType.Constructor, typeof(ResourceSet))]
		public class AccessorySlots_TargetMethod_Patch
		{
			public static void Postfix(AccessorySlots __instance, ResourceSet parent)
			{
				GlowyEyes.Register(__instance, parent);
			}
		}
		public class GlowyEyes
		{
			public static bool RegistrationSuccessful = false;
			public static void Register(AccessorySlots instance, ResourceSet parent)
			{
				AddAccessories(Assets.GetAnim("eye_spice_effect_kanim"), instance.Eyes, parent);
			}

			public static void AddAccessories(KAnimFile file, AccessorySlot slot, ResourceSet parent)
			{
				if (file == null)
				{
					SgtLogger.error("eye_spice_effect was null!");
					return;
				}


				var build = file.GetData().build;
				var id = slot.Id.ToLower();

				for (var i = 0; i < build.symbols.Length; i++)
				{
					var symbolName = HashCache.Get().Get(build.symbols[i].hash);
					if (symbolName.StartsWith(id))
					{
						var accessory = new Accessory(symbolName, parent, slot, file.batchTag, build.symbols[i], animFile: file);
						slot.accessories.Add(accessory);
						HashCache.Get().Add(accessory.IdHash.HashValue, accessory.Id);
					}
				}
				RegistrationSuccessful = true;
			}
		}
		[HarmonyPatch(typeof(MinionConfig), "CreatePrefab")]
		public class MinionConfig_CreatePrefab_Patch
		{
			public static void Postfix(ref GameObject __result)
			{
				AllSpicyEyes.Add(__result.AddOrGet<SpiceEyes>());
			}
		}
		static List<SpiceEyes> AllSpicyEyes = new List<SpiceEyes>();


		//TODO: manual patch to avoid breaking translations
		[HarmonyPatch(typeof(SaveLoader), "Save", new Type[] { typeof(string), typeof(bool), typeof(bool) })]
		public class SaveLoader_Save_Patch
		{
			public static void Prefix()
			{
				foreach (SpiceEyes facePaint in AllSpicyEyes)
				{
					facePaint.OnSaveGame();
				}
			}

			public static void Postfix()
			{
				foreach (SpiceEyes facePaint in AllSpicyEyes)
				{
					facePaint.OnLoadGame();
				}
			}
		}
	}
}
