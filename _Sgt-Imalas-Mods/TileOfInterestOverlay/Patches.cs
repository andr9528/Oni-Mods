﻿using Database;
using HarmonyLib;
using Klei.AI;
using PeterHan.PLib.Actions;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Database;
using PeterHan.PLib.Detours;
using PeterHan.PLib.PatchManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static StatusItem;
using static TileOfInterestOverlay.ModAssets;

namespace TileOfInterestOverlay
{
    internal class Patches : KMod.UserMod2
    {
        /// <summary>
        /// Public and non-public instance methods/constructors/types/fields.
        /// </summary>
        private const BindingFlags INSTANCE_ALL = PPatchTools.BASE_FLAGS | BindingFlags.
            Instance;

        private delegate void RegisterMode(OverlayScreen screen, OverlayModes.Mode mode);

        /// <summary>
        /// The key binding to open pip planting.
        /// </summary>
        private static PAction OpenOverlay;

        /// <summary>
        /// The private type to be used when making overlay buttons.
        /// </summary>
        private static readonly Type OVERLAY_TYPE = typeof(OverlayMenu).GetNestedType(
            "OverlayToggleInfo", INSTANCE_ALL);

        /// <summary>
        /// Registers a new overlay mode.
        /// </summary>
        private static readonly RegisterMode REGISTER_MODE = typeof(OverlayScreen).
            Detour<RegisterMode>();

        [PLibMethod(RunAt.AfterDbInit)]
        internal static void AfterDbInit()
        {
            // Assets are now loaded, so create pip icon
            var pip = Assets.GetAnim("squirrel_kanim");
            Sprite sprite = null;
            if (pip != null)
                sprite = Def.GetUISpriteFromMultiObjectAnim(pip);
            if (sprite == null)
                // Pip anim is somehow missing?
                sprite = Assets.GetSprite("overlay_farming");
            //Assets.Sprites.Add(PipPlantOverlayStrings.OVERLAY_ICON, sprite);
        }

        /// <summary>
        /// Gets an instance of the private overlay toggle info class used for creating new
        /// overlay buttons.
        /// </summary>
        /// <param name="text">The button text to be shown on mouseover.</param>
        /// <param name="icon_name">The icon to show in the overlay list.</param>
        /// <param name="sim_view">The overlay mode to enter when selected.</param>
        /// <returns>The button to be added.</returns>
        private static KIconToggleMenu.ToggleInfo CreateOverlayInfo(string text,
                string icon_name, HashedString sim_view, Action openKey,
                string tooltip)
        {
            const int KNOWN_PARAMS = 7;
            KIconToggleMenu.ToggleInfo info = null;
            ConstructorInfo[] cs;
            if (OVERLAY_TYPE == null || (cs = OVERLAY_TYPE.GetConstructors(INSTANCE_ALL)).
                    Length != 1)
                PUtil.LogWarning("Unable to add TileOfInterest - missing constructor");
            else
            {
                var cons = cs[0];
                var toggleParams = cons.GetParameters();
                int paramCount = toggleParams.Length;
                // Manually plug in the knowns
                if (paramCount < KNOWN_PARAMS)
                    PUtil.LogWarning("Unable to add TileOfInterest - parameters missing");
                else
                {
                    object[] args = new object[paramCount];
                    args[0] = text;
                    args[1] = icon_name;
                    args[2] = sim_view;
                    args[3] = "";
                    args[4] = openKey;
                    args[5] = tooltip;
                    args[6] = text;
                    // 3 and further (if existing) get new optional values
                    for (int i = KNOWN_PARAMS; i < paramCount; i++)
                    {
                        var op = toggleParams[i];
                        if (op.IsOptional)
                            args[i] = op.DefaultValue;
                        else
                        {
                            PUtil.LogWarning("Unable to add TileOfInterest - new parameters");
                            args[i] = null;
                        }
                    }
                    info = cons.Invoke(args) as KIconToggleMenu.ToggleInfo;
                }
            }
            return info;
        }

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            PUtil.InitLibrary();
            new PPatchManager(harmony).RegisterPatchClass(typeof(Patches));
            OpenOverlay = new PActionManager().CreateAction("OVERLAY_ACTIONTOGGLE_TILEOFINTEREST", "Tile of interest overlay");
                       
        }

        /// <summary>
        /// Applied to OverlayLegend to add an entry for the Pip Planting overlay.
        /// </summary>
        [HarmonyPatch(typeof(OverlayLegend), "OnSpawn")]
        public static class OverlayLegend_OnSpawn_Patch
        {
            /// <summary>
            /// Applied before OnSpawn runs.
            /// </summary>
            internal static void Prefix(ICollection<OverlayLegend.OverlayInfo> ___overlayInfoList)
            {
                ___overlayInfoList.Add(new OverlayLegend.OverlayInfo
                {
                    infoUnits = new List<OverlayLegend.OverlayInfoUnit>(1) {
                        new OverlayLegend.OverlayInfoUnit(
                            Assets.GetSprite("unknown"),
                            "STRINGS.UI.OVERLAYS.PIPPLANTING.DESCRIPTION",
                            Color.white, Color.white)
                    },
                    isProgrammaticallyPopulated = true,
                    mode = TileOfInterestOverlay.ID,
                    name = "STRINGS.UI.OVERLAYS.PIPPLANTING.NAME",
                });
            }
        }

        /// <summary>
        /// Applied to OverlayMenu to add a button for our overlay.
        /// </summary>
        [HarmonyPatch(typeof(OverlayMenu), "InitializeToggles")]
        public static class OverlayMenu_InitializeToggles_Patch
        {
            /// <summary>
            /// Applied after InitializeToggles runs.
            /// </summary>
            internal static void Postfix(ICollection<KIconToggleMenu.ToggleInfo> ___overlayToggleInfos)
            {
                var action = (OpenOverlay == null) ? PAction.MaxAction : OpenOverlay.
                    GetKAction();
                var info = CreateOverlayInfo("text", "unknown", TileOfInterestOverlay.ID, action,
                    "tooltip");
                if (info != null)
                    ___overlayToggleInfos?.Add(info);
            }
        }

        //[HarmonyPatch(typeof(UnitConfigurationScreen), "Init")]
        //public static class GameOptionsScreen_Init_Patch
        //{
        //    /// <summary>
        //    /// Test
        //    /// </summary>
        //    internal static void Postfix(UnitConfigurationScreen __instance)
        //    {
        //        UIUtils.AddSimpleTooltipToObject(__instance.celsiusToggle.gameObject, "Test tooltip");
        //    }
        //}




        /// <summary>
        /// Applied to OverlayScreen to add our overlay.
        /// </summary>
        [HarmonyPatch(typeof(OverlayScreen), "RegisterModes")]
        public static class OverlayScreen_RegisterModes_Patch
        {
            /// <summary>
            /// Applied after RegisterModes runs.
            /// </summary>
            internal static void Postfix(OverlayScreen __instance)
            {
                PUtil.LogDebug("Creating tileofinterestoverlay");
                REGISTER_MODE.Invoke(__instance, new TileOfInterestOverlay());
            }
        }

        /// <summary>
        /// Applied to SimDebugView to add a color handler for pip plant overlays.
        /// </summary>
        [HarmonyPatch(typeof(SimDebugView), "OnPrefabInit")]
        public static class SimDebugView_OnPrefabInit_Patch
        {
            /// <summary>
            /// Applied after OnPrefabInit runs.
            /// </summary>
            internal static void Postfix(IDictionary<HashedString, Func<SimDebugView, int, Color>> ___getColourFuncs)
            {
                ___getColourFuncs[TileOfInterestOverlay.ID] = TileOfInterestOverlay.GetColor;
            }
        }
    }
}

