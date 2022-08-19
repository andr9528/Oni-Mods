﻿using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig
{
    [Serializable]
    [RestartRequired]
    [ModInfo("Rocketry Tewaks")]
    public class Config : SingletonOptions<Config>
    {

        [Option("Cartographic Module Scan Range", "Cartographic Modules will instantly reveal hexes in this radius.")]
        [Limit(0, 3)]
        [JsonProperty]
        public int ScannerModuleRange { get; set; }

        [Option("Critter Containment Module Capacity", "Amount of critters the module can hold at once")]
        [Limit(1, 15)]
        [JsonProperty]
        public int CritterStorageCapacity { get; set; }

        [Option("Laser Drillcone Speed", "Mining speed in Kg/s for the Laser Drillcone. (The Basic Drillcone mines at 7.5kg/s).")]
        [Limit(1f, 15f)]
        [JsonProperty]
        public float LaserDrillconeSpeed { get; set; }
        public Config()
        {
            ScannerModuleRange = 1;
            CritterStorageCapacity = 5;
            LaserDrillconeSpeed = 3.75f;
        }
    }
}
