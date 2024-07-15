﻿using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace BathTub
{
    public class Mod : UserMod2
    {
        public static Harmony haromy;
        public override void OnLoad(Harmony harmony)
        {
            haromy = harmony;
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);
        }
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);
            ModAssets.RoomsExpandedActive = ModIntegration.Rooms_Expanded.InitializeIntegration();
        }
    }
    
}
