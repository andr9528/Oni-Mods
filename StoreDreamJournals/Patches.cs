﻿using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace StoreDreamJournals
{
    class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(DreamJournalConfig))]
        [HarmonyPatch(nameof(DreamJournalConfig.CreatePrefab))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Postfix(GameObject __result)
            {

                __result.AddOrGet<KBoxCollider2D>().size = (Vector2)new Vector2f(0.67f, 0.75f);

                KPrefabID kprefabId = __result.AddOrGet<KPrefabID>();                
                kprefabId.RemoveTag(GameTags.IgnoreMaterialCategory);
                kprefabId.AddTag(GameTags.IndustrialIngredient);
            }
        }
    }
}
