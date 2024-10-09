﻿using BlueprintsV2.Tools;
using HarmonyLib;

namespace BlueprintsV2.Patches
{
	internal class CleanupPatch
	{
		[HarmonyPatch(typeof(Game), "DestroyInstances")]
		public static class GameDestroyInstances
		{
			public static void Postfix()
			{
				CreateBlueprintTool.DestroyInstance();
				UseBlueprintTool.DestroyInstance();
				SnapshotTool.DestroyInstance();
				MultiToolParameterMenu.DestroyInstance();
				ModAssets.SelectedBlueprint = null;
				ModAssets.SelectedFolder = null;
				ModAssets.BLUEPRINTS_AUTOFILE_WATCHER.Dispose();
			}
		}
	}
}
