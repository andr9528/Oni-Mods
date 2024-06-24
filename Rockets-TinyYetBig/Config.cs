﻿using Newtonsoft.Json;
using PeterHan.PLib;
using PeterHan.PLib.Options;
using Rockets_TinyYetBig.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig
{
    [Serializable]
    [RestartRequired]
    [ConfigFile(SharedConfigLocation: true)]
    [ModInfo("https://github.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods", "preview.png")]
    public class Config : SingletonOptions<Config>, PeterHan.PLib.Options.IOptions
    {
        public static bool SpaceStationsPossible =>
               Instance.CompressInteriors
            && Instance.EnableAdvWorldSelector
            && Instance.SpaceStationsAndTech
            && Instance.NeutroniumMaterial
            ;


        //[Option("Vanilla+ Preset", "Load a settings preset: All the bugfixes and qol improvements, everything else disabled")]
        //public Action<object> LoadVanillaPlusSettings { get 
        //    {
        //        return i =>
        //        {
        //            EnableAdvWorldSelector = true;
        //            CompressInteriors = true;
        //            ScannerModuleRangeRadius = 4;
        //            ScannerModuleScanSpeed = 0.33f;
        //            HabitatPowerPlug = true;
        //            EnableExtendedHabs = false;
        //            HabitatInteriorRadiation = false;
        //            HabitatInteriorPortImprovements = true;
        //            SlimLargeEngines = false;

        //            ///Drilling&Shipping
        //            EnableCritterStorage = false;
        //            CritterStorageCapacity = 5;
        //            EnableLaserDrill = false;
        //            LaserDrillconeSpeed = 3.75f;
        //            EnableFridge = false;
        //            InfinitePOI = false;
        //            EnableLargeCargoBays = false;
        //            InsulatedCargoBays = true;
        //            EnableRadboltStorage = false;
        //            EnableDrillSupport = false;
        //            LaserDrillconeSpeed = 25;
        //            DrillconeSupportDiamondMass = 1500;
        //            PilotSkillAffectsDrillSpeed = false;
        //            RefillDrillSupport = false;
        //            EnablePOISensor = false;

        //            RebalancedCargoCapacity = false;
        //            GasCargoBayKgPerUnit = 500;
        //            LiquidCargoBayKgPerUnit = 1250;
        //            SolidCargoBayKgPerUnit = 2000;
        //            SmallCargoBayUnits = 9;
        //            MediumCargoBayUnits = 27;
        //            CollossalCargoBayUnits = 64;


        //            /// Fuel&Logistics
        //            BuffLargeOxidizer = true;
        //            EthanolEngines = true;
        //            Boosters = false;
        //            EnableNatGasEngine = false;
        //            EnableNatGasEngineRange = 15;
        //            EnableEarlyGameFuelTanks = false;
        //            EnableFuelLoaders = false;
        //            EnableWallAdapter = false;
        //            EnableBunkerPlatform = false;

        //            /// Power&Utility
        //            EnableSolarNosecone = false;
        //            EnableGenerators = false;
        //            EnableSmolBattery = false;
        //            IsotopeDecayTime = 50;

        //            /// SpaceStations
        //            SpaceStationsAndTech = false;
        //            RocketDocking =  false;
        //            NeutroniumMaterial = false;

        //            /// EasterEggs
        //            SpiceEyes = false;
        //        };
        //    }  
        //}


        #region vanillaplus
        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEADVWORLDSELECTOR.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEADVWORLDSELECTOR.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
        [JsonProperty]
        public bool EnableAdvWorldSelector { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.COMPRESSINTERIORS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.COMPRESSINTERIORS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
        [JsonProperty]
        public bool CompressInteriors { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEEXTENDEDHABS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
        [JsonProperty]
        public bool EnableExtendedHabs { get; set; }=true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.SCANNERMODULERANGERADIUS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.SCANNERMODULERANGERADIUS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
        [Limit(0, 6)]
        [JsonProperty]
        public int ScannerModuleRangeRadius { get; set; } = 4;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.SCANNERMODULESCANSPEED.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.SCANNERMODULESCANSPEED.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
        [Limit(0.1f, 1f)]
        [JsonProperty]
        public float ScannerModuleScanSpeed { get; set; } = 0.4f;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.HABITATPOWERPLUG.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.HABITATPOWERPLUG.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
        [JsonProperty]
        public bool HabitatPowerPlug { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.HABITATINTERIORRADIATION.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.HABITATINTERIORRADIATION.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
        [JsonProperty]
        public bool HabitatInteriorRadiation { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.HABITATINTERIORPORTIMPROVEMENTS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.HABITATINTERIORPORTIMPROVEMENTS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
        [JsonProperty] 
        public bool HabitatInteriorPortImprovements { get; set; } = true;


        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.SLIMLARGEENGINES.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.SLIMLARGEENGINES.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.A_ROCKETRYPLUS")]
        [JsonProperty]
        public bool SlimLargeEngines { get; set; } = false;

        #endregion

        #region mining&shipping

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLELASERDRILL.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLESINGLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        public bool EnableLaserDrill { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.LASERDRILLCONESPEED.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.LASERDRILLCONESPEED.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [Limit(1f, 15f)]
        [JsonProperty]
        public float LaserDrillconeSpeed { get; set; } = 4.875f;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.LASERDRILLCONECAPACITY.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.LASERDRILLCONECAPACITY.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [Limit(1000f, 20000f)]
        [JsonProperty]
        public float LaserDrillconeCapacity { get; set; } = 6000f;


        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEDRILLSUPPORT.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLESINGLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        public bool EnableDrillSupport { get; set; } = true;


        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.DRILLCONESUPPORTBOOST.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.DRILLCONESUPPORTBOOST.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        [Limit(0, 100)]
        public int DrillconeSupportSpeedBoost { get; set; } = 20;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.DRILLCONESUPPORTDIAMONDMASS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.DRILLCONESUPPORTDIAMONDMASS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        [Limit(1000, 10000)]
        public int DrillconeSupportDiamondMass { get; set; } = 1500;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.REFILLDRILLSUPPORT.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.REFILLDRILLSUPPORT.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        public bool RefillDrillSupport { get; set; } = false;


        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.PILOTSKILLAFFECTSDRILLSPEED.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.PILOTSKILLAFFECTSDRILLSPEED.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        public bool PilotSkillAffectsDrillSpeed { get; set; } = true;


        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.INFINITEPOI.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.INFINITEPOI.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        public bool InfinitePOI { get; set; } = false;


        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEFRIDGE.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLESINGLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        public bool EnableFridge { get; set; } = true;


        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLELARGECARGOBAYS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        public bool EnableLargeCargoBays { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.INSULATEDCARGOBAYS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.INSULATEDCARGOBAYS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        public bool InsulatedCargoBays { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLERADBOLTSTORAGE.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLESINGLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        public bool EnableRadboltStorage { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.RADBOLTSTORAGECAPACITY.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.RADBOLTSTORAGECAPACITY.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [Limit(1000, 10000)]
        [JsonProperty]
        public float RadboltStorageCapacity { get; set; } = 3000;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEPOISENSOR.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLESINGLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        public bool EnablePOISensor { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLECRITTERSTORAGE.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLESINGLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        public bool EnableCritterStorage { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CRITTERSTORAGECAPACITY.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CRITTERSTORAGECAPACITY.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [Limit(1, 15)]
        [JsonProperty]
        public int CritterStorageCapacity { get; set; } = 5;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.REBALANCEDCARGOCAPACITY.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.REBALANCEDCARGOCAPACITY.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [JsonProperty]
        public bool RebalancedCargoCapacity { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.GASCARGOBAYKGPERUNIT", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.UNITDESCRIPTION", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [Limit(200, 1500)]
        [JsonProperty]
        public int GasCargoBayKgPerUnit { get; set; } = 500;


        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.LIQUIDCARGOBAYKGPERUNIT", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.UNITDESCRIPTION", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [Limit(500, 2000)]
        [JsonProperty]
        public int LiquidCargoBayKgPerUnit { get; set; } = 1250;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.SOLIDCARGOBAYKGPERUNIT", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.UNITDESCRIPTION", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [Limit(800, 6000)]
        [JsonProperty]
        public int SolidCargoBayKgPerUnit { get; set; } = 2000;


        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.SMALLCARGOBAYUNITS", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [Limit(3, 32)]
        [JsonProperty]
        public float SmallCargoBayUnits { get; set; } = 9;


        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.MEDIUMCARGOBAYUNITS", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [Limit(6, 64)]
        [JsonProperty]
        public float MediumCargoBayUnits { get; set; } = 27;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.COLLOSSALCARGOBAYUNITS", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CARGOBAYUNITS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.B_MININGSHIPPING")]
        [Limit(9, 128)]
        [JsonProperty]
        public float CollossalCargoBayUnits { get; set; } = 64;

        #endregion

        #region Fuel&Logistics

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.BUFFLARGEOXIDIZER.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.BUFFLARGEOXIDIZER.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
        [JsonProperty]
        public bool BuffLargeOxidizer { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ETHANOLENGINES.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.ETHANOLENGINES.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
        [JsonProperty]
        public bool EthanolEngines { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEBOOSTERS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
        [JsonProperty]
#if DEBUG
        public bool EnableBoosters { get; set; }
#else
        private bool EnableBoosters { get; set; }
#endif
        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLENATGASENGINE.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLESINGLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
        [JsonProperty]
        public bool EnableNatGasEngine { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.NATGASENGINERANGE.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.NATGASENGINERANGE.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
        [JsonProperty]
        [Limit(8, 20)]
        public int NatGasEngineRange { get; set; } = 15;
        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEELECTRICENGINE.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLESINGLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
        [JsonProperty]
        private bool EnableElectricEngine { get; set; }

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEEARLYGAMEFUELTANKS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
        [JsonProperty]
        public bool EnableEarlyGameFuelTanks { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEFUELLOADERS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
        [JsonProperty]
        public bool EnableFuelLoaders { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEROCKETLOADERLOGICOUTPUTS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEROCKETLOADERLOGICOUTPUTS.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
        [JsonProperty]
        public bool EnableRocketLoaderLogicOutputs { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEWALLADAPTER.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
        [JsonProperty]
        public bool EnableWallAdapter { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEBUNKERPLATFORM.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.C_FUELLOGISTICS")]
        [JsonProperty]
        public bool EnableBunkerPlatform { get; set; } = true;
        #endregion

        #region Power&Utility

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLESOLARNOSECONE.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLESINGLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.D_POWERUTILITY")]
        [JsonProperty]
        public bool EnableSolarNosecone { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLEGENERATORS.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLEMULTI", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.D_POWERUTILITY")]
        [JsonProperty]
        public bool EnableGenerators { get; set; } = true;


        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ENABLESMOLBATTERY.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.TOGGLESINGLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.D_POWERUTILITY")]
        [JsonProperty]
        public bool EnableSmolBattery { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ISOTOPEDECAYTIME.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.ISOTOPEDECAYTIME.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.D_POWERUTILITY")]
        [Limit(10f, 200f)]
        [JsonProperty]
        public float IsotopeDecayTime { get; set; } = 50;
        #endregion
        #region SpaceStations

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.SPACESTATIONSANDTECH.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.SPACESTATIONSANDTECH.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.E_SPACEEXPANSION")]
        [JsonProperty]
        public bool SpaceStationsAndTech { get; set; } = false;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.ROCKETDOCKING.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.ROCKETDOCKING.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.E_SPACEEXPANSION")]
        [JsonProperty]
        public bool RocketDocking { get; set; } = true;

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.NEUTRONIUMMATERIAL.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.NEUTRONIUMMATERIAL.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.E_SPACEEXPANSION")]
        [JsonProperty]
        public bool NeutroniumMaterial { get; set; } = true;

        #endregion

        #region EasterEggs

        [Option("STRINGS.OPTIONS_ROCKETRYEXPANDED.SPICEEYES.TITLE", "STRINGS.OPTIONS_ROCKETRYEXPANDED.SPICEEYES.TOOLTIP", "STRINGS.OPTIONS_ROCKETRYEXPANDED.CATEGORIES.F_EASTEREGGS")]
        [JsonProperty]
        public bool SpiceEyes { get; set; } = true;

        #endregion

        public IEnumerable<IOptionsEntry> CreateOptions()
        {
            return new List<IOptionsEntry>();
        }

        public void OnOptionsChanged()
        {
            SpaceStationsAndTech = SpaceStationsPossible;
            POptions.WriteSettings(this);
        }
    }
}
