﻿
using BlueprintsV2.ModAPI;
using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System.Collections.Generic;
using UtilLibs;
using static BlueprintsV2.ModAssets;

namespace BlueprintsV2
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			PUtil.InitLibrary();
			new POptions().RegisterOptions(this, typeof(Config));
			base.OnLoad(harmony);

			ModAssets.RegisterActions();
			SgtLogger.l("Loading Mod Assets...");
			LoadAssets();

			SgtLogger.LogVersion(this, harmony);
			BlueprintFileHandling.AttachFileWatcher();

		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			API_Methods.RegisterExtraData();
		}
	}
}
