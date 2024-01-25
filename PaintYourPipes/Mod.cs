﻿using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace PaintYourPipes
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);
            ModAssets.HotKeys.Register();
        }
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);
            Patches.AddColorComponentToFinishedBuildings.ExecutePatch(harmony);
            CompatibilityPatches.Reverse_Bridges_Compatibility.ExecutePatch(harmony);
            CompatibilityPatches.MaterialColour_Compatibility.ExecutePatch(harmony);
        }
    }
}
