﻿using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using UtilLibs;

namespace OniRetroEdition
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            PUtil.InitLibrary(false);
            new POptions().RegisterOptions(this, typeof(Config));
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this);
            new PVersionCheck().Register(this, new SteamVersionChecker());
        }
    }
}