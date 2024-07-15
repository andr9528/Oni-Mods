﻿using Database;
using Klei.CustomSettings;
using Newtonsoft.Json;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using static ClusterTraitGenerationManager.ClusterData.CGSMClusterManager;
using UnityEngine;
using UtilLibs;
using ClusterTraitGenerationManager.UI.SO_StarmapEditor;

namespace ClusterTraitGenerationManager.ClusterData
{
    public class CustomClusterData
    {
        int GetAdjustedOuterExpansion()
        {
            int outerPlanetcount = RandomOuterPlanetsStarmapItem != null ? (int)RandomOuterPlanetsStarmapItem.InstancesToSpawn : 0;
            bool outerplanetSelected = RandomOuterPlanetsStarmapItem != null ? CustomCluster.HasStarmapItem(RandomOuterPlanetsStarmapItem.id, out _) : false;

            int planetDiff = (CustomCluster.OuterPlanets.Count + (outerplanetSelected ? -1 : 0) + outerPlanetcount - CustomCluster.defaultOuterPlanets);


            return planetDiff;
        }

        [JsonIgnore] public bool HasTear => POIs != null && POIs.Any(item => item.Value.placementPOI != null && item.Value.placementPOI.pois != null && item.Value.placementPOI.pois.Contains("TemporalTear"));
        [JsonIgnore] public bool HasTeapot => POIs != null && POIs.Any(item => item.Value.placementPOI != null && item.Value.placementPOI.pois != null && item.Value.placementPOI.pois.Contains("ArtifactSpacePOI_RussellsTeapot"));
        [JsonIgnore] public int AdjustedOuterExpansion => GetAdjustedOuterExpansion();

        public int defaultRings = 12;
        public int defaultOuterPlanets = 6;
        public int Rings { get; private set; }

        [JsonIgnore] private SO_StarmapLayout _so_Starmap;
        [JsonIgnore]
        public SO_StarmapLayout SO_Starmap
        {
            get
            {
                if (_so_Starmap == null)
                    _so_Starmap = new SO_StarmapLayout(CurrentSeed);
                return _so_Starmap;
            }
            set
            {
                _so_Starmap = value;
            }
        }

        public StarmapItem StarterPlanet { get; set; }
        public StarmapItem WarpPlanet { get; set; }
        public Dictionary<string, StarmapItem> OuterPlanets = new Dictionary<string, StarmapItem>();
        public Dictionary<string, StarmapItem> POIs = new Dictionary<string, StarmapItem>();
        public string DLC_Id = DlcManager.GetHighestActiveDlcId();

        public Dictionary<int, List<string>> VanillaStarmapItems = new Dictionary<int, List<string>>();
        public int MaxStarmapDistance;

        public bool HasStarmapItem(string id, out StarmapItem item1)
        {
            if (id == null || id.Length == 0)
            {
                item1 = null;
                return false;
            }

            if (StarterPlanet != null && StarterPlanet.id == id)
            {
                item1 = StarterPlanet;
                return true;
            }
            else if (WarpPlanet != null && WarpPlanet.id == id)
            {
                item1 = WarpPlanet;
                return true;
            }
            else if (OuterPlanets.ContainsKey(id))
            {
                item1 = OuterPlanets[id];
                return true;
            }
            else if (POIs.ContainsKey(id))
            {
                item1 = POIs[id];
                return true;
            }
            if (PlanetoidDict.TryGetValue(id, out item1))
            {
                return false;
            }
            return false;
        }

        public List<StarmapItem> GetAllPlanets()
        {
            var list = new List<StarmapItem>();
            if (StarterPlanet != null)
                list.Add(StarterPlanet);
            if (WarpPlanet != null) list.Add(WarpPlanet);
            list.AddRange(OuterPlanets.Values);
            return list;
        }

        public List<string> GiveWorldTraitsForWorldGen(ProcGen.World world, int seed)
        {

            List<string> list = new List<string>();

            if (HasStarmapItem(world.filePath, out var starmapItem))
            {
                list = starmapItem.GetWorldTraits();
            }
            else ///randomly selected planet
            {
                int random = new System.Random(seed).Next(101);
                int count = 0;
                if (random >= 85)
                    count = 2;
                else if (random >= 40)
                    count = 1;
                List<string> randomSelectedTraits = new List<string>();

                if (world.disableWorldTraits || world.worldTraitRules == null)
                {
                    SgtLogger.l("worldtraits disabled, not rolling any random traits", Strings.Get(world.name));
                    return randomSelectedTraits;
                }

                return AddRandomTraitsForWorld(randomSelectedTraits, world, count, seed);
            }

            if (list.Any(id => id == ModAssets.CustomTraitID))
            {
                int random = new System.Random(seed).Next(101);
                int count = 1;
                if (random >= 85)
                    count = 3;
                else if (random >= 50)
                    count = 2;

                List<string> randomSelectedTraits = new List<string>();


                if (world.disableWorldTraits || world.worldTraitRules == null)
                    return randomSelectedTraits;

                return AddRandomTraitsForWorld(randomSelectedTraits, world, count, seed);
            }


            return list;
        }
        public static List<string> AddRandomTraitsForWorld(List<string> existing, ProcGen.World world, int count, int seed)
        {
            SgtLogger.l($"rolling {count} random traits", Strings.Get(world.name));
            for (int i = 0; i < count; ++i)
            {
                var possibleTraits = StarmapItem.AllowedWorldTraitsFor(existing, world)
                    .Where(item =>
                    item.filePath != ModAssets.CustomTraitID
                    //&& !item.filePath.ToUpperInvariant().Contains("SPACEHOLE")
                    && !RandomTraitInBlacklist(item.filePath));
                if (possibleTraits.Count() == 0)
                    break;
                else
                {
                    possibleTraits = possibleTraits.Shuffle(new System.Random(seed));
                    string randTrait = possibleTraits.First().filePath == ModAssets.CustomTraitID ? possibleTraits.Last().filePath : possibleTraits.First().filePath;

                    if (randTrait != ModAssets.CustomTraitID)
                    {
                        existing.Add(randTrait);
                        SgtLogger.l(seed + " rolled " + randTrait, Strings.Get(world.name));
                    }
                    seed += 1;
                }
            }
            return existing;
        }

        public void SetRings(int rings, bool defaultRing = false)
        {
            rings = Math.Min(rings, ringMax);
            rings = Math.Max(rings, ringMin);

            Rings = rings;
            if (defaultRing)
                defaultRings = rings;

            if (StarterPlanet != null && StarterPlanet.placement != null)
            {
                if (StarterPlanet.placement.allowedRings.max >= rings)
                {
                    StarterPlanet.SetOuterRing(rings);
                }
                if (StarterPlanet.placement.allowedRings.min >= rings)
                {
                    StarterPlanet.SetInnerRing(rings);
                }
            }
            if (WarpPlanet != null && WarpPlanet.placement != null)
            {
                if (WarpPlanet.placement.allowedRings.max >= rings)
                {
                    WarpPlanet.SetOuterRing(rings);
                }
                if (WarpPlanet.placement.allowedRings.min >= rings)
                {
                    WarpPlanet.SetInnerRing(rings);
                }
            }

            foreach (var planet in OuterPlanets.Values)
            {
                if (planet.placement != null)
                {
                    if (planet.placement.allowedRings.max >= rings)
                    {
                        planet.SetOuterRing(rings);
                    }
                    if (planet.placement.allowedRings.min >= rings)
                    {
                        planet.SetInnerRing(rings);
                    }
                }
            }
            foreach (var planet in POIs.Values)
            {
                if (planet.placementPOI != null)
                {
                    if (planet.placementPOI.allowedRings.max >= rings)
                    {
                        planet.SetOuterRing(rings);
                    }
                    if (planet.placementPOI.allowedRings.min >= rings)
                    {
                        planet.SetInnerRing(rings);
                    }
                }
            }
            //if (RandomPOIStarmapItem != null)
            MaxPOICount = Math.Max(16, Mathf.RoundToInt((7.385f * ((float)rings)) - 56.615f));
        }

        public bool SomeStarmapitemsMissing(out List<string> missings)
        {
            missings = Db.Get().SpaceDestinationTypes.resources.Select(entry => entry.Id).ToList();

            foreach (var poiList in VanillaStarmapItems.Values)
            {
                missings.RemoveAll(entry => poiList.Contains(entry));

            }
            return missings.Count > 0;
        }

        public int AddVanillaStarmapDistance()
        {
            VanillaStarmapItems[MaxStarmapDistance].RemoveAll(item => item == "Wormhole");

            VanillaStarmapItems[++MaxStarmapDistance] = new List<string>() { "Wormhole" };
            return MaxStarmapDistance;
        }
        public void RemoveFurthestVanillaStarmapDistance()
        {
            VanillaStarmapItems.Remove(MaxStarmapDistance);
            VanillaStarmapItems[--MaxStarmapDistance].Add("Wormhole");
        }
        public void RemoveVanillaPoi(Tuple<string, int> item) => RemoveVanillaPoi(item.first, item.second);
        public void RemoveVanillaPoi(string id, int range)
        {
            SgtLogger.l("removing " + id + " from " + range);
            VanillaStarmapItems[range].Remove(id);
        }
        public void AddVanillaPoi(string id, int range)
        {
            SgtLogger.l("adding " + id + " to " + range);
            VanillaStarmapItems[range].Add(id);
        }

        public void ResetVanillaStarmap()
        {
            if (DlcManager.IsExpansion1Active())
                return;
            SgtLogger.l("Resetting vanilla starmap");
            VanillaStarmapItems.Clear();
            GenerateVanillaStarmapDestinations();
        }

        void PopulateVanillaStarmapLocations()
        {
            SpaceDestinationTypes destinationTypes = Db.Get().SpaceDestinationTypes;
            _vanillaSpawns = new List<List<string>>()
                {
                    new List<string>(),
                    new List<string>() { destinationTypes.OilyAsteroid.Id },
                    new List<string>() { destinationTypes.Satellite.Id },
                    new List<string>()
                    {
                        destinationTypes.Satellite.Id,
                        destinationTypes.RockyAsteroid.Id,
                        destinationTypes.CarbonaceousAsteroid.Id,
                        destinationTypes.ForestPlanet.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.MetallicAsteroid.Id,
                        destinationTypes.RockyAsteroid.Id,
                        destinationTypes.CarbonaceousAsteroid.Id,
                        destinationTypes.SaltDwarf.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.MetallicAsteroid.Id,
                        destinationTypes.RockyAsteroid.Id,
                        destinationTypes.CarbonaceousAsteroid.Id,
                        destinationTypes.IcyDwarf.Id,
                        destinationTypes.OrganicDwarf.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.IcyDwarf.Id,
                        destinationTypes.OrganicDwarf.Id,
                        destinationTypes.DustyMoon.Id,
                        destinationTypes.ChlorinePlanet.Id,
                        destinationTypes.RedDwarf.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.DustyMoon.Id,
                        destinationTypes.TerraPlanet.Id,
                        destinationTypes.VolcanoPlanet.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.TerraPlanet.Id,
                        destinationTypes.GasGiant.Id,
                        destinationTypes.IceGiant.Id,
                        destinationTypes.RustPlanet.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.GasGiant.Id,
                        destinationTypes.IceGiant.Id,
                        destinationTypes.HydrogenGiant.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.RustPlanet.Id,
                        destinationTypes.VolcanoPlanet.Id,
                        destinationTypes.RockyAsteroid.Id,
                        destinationTypes.TerraPlanet.Id,
                        destinationTypes.MetallicAsteroid.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.ShinyPlanet.Id,
                        destinationTypes.MetallicAsteroid.Id,
                        destinationTypes.RockyAsteroid.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.GoldAsteroid.Id,
                        destinationTypes.OrganicDwarf.Id,
                        destinationTypes.ForestPlanet.Id,
                        destinationTypes.ChlorinePlanet.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.IcyDwarf.Id,
                        destinationTypes.MetallicAsteroid.Id,
                        destinationTypes.DustyMoon.Id,
                        destinationTypes.VolcanoPlanet.Id,
                        destinationTypes.IceGiant.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.ShinyPlanet.Id,
                        destinationTypes.RedDwarf.Id,
                        destinationTypes.RockyAsteroid.Id,
                        destinationTypes.GasGiant.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.HydrogenGiant.Id,
                        destinationTypes.ForestPlanet.Id,
                        destinationTypes.OilyAsteroid.Id
                    },
                    new List<string>()
                    {
                        destinationTypes.GoldAsteroid.Id,
                        destinationTypes.SaltDwarf.Id,
                        destinationTypes.TerraPlanet.Id,
                        destinationTypes.VolcanoPlanet.Id
                    }
                };

            _possibleVanillaStarmapLocations = new Dictionary<string, List<int>>();

            for (int distance = 0; distance < _vanillaSpawns.Count; distance++)
            {
                foreach (var planet in _vanillaSpawns[distance])
                {
                    if (!_possibleVanillaStarmapLocations.ContainsKey(planet))
                        _possibleVanillaStarmapLocations.Add(planet, new List<int>());
                    _possibleVanillaStarmapLocations[planet].Add(distance);
                }
            }
        }


        [JsonIgnore]
        List<List<string>> _vanillaSpawns = null;

        [JsonIgnore]
        List<List<string>> VanillaSpawns
        {
            get
            {
                if (_vanillaSpawns == null)
                    PopulateVanillaStarmapLocations();
                return _vanillaSpawns;
            }
        }
        [JsonIgnore]
        Dictionary<string, List<int>> PossibleVanillaStarmapLocations
        {
            get
            {
                if (_possibleVanillaStarmapLocations == null)
                    PopulateVanillaStarmapLocations();

                return _possibleVanillaStarmapLocations;
            }
        }
        [JsonIgnore]
        Dictionary<string, List<int>> _possibleVanillaStarmapLocations = null;

        public void AddMissingStarmapItems()
        {
            if (SomeStarmapitemsMissing(out var missingIds))
            {
                string setting = selectScreen.newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed);
                int seed = int.Parse(setting);
                SgtLogger.l(setting, "seed");
                var random = new System.Random(seed);
                foreach (string planetId in missingIds)
                {
                    List<int> possibleLocations = PossibleVanillaStarmapLocations.ContainsKey(planetId)
                        ? PossibleVanillaStarmapLocations[planetId]
                        : MaxStarmapDistance > 5
                            ? Enumerable.Range(4, MaxStarmapDistance - 5).ToList()
                            : new List<int>() { 0 };

                    possibleLocations = possibleLocations.Shuffle(random).ToList();
                    int distance = possibleLocations.First();
                    SgtLogger.l(planetId + ": " + distance, "adding missing," + PossibleVanillaStarmapLocations.ContainsKey(planetId));
                    AddVanillaPoi(planetId, distance);
                }
            }
        }
        ///<summary>
        /// copied from SpaceCraftManager.GenerateFixedDestinations and SpaceCraftManager.GenerateRandomDestinations.
        /// required since those methods require the savegame seed and arent returning anything
        /// </summary>
        void GenerateVanillaStarmapDestinations()
        {

            string setting = selectScreen.newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed);
            int seed = int.Parse(setting);
            SpaceDestinationTypes destinationTypes = Db.Get().SpaceDestinationTypes;

            List<Tuple<string, int>> destinationsWithDistance = new List<Tuple<string, int>>();
            ///Fixed Items:
            destinationsWithDistance.Add(new(destinationTypes.CarbonaceousAsteroid.Id, 0));
            destinationsWithDistance.Add(new(destinationTypes.CarbonaceousAsteroid.Id, 0));
            destinationsWithDistance.Add(new(destinationTypes.MetallicAsteroid.Id, 1));
            destinationsWithDistance.Add(new(destinationTypes.RockyAsteroid.Id, 2));
            destinationsWithDistance.Add(new(destinationTypes.IcyDwarf.Id, 3));
            destinationsWithDistance.Add(new(destinationTypes.OrganicDwarf.Id, 4));

            destinationsWithDistance.Add(new(destinationTypes.Earth.Id, 4));
            ///Random Items:
            KRandom krandom = new KRandom(seed);


            List<int> intList = new List<int>();
            int num1 = 3;
            int minValue = 15;
            int maxValue = 25;
            for (int index1 = 0; index1 < VanillaSpawns.Count; ++index1)
            {
                if (VanillaSpawns[index1].Count != 0)
                {
                    for (int index2 = 0; index2 < num1; ++index2)
                        intList.Add(index1);
                }
            }
            int num2 = krandom.Next(minValue, maxValue);
            for (int index3 = 0; index3 < num2; ++index3)
            {
                int index4 = krandom.Next(0, intList.Count - 1);
                int num3 = intList[index4];
                intList.RemoveAt(index4);
                List<string> stringList = VanillaSpawns[num3];
                destinationsWithDistance.Add(new(stringList[krandom.Next(0, stringList.Count)], num3));
            }

            destinationsWithDistance.Add(new(destinationTypes.Wormhole.Id, VanillaSpawns.Count));
            MaxStarmapDistance = VanillaSpawns.Count;

            for (int distance = 0; distance <= MaxStarmapDistance; distance++)
            {
                VanillaStarmapItems[distance] = new List<string>();
            }

            foreach (var entry in destinationsWithDistance)
            {
                VanillaStarmapItems[entry.second].Add(entry.first);
            }
        }

        public void RemovePoiGroup(string key)
        {
            if (POIs.ContainsKey(key))
                POIs.Remove(key);
        }

        internal StarmapItem AddNewPoiGroupFromPOI(string startPoiId)
        {
            return AddLegacyPOIGroup(startPoiId, 0, CustomCluster.Rings, 1);
        }

        internal StarmapItem AddPoiGroup(string key, SpaceMapPOIPlacement spaceMapPOIPlacement, float numberToSpawn)
        {
            StarmapItem item = new StarmapItem(key, StarmapItemCategory.POI, null);
            item.MakeItemPOI(spaceMapPOIPlacement);
            item.SetSpawnNumber(numberToSpawn, true);
            POIs[key] = item;
            return item;
        }

        internal StarmapItem AddLegacyPOIGroup(string key, int minRing, int maxRing, float numberToSpawn)
        {
            SpaceMapPOIPlacement placement = new SpaceMapPOIPlacement()
            {
                allowedRings = new MinMaxI(minRing, maxRing),
                pois = new List<string> { key },
                numToSpawn = 1,
                avoidClumping = false,
                canSpawnDuplicates = numberToSpawn > 1
            };
            return AddPoiGroup(GetPOIGroupId(placement, true), placement, numberToSpawn);
        }
    }
}
