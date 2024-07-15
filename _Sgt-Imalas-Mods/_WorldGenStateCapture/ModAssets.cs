﻿using _WorldGenStateCapture.WorldStateData;
using _WorldGenStateCapture.WorldStateData.Starmap.SpacemapItems;
using _WorldGenStateCapture.WorldStateData.WorldPOIs;
using Klei.AI;
using Klei.CustomSettings;
using ProcGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Satsuma.CompleteBipartiteGraph;
using static STRINGS.UI.CLUSTERMAP;

namespace _WorldGenStateCapture
{
    internal class ModAssets
    {
        public static Dictionary<WorldContainer, List<MapGeyser>> currentGeysers = new();
        public static Dictionary<WorldContainer, List<MapPOI>> currentPOIs = new();
        public static List<HexMap_Entry> dlcStarmapItems = new List<HexMap_Entry>();
        public static List<VanillaMap_Entry> baseStarmapItems = new List<VanillaMap_Entry>();

        static string BaseGameFolder = "BasegameSeeds";
        static string DlcClassicFolder = "DlcClassicSeeds";
        static string DlcSpacedOutFolder = "DlcSOSeeds";

        internal static void AccumulateSeedData()
        {

            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(IO_Utils.ModPath, BaseGameFolder));
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(IO_Utils.ModPath, DlcClassicFolder));
            System.IO.Directory.CreateDirectory(System.IO.Path.Combine(IO_Utils.ModPath, DlcSpacedOutFolder));


            bool dlcActive = DlcManager.IsExpansion1Active();

            WorldDataInstance DataItem = new WorldDataInstance();


            SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout);
            if (currentQualitySetting == null)
            {
                SgtLogger.error("Clusterlayout was null");
                return;
            }
            //if(Global.Instance.modManager.mods.Count > 1)
            //{
            //    SgtLogger.error("other mods installed");
            //}



            var GridMap = new Bitmap(Grid.WidthInCells, Grid.HeightInCells);
            
            for (int i = 0; i < Grid.CellCount; i++)
            {
                SubWorld.ZoneType data = World.Instance.zoneRenderData.worldZoneTypes[i];
                Grid.CellToXY(i,out var x,out var y);
                Color32 color = World.Instance.zoneRenderData.zoneColours[(int)data];
                
                GridMap.SetPixel(x, (Grid.HeightInCells-1)  - y, System.Drawing.Color.FromArgb(color.r,color.g,color.b));
            }


            ClusterLayout clusterData = SettingsCache.clusterLayouts.GetClusterData(currentQualitySetting.id);
            SettingLevel currentQualitySetting2 = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed);
            //string otherSettingsCode = CustomGameSettings.Instance.GetOtherSettingsCode();
            string storyTraitSettingsCode = CustomGameSettings.Instance.GetStoryTraitSettingsCode();


            int.TryParse(currentQualitySetting2.id, out int seed);
            DataItem.Seed = seed;
            DataItem.Coordinate = clusterData.GetCoordinatePrefix();
            DataItem.FullCoordinate = CustomGameSettings.Instance.GetSettingsCoordinate();
            DataItem.StoryTraits = new(CustomGameSettings.Instance.GetCurrentStories());

            SgtLogger.l("accumulating pois...");
            foreach (var asteroid in ClusterManager.Instance.WorldContainers)
            {
                SgtLogger.l("collecting "+asteroid.GetProperName());
                var asteroidData = new AsteroidData()
                {
                    Id = System.IO.Path.GetFileName(asteroid.worldName),
                    OffsetX = asteroid.WorldOffset.X,
                    OffsetY = asteroid.WorldOffset.Y,
                    SizeX = asteroid.WorldSize.X,
                    SizeY = asteroid.WorldSize.Y,
                    WorldTraits = asteroid.WorldTraitIds,
                    StoryTraits = asteroid.StoryTraitIds
                };

                if (currentPOIs.ContainsKey(asteroid))
                    asteroidData.POIs = new(currentPOIs[asteroid]);

                if (currentGeysers.ContainsKey(asteroid))
                    asteroidData.Geysers = new(currentGeysers[asteroid]);

                DataItem.Asteroids.Add(asteroidData);
            }

            if (dlcActive)
            {
                DataItem.StarmapEntries_SpacedOut = new(dlcStarmapItems);
            }
            else
            {
                DataItem.StarmapEntries_Vanilla = new(baseStarmapItems);
            }

            string parentPath = string.Empty;
            switch (clusterData.clusterCategory)
            {
                case 0:
                    parentPath = System.IO.Path.Combine(IO_Utils.ModPath, BaseGameFolder);
                    break;
                case 1:
                    parentPath = System.IO.Path.Combine(IO_Utils.ModPath, DlcClassicFolder);
                    break;
                case 2:
                    parentPath = System.IO.Path.Combine(IO_Utils.ModPath, DlcSpacedOutFolder);
                    break;

            }
            if (parentPath != string.Empty)
            {
                string fileName = DataItem.FullCoordinate + ".json";
                IO_Utils.WriteToFile(DataItem, System.IO.Path.Combine(parentPath, fileName));
                GridMap.Save(System.IO.Path.Combine(parentPath, DataItem.FullCoordinate + ".png"));
            }


            currentGeysers.Clear();
            currentPOIs.Clear();
            dlcStarmapItems.Clear();
            baseStarmapItems.Clear();
        }
    }
}
