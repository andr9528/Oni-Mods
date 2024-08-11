﻿using ClusterTraitGenerationManager.ClusterData;
using Klei.CustomSettings;
using Newtonsoft.Json;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UtilLibs;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using static ClusterTraitGenerationManager.STRINGS;
using static CustomGameSettings;

namespace ClusterTraitGenerationManager
{
    public class SO_POI_DataEntry
    {
        public int x, y;
        public string itemId;
        public SO_POI_DataEntry(AxialI point, string _itemId)
        {
            x = point.R;
            y = point.Q;
            itemId = _itemId;
        }
        [JsonIgnore]
        public AxialI locationData => new AxialI(x, y);
    }

    internal class CustomClusterSettingsPreset
    {
        public string FileName;
        public string PresetDLCId = DlcManager.EXPANSION1_ID;
        public string ConfigName;
        public int Rings;
        public int DefaultRings;
        public SerializableStarmapItem StarterPlanet;
        public SerializableStarmapItem WarpPlanet;
        public Dictionary<string, SerializableStarmapItem> OuterPlanets;
        public Dictionary<string, SerializableStarmapItem> POIs;
        public List<SO_POI_DataEntry> SO_POI_Overrides;
        public Dictionary<int, List<string>> VanillaStarmapLocations;
        public Dictionary<string, string> StoryTraits;
        public List<string> BlacklistedTraits;

        void PopulatePresetData(CustomClusterData data)
        {
            PresetDLCId = data.DLC_Id;
            Rings = data.Rings;
            DefaultRings = data.defaultRings;
            StarterPlanet = SerializableStarmapItem.InitPlanet(data.StarterPlanet);
            WarpPlanet = SerializableStarmapItem.InitPlanet(data.WarpPlanet);
            OuterPlanets = new Dictionary<string, SerializableStarmapItem>();
            foreach (var planet in data.OuterPlanets)
            {
                OuterPlanets.Add(planet.Key, SerializableStarmapItem.InitPlanet(planet.Value));
            }

            POIs = new Dictionary<string, SerializableStarmapItem>();
            foreach (var poi in data.POIs)
            {
                POIs.Add(poi.Key, SerializableStarmapItem.InitPOI(poi.Value));
            }
            if (!DlcManager.IsExpansion1Active())
            {
                VanillaStarmapLocations = new Dictionary<int, List<string>>(data.VanillaStarmapItems);

            }
            else if (data.SO_Starmap != null && data.SO_Starmap.UsingCustomLayout)
            {
                SO_POI_Overrides = new List<SO_POI_DataEntry>();
                data.SO_Starmap.OverridePlacements.ToList().ForEach(entry => SO_POI_Overrides.Add(new SO_POI_DataEntry(entry.Key, entry.Value)));
            }
        }

        public string ImmuneSystem, CalorieBurn, Morale, Durability, MeteorShowers, Radiation, Stress, Seed, SandboxMode, StressBreaks, CarePackages, FastWorkersMode, SaveToCloud, Teleporters;
        private void LoadCurrentGameSettings()
        {
            BlacklistedTraits = CGSMClusterManager.BlacklistedTraits.ToList();
            var instance = CustomGameSettings.Instance;
            bool isNoSweat = instance.customGameMode == CustomGameMode.Nosweat;
            {
                ///ImmuneSystem
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.ImmuneSystem.id))
                {
                    ImmuneSystem = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ImmuneSystem).id;
                }
                else
                {
                    ImmuneSystem = isNoSweat ? CustomGameSettingConfigs.ImmuneSystem.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.ImmuneSystem.GetDefaultLevelId();
                }

                ///CalorieBurn
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.CalorieBurn.id))
                {
                    CalorieBurn = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CalorieBurn).id;
                }
                else
                {
                    CalorieBurn = isNoSweat ? CustomGameSettingConfigs.CalorieBurn.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.CalorieBurn.GetDefaultLevelId();
                }

                ///Morale
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Morale.id))
                {
                    Morale = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Morale).id;
                }
                else
                {
                    Morale = isNoSweat ? CustomGameSettingConfigs.Morale.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.Morale.GetDefaultLevelId();
                }

                ///Durability (suits)
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Durability.id))
                {
                    Durability = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Durability).id;
                }
                else
                {
                    Durability = isNoSweat ? CustomGameSettingConfigs.Durability.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.Durability.GetDefaultLevelId();
                }

                ///MeteorShowers
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.MeteorShowers.id))
                {
                    MeteorShowers = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.MeteorShowers).id;
                }
                else
                {
                    MeteorShowers = isNoSweat ? CustomGameSettingConfigs.MeteorShowers.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.MeteorShowers.GetDefaultLevelId();
                }

                ///Radiation
                if (DlcManager.IsExpansion1Active())
                {
                    if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Radiation.id))
                    {
                        Radiation = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Radiation).id;
                    }
                    else
                    {
                        Radiation = isNoSweat ? CustomGameSettingConfigs.Radiation.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.Radiation.GetDefaultLevelId();
                    }
                }

                ///Stress
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Stress.id))
                {
                    Stress = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Stress).id;
                }
                else
                {
                    Stress = isNoSweat ? CustomGameSettingConfigs.Stress.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.Stress.GetDefaultLevelId();
                }

                ///StressBreaks
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.StressBreaks.id))
                {
                    StressBreaks = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.StressBreaks).id;
                }
                else
                {
                    StressBreaks = isNoSweat ? CustomGameSettingConfigs.StressBreaks.GetNoSweatDefaultLevelId() : CustomGameSettingConfigs.StressBreaks.GetDefaultLevelId();
                }

                ///CarePackages
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.CarePackages.id))
                {
                    CarePackages = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.CarePackages).id;
                }
                else
                {
                    CarePackages = isNoSweat
                        ? CustomGameSettingConfigs.CarePackages.GetNoSweatDefaultLevelId()
                        : CustomGameSettingConfigs.CarePackages.GetDefaultLevelId();
                }

                ///Fast Workers
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.FastWorkersMode.id))
                {
                    FastWorkersMode = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.FastWorkersMode).id;
                }
                else
                {
                    FastWorkersMode = isNoSweat
                        ? CustomGameSettingConfigs.FastWorkersMode.GetNoSweatDefaultLevelId()
                        : CustomGameSettingConfigs.FastWorkersMode.GetDefaultLevelId();
                }

                ///Save to Cloud
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.SaveToCloud.id))
                {
                    SaveToCloud = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.SaveToCloud).id;
                }
                else
                {
                    SaveToCloud = isNoSweat
                        ? CustomGameSettingConfigs.SaveToCloud.GetNoSweatDefaultLevelId()
                        : CustomGameSettingConfigs.SaveToCloud.GetDefaultLevelId();
                }

                ///Teleporters
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.Teleporters.id))
                {
                    Teleporters = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Teleporters).id;
                }
                else
                {
                    Teleporters = isNoSweat
                        ? CustomGameSettingConfigs.Teleporters.GetNoSweatDefaultLevelId()
                        : CustomGameSettingConfigs.Teleporters.GetDefaultLevelId();
                }

                ///Sandbox
                if (instance.QualitySettings.ContainsKey(CustomGameSettingConfigs.SandboxMode.id))
                {
                    SandboxMode = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.SandboxMode).id;
                }
                else
                {
                    SandboxMode = isNoSweat
                        ? CustomGameSettingConfigs.SandboxMode.GetNoSweatDefaultLevelId()
                        : CustomGameSettingConfigs.SandboxMode.GetDefaultLevelId();
                }

            }

            Seed = instance.GetCurrentQualitySetting(CustomGameSettingConfigs.WorldgenSeed).id;

            StoryTraits = new Dictionary<string, string>();
            foreach (var story in instance.StorySettings)
            {
                string value = string.Empty;

                if (!instance.currentStoryLevelsBySetting.ContainsKey(story.Key))
                {
                    value = isNoSweat
                    ? story.Value.GetNoSweatDefaultLevelId()
                    : story.Value.GetDefaultLevelId();
                }
                else
                {
                    value = instance.currentStoryLevelsBySetting[story.Key];
                }
                StoryTraits.Add(story.Key, value);
            }
        }

        private void SetCustomGameSettings(SettingConfig ConfigToSet, object valueId, bool isStoryTrait = false)
        {
            string valueToSet = valueId.ToString();
            if (valueId is bool)
            {
                var toggle = ConfigToSet as ToggleSettingConfig;
                valueToSet = ((bool)valueId) ? toggle.on_level.id : toggle.off_level.id;
            }
            if (isStoryTrait)
            {
                CustomGameSettings.Instance.SetStorySetting(ConfigToSet, valueToSet);
            }
            else
            {
                CustomGameSettings.Instance.SetQualitySetting(ConfigToSet, valueToSet);
            }
        }

        private void ApplyGameSettings()
        {

            if (BlacklistedTraits == null)
                BlacklistedTraits = new List<string>();

            CGSMClusterManager.BlacklistedTraits = new(this.BlacklistedTraits);

            ///ImmuneSystem
            if (ImmuneSystem != null && ImmuneSystem.Length > 0)
                SetCustomGameSettings(CustomGameSettingConfigs.ImmuneSystem, ImmuneSystem);

            ///CalorieBurn
            if (CalorieBurn != null && CalorieBurn.Length > 0)
                SetCustomGameSettings(CustomGameSettingConfigs.CalorieBurn, CalorieBurn);

            ///Morale
            if (Morale != null && Morale.Length > 0)
                SetCustomGameSettings(CustomGameSettingConfigs.Morale, Morale);

            ///Durability (suits)
            if (Durability != null && Durability.Length > 0)
                SetCustomGameSettings(CustomGameSettingConfigs.Durability, Durability);

            ///MeteorShowers
            if (MeteorShowers != null && MeteorShowers.Length > 0)
                SetCustomGameSettings(CustomGameSettingConfigs.MeteorShowers, MeteorShowers);

            ///Radiation
            if (DlcManager.IsExpansion1Active())
            {
                if (Radiation != null && Radiation.Length > 0)
                    SetCustomGameSettings(CustomGameSettingConfigs.Radiation, Radiation);
            }

            ///Stress
            if (Stress != null && Stress.Length > 0)
                SetCustomGameSettings(CustomGameSettingConfigs.Stress, Stress);

            ///StressBreaks
            if (StressBreaks != null && StressBreaks.Length > 0)
                SetCustomGameSettings(CustomGameSettingConfigs.StressBreaks, StressBreaks);

            ///CarePackages
            if (CarePackages != null && CarePackages.Length > 0)
                SetCustomGameSettings(CustomGameSettingConfigs.CarePackages, CarePackages);

            ///Fast Workers
            if (FastWorkersMode != null && FastWorkersMode.Length > 0)
                SetCustomGameSettings(CustomGameSettingConfigs.FastWorkersMode, FastWorkersMode);

            ///Save to Cloud
            if (SaveToCloud != null && SaveToCloud.Length > 0)
                SetCustomGameSettings(CustomGameSettingConfigs.SaveToCloud, SaveToCloud);

            ///Teleporters
            if (DlcManager.IsExpansion1Active())
            {
                if (Teleporters != null && Teleporters.Length > 0)
                    SetCustomGameSettings(CustomGameSettingConfigs.Teleporters, Teleporters);
            }

            ///Seed
            if (Seed != null && Seed.Length > 0)
                SetCustomGameSettings(CustomGameSettingConfigs.WorldgenSeed, Seed);


            if (StoryTraits != null && StoryTraits.Count > 0)
            {
                foreach (var story in StoryTraits)
                {
                    if (CustomGameSettings.Instance.StorySettings.ContainsKey(story.Key))
                    {
                        SetCustomGameSettings(CustomGameSettings.Instance.StorySettings[story.Key], story.Value, true);
                    }
                }
            }
        }



        public class SerializableStarmapItem
        {
            public string itemID;
            public int _predefinedPlacementOrder = -1;
            public int minRing, maxRing, buffer;
            public float numberToSpawn;
            public StarmapItemCategory category;
            public WorldSizePresets sizePreset;
            public WorldRatioPresets ratioPreset;
            public int customX, customY;
            public List<string> meteorSeasons;
            public List<string> planetTraits;
            public List<string> pois;
            public bool allowDuplicates, avoidClumping;

            public SerializableStarmapItem(
                string itemID,
                int placementOrder,
                int minRing,
                int maxRing,
                int buffer,
                float numberToSpawn,
                StarmapItemCategory category,
                WorldSizePresets sizePreset,
                WorldRatioPresets ratioPreset,
                int customX, int customY,
                List<string> meteorSeasons,
                List<string> planetTraits)
            {
                this.itemID = itemID;
                this._predefinedPlacementOrder = placementOrder;
                this.minRing = minRing;
                this.maxRing = maxRing;
                this.buffer = buffer;
                this.numberToSpawn = numberToSpawn;
                this.category = category;
                this.sizePreset = sizePreset;
                this.ratioPreset = ratioPreset;
                this.customX = customX;
                this.customY = customY;
                this.meteorSeasons = meteorSeasons;
                this.planetTraits = planetTraits;
            }

            public static SerializableStarmapItem InitPOI(StarmapItem poiItem)
            {
                if (poiItem == null)
                    return null;

                SgtLogger.Assert("poi placement, " + poiItem.id, poiItem.placementPOI);
                SgtLogger.Assert("pois, " + poiItem.id, poiItem.placementPOI.pois);
                SgtLogger.Assert("duplicates, " + poiItem.id, poiItem.placementPOI.canSpawnDuplicates);
                SgtLogger.Assert("avoidClumping, " + poiItem.id, poiItem.placementPOI.avoidClumping);


                return new SerializableStarmapItem(
                    poiItem.id,
                    -1,
                    poiItem.minRing,
                    poiItem.maxRing,
                    poiItem.buffer,
                    poiItem.InstancesToSpawn,
                    poiItem.category,
           default,
           default,
            -1,
            -1,
            null,
            null
                    )
                {
                    avoidClumping = poiItem.placementPOI.avoidClumping,
                    allowDuplicates = poiItem.placementPOI.canSpawnDuplicates,
                    pois = poiItem.placementPOI.pois

                };
            }
            public static SerializableStarmapItem InitRandomPlanet(StarmapItem poiItem)
            {
                if (poiItem == null)
                    return null;

                return new SerializableStarmapItem(
                    poiItem.id,
                    poiItem.PredefinedPlacementOrder,
                    poiItem.minRing,
                    poiItem.maxRing,
                    poiItem.buffer,
                    poiItem.InstancesToSpawn,
                    poiItem.category,
                    default,
                    default,
                    -1,
                    -1,
                    null,
                    null
                    );
            }


            public static SerializableStarmapItem InitPlanet(StarmapItem poiItem)
            {
                if (poiItem == null)
                    return null;

                if (poiItem.id.Contains(CGSMClusterManager.RandomKey))
                    return InitRandomPlanet(poiItem);

                return new SerializableStarmapItem(
                    poiItem.id,
                    poiItem.PredefinedPlacementOrder,
                    poiItem.minRing,
                    poiItem.maxRing,
                    poiItem.buffer,
                    poiItem.InstancesToSpawn,
                    poiItem.category,
                    poiItem.CurrentSizePreset,
                    poiItem.CurrentRatioPreset,
                    poiItem.CustomX,
                    poiItem.CustomY,
                    poiItem.world.seasons,
                    poiItem.CurrentTraits
                    );
            }
        }


        public void OpenPopUpToChangeName(System.Action callBackAction = null, GameObject parentOverride = null)
        {
            if (parentOverride == null) parentOverride = FrontEndManager.Instance.gameObject;


            FileNameDialog fileNameDialog = Util.KInstantiateUI(ScreenPrefabs.Instance.FileNameDialog.gameObject, parentOverride, true).GetComponent<FileNameDialog>();
            fileNameDialog.SetTextAndSelect(ConfigName);
            fileNameDialog.onConfirm = (System.Action<string>)(newName =>
            {
                if (newName.EndsWith(".sav"))
                {
                    int place = newName.LastIndexOf(".sav");

                    if (place != -1)
                        newName = newName.Remove(place, 4);
                }
                this.ChangenName(newName);

                if (callBackAction != null)
                    callBackAction.Invoke();
            });
        }

        public void ChangenName(string newName)
        {
            DeleteFile();
            ConfigName = newName;
            FileName = FileNameWithHash(newName);
            WriteToFile();
        }

        static string FileNameWithHash(string filename)
        {
            return filename.Replace(" ", "_") + "_" + GenerateHash(System.DateTime.Now.ToString());
        }

        public void ApplyPreset()
        {

            if (CGSMClusterManager.CustomCluster == null)
                return;

            int missinCount = 0;

            SgtLogger.l("Applying Preset " + ConfigName);

            ApplyGameSettings();
            SgtLogger.l("Settings loaded");
            var dict = PlanetoidDict;

            var cluster = CGSMClusterManager.CustomCluster;
            cluster.defaultRings = DefaultRings;
            cluster.SetRings(this.Rings);
            cluster.DLC_Id = PresetDLCId;

            if (StarterPlanet != null)
            {
                string itemId = StarterPlanet.itemID;

                var StarterPlanetItem = dict.ContainsKey(itemId) ? dict[itemId] : null;
                if (StarterPlanetItem != null)
                {
                    ApplyDataToStarmapItem(StarterPlanet, StarterPlanetItem);
                    cluster.StarterPlanet = StarterPlanetItem;
                }
                else
                {
                    missinCount++;
                    cluster.StarterPlanet = null;
                }
            }
            else
            {
                cluster.StarterPlanet = null;
            }

            if (WarpPlanet != null)
            {
                string itemId = WarpPlanet.itemID;


                var WarpPlanetItem = dict.ContainsKey(itemId) ? dict[itemId] : null;
                if (WarpPlanetItem != null)
                {
                    ApplyDataToStarmapItem(WarpPlanet, WarpPlanetItem);
                    cluster.WarpPlanet = WarpPlanetItem;
                }
                else
                {
                    missinCount++;
                    cluster.WarpPlanet = null;
                }
            }
            else
            {
                cluster.WarpPlanet = null;
            }

            cluster.OuterPlanets.Clear();
            foreach (var outerplanet in this.OuterPlanets)
            {

                string itemId = outerplanet.Value.itemID;

                var outerItem = dict.ContainsKey(itemId) ? (dict[itemId]) : null;
                if (outerItem != null)
                {
                    ApplyDataToStarmapItem(outerplanet.Value, outerItem);
                    cluster.OuterPlanets[outerplanet.Key] = outerItem;
                }
                else
                {
                    SgtLogger.l(outerplanet.Key + " had no item");
                    missinCount++;
                }
            }

            cluster.POIs.Clear();
            foreach (var poi in this.POIs)
            {
                if (poi.Value == null)
                {
                    SgtLogger.l(poi.Key + " had no item");
                    continue;
                }
                if (poi.Value.pois == null || poi.Value.pois.Count == 0)
                {
                    SgtLogger.l("legacy poi " + poi.Key + " found");
                    cluster.AddLegacyPOIGroup(poi.Key, poi.Value.minRing, poi.Value.maxRing, poi.Value.numberToSpawn);
                }
                else
                {
                    cluster.AddPoiGroup(poi.Key, new ProcGen.SpaceMapPOIPlacement()
                    {
                        allowedRings = new ProcGen.MinMaxI(poi.Value.minRing, poi.Value.maxRing),
                        pois = poi.Value.pois,
                        canSpawnDuplicates = poi.Value.allowDuplicates,
                        avoidClumping = poi.Value.avoidClumping,
                        numToSpawn = (int)Mathf.FloorToInt(poi.Value.numberToSpawn)

                    }, poi.Value.numberToSpawn);
                }
            }
            if (!DlcManager.IsExpansion1Active())
            {
                if (VanillaStarmapLocations != null)
                {
                    cluster.VanillaStarmapItems.Clear();
                    cluster.VanillaStarmapItems = new Dictionary<int, List<string>>(this.VanillaStarmapLocations);
                }
            }
            else
            {
                if (SO_POI_Overrides != null)
                {
                    SgtLogger.l("applying custom spacemap");
                    cluster.SO_Starmap.OverridePlacements = new Dictionary<AxialI, string>();
                    this.SO_POI_Overrides.ForEach(entry => cluster.SO_Starmap.OverridePlacements[entry.locationData] = entry.itemId);
                    cluster.SO_Starmap.SetUsingCustomLayout();
                }
                else
                {
                    cluster.SO_Starmap = null;
                }
            }
            if (missinCount > 0)
            {
                DialogUtil.CreateConfirmDialogFrontend(ERRORMESSAGES.MISSINGWORLDS_TITLE, string.Format(ERRORMESSAGES.MISSINGWORLDS_TEXT, missinCount));
            }
        }
        void ApplyDataToStarmapItem(SerializableStarmapItem item, StarmapItem reciverToLookup)
        {
            item.minRing = Math.Max(0, item.minRing);
            item.maxRing = Math.Max(0, item.maxRing);
            if (item.category != StarmapItemCategory.POI)
                item.buffer = Math.Max(0, item.buffer);

            var reciever = GivePrefilledItem(reciverToLookup);

            SgtLogger.l("setting starmap item rings: min->" + item.minRing + ", max->" + item.maxRing + ", buffer: " + item.buffer, reciever.id);

            if (item._predefinedPlacementOrder != -1)
                reciever.PredefinedPlacementOrder = item._predefinedPlacementOrder;

            reciever.SetOuterRing(item.maxRing);
            reciever.SetInnerRing(item.minRing);
            reciever.SetBuffer(item.buffer);
            reciever.SetSpawnNumber(item.numberToSpawn);
            if (item.sizePreset != default)
            {
                reciever.SetPlanetSizeToPreset(item.sizePreset);
            }
            if (item.ratioPreset != default)
            {
                reciever.SetPlanetRatioToPreset(item.ratioPreset);
            }
            if (item.customX > 0)
                reciever.ApplyCustomDimension(item.customX, false);

            if (item.customY > 0)
                reciever.ApplyCustomDimension(item.customY, true);

            if (reciever.world != null)
            {
                reciever.world.seasons = item.meteorSeasons;
            }
            if (!reciever.IsPOI && !reciever.IsRandom)
            {
                reciever.SetWorldTraits(item.planetTraits);
            }
            else
            {
                //reciever.MaxNumberOfInstances = item.maxNumberToSpawn;
                reciever.SetSpawnNumber(item.numberToSpawn);
            }
        }



        public CustomClusterSettingsPreset(string fileName, string configName, CustomClusterData data)
        {
            FileName = fileName;
            ConfigName = configName;
            PopulatePresetData(data);
            LoadCurrentGameSettings();
        }
        public CustomClusterSettingsPreset() { }

        public static string GenerateHash(string str)
        {
            using (var md5Hasher = MD5.Create())
            {
                var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
                return BitConverter.ToString(data).Replace("-", "").Substring(0, 6);
            }
        }

        public static CustomClusterSettingsPreset CreateFromCluster(CustomClusterData data, string nameOverride = "")
        {

            string scheduleName = nameOverride.Length > 0 ? nameOverride : "UNNAMED CLUSTER";

            var config = new CustomClusterSettingsPreset(
                FileNameWithHash(scheduleName),
                scheduleName,
                data);
            return config;
        }
        public static CustomClusterSettingsPreset ReadFromFile(FileInfo filePath)
        {
            if (!filePath.Exists || filePath.Extension != ".json")
            {
                SgtLogger.logwarning("Not a valid custom cluster preset.");
                return null;
            }
            else
            {
                FileStream filestream = filePath.OpenRead();
                using (var sr = new StreamReader(filestream))
                {
                    string jsonString = sr.ReadToEnd();
                    CustomClusterSettingsPreset preset = JsonConvert.DeserializeObject<CustomClusterSettingsPreset>(jsonString);
                    preset.FixAsteroidIDs();
                    return preset;
                }
            }
        }

        private void FixAsteroidIDs()
        {
            if (StarterPlanet != null)
            {
                if (ModAssets.FindSwapAsteroid(StarterPlanet.itemID, out var newId))
                    StarterPlanet.itemID = newId;
            }
            if (WarpPlanet != null)
            {
                if (ModAssets.FindSwapAsteroid(WarpPlanet.itemID, out var newId))
                    WarpPlanet.itemID = newId;
            }

            if (OuterPlanets != null)
            {
                List<string> Keys = OuterPlanets.Keys.ToList();
                foreach (var asteroid in Keys)
                {
                    if (ModAssets.FindSwapAsteroid(asteroid, out var newId))
                    {
                        var ast = OuterPlanets[asteroid];
                        ast.itemID = newId;

                        OuterPlanets.Remove(asteroid);
                        OuterPlanets.Add(newId, ast);
                    }
                }

            }
            if (SO_POI_Overrides != null)
            {
                foreach (var poiPos in SO_POI_Overrides)
                {
                    if (ModAssets.FindSwapAsteroid(poiPos.itemId, out var newId))
                    {
                        poiPos.itemId = newId;
                    }
                }
            }
        }

        public void WriteToFile()
        {
            try
            {
                var path = Path.Combine(ModAssets.CustomClusterTemplatesPath, FileName + ".json");

                var fileInfo = new FileInfo(path);
                FileStream fcreate = fileInfo.Open(FileMode.Create);

                var JsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
                using (var streamWriter = new StreamWriter(fcreate))
                {
                    streamWriter.Write(JsonString);
                }
            }
            catch (Exception e)
            {
                SgtLogger.logError("Could not write file, Exception: " + e);
            }
        }
        public void DeleteFile()
        {
            try
            {
                var path = Path.Combine(ModAssets.CustomClusterTemplatesPath, FileName + ".json");

                var fileInfo = new FileInfo(path);
                fileInfo.Delete();
            }
            catch (Exception e)
            {
                SgtLogger.logError("Could not delete file, Exception: " + e);
            }
        }
    }
}
