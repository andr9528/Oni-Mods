﻿using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;

namespace ConveyorTiles
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	[ModInfo("Conveyor Tiles")]
	public class Config : SingletonOptions<Config>
	{
		[Option("STRINGS.MODCONFIG.CONVEYORSPEEDMULTIPLIER.NAME", "STRINGS.MODCONFIG.CONVEYORSPEEDMULTIPLIER.TOOLTIP")]
		[JsonProperty]
		[Limit(0.1f, 10f)]
		public float SpeedMultiplier { get; set; }

		[Option("STRINGS.MODCONFIG.CONVEYORUSESPOWER.NAME", "STRINGS.MODCONFIG.CONVEYORUSESPOWER.TOOLTIP")]
		[JsonProperty]
		[Limit(0, 20)]
		public int TileWattage { get; set; }

		[Option("STRINGS.MODCONFIG.COLOREDGEAR.NAME", "STRINGS.MODCONFIG.COLOREDGEAR.TOOLTIP")]
		[JsonProperty]
		public bool GearTint { get; set; }

		[Option("STRINGS.MODCONFIG.IMMUNEDUPES.NAME", "STRINGS.MODCONFIG.IMMUNEDUPES.TOOLTIP")]
		[JsonProperty]
		public bool Immunes { get; set; }

		[Option("STRINGS.MODCONFIG.IMMUNECRITTERS.NAME", "STRINGS.MODCONFIG.IMMUNECRITTERS.TOOLTIP")]
		[JsonProperty]
		public bool ImmuneCritters { get; set; }

		[Option("STRINGS.MODCONFIG.NOLOGICPORT.NAME", "STRINGS.MODCONFIG.NOLOGICPORT.TOOLTIP")]
		[JsonProperty]
		public bool NoLogicInputs { get; set; }
		public Config()
		{
			TileWattage = 4;
			SpeedMultiplier = 1f;
			GearTint = true;
			Immunes = false;
			NoLogicInputs = false;
			ImmuneCritters = false;
		}
	}
}
