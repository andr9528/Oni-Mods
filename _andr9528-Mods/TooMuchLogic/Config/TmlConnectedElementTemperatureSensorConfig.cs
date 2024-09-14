using System.Collections.Generic;
using TooMuchLogic.Logic;
using TUNING;
using UnityEngine;
using Utilities.Abstractions;
using UtilLibs;

namespace TooMuchLogic
{
    public class TmlConnectedElementTemperatureSensorConfig : IBuildingConfig, IPatchMe
    {
        public static string ID = "TmlConnectedElementTemperatureSensor";

        /// <inheritdoc />
        public override BuildingDef CreateBuildingDef()
        {
            string id = ID;
            var constructionAmount = new float[2] {50f, 25f,};
            var constructionResources = new string[2] {"RefinedMetal", "Diamond",};
            var meltingPoint = 1600f; // 1600 Hundred what? Potatoes?
            EffectorValues decorPenalty = BUILDINGS.DECOR.PENALTY.TIER0;
            EffectorValues noise = NOISE_POLLUTION.NONE;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 1, 1, "switchthermal_kanim", 30, 45f,
                constructionAmount, constructionResources, meltingPoint, BuildLocationRule.Anywhere, decorPenalty,
                noise);

            buildingDef.Overheatable = false;
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.ViewMode = OverlayModes.Logic.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.AlwaysOperational = true;
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>();
            buildingDef.LogicOutputPorts.Add(LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0),
                (string) STRINGS.BUILDINGS.PREFABS.TMLCONNECTEDELEMENTTEMPERATURESENSOR.LOGIC_PORT,
                (string) STRINGS.BUILDINGS.PREFABS.TMLCONNECTEDELEMENTTEMPERATURESENSOR.LOGIC_PORT_ACTIVE,
                (string) STRINGS.BUILDINGS.PREFABS.TMLCONNECTEDELEMENTTEMPERATURESENSOR.LOGIC_PORT_INACTIVE, true));
            SoundEventVolumeCache.instance.AddVolume("switchthermal_kanim", "PowerSwitch_on",
                NOISE_POLLUTION.NOISY.TIER3);
            SoundEventVolumeCache.instance.AddVolume("switchthermal_kanim", "PowerSwitch_off",
                NOISE_POLLUTION.NOISY.TIER3);
            GeneratedBuildings.RegisterWithOverlay(OverlayModes.Logic.HighlightItemIDs, ID);

            return buildingDef;
        }

        /// <inheritdoc />
        public override void DoPostConfigureComplete(GameObject go)
        {
            var temperatureSensor = go.AddOrGet<TmlConnectedElementTemperatureSensor>();
            temperatureSensor.manuallyControlled = false;
            temperatureSensor.minTemp = 0.0f;
            temperatureSensor.maxTemp = 9999f;
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
        }

        /// <inheritdoc />
        public string GetId()
        {
            return ID;
        }

        /// <inheritdoc />
        public string GetPlanMenuCategory()
        {
            return GameStrings.PlanMenuCategory.Automation;
        }

        /// <inheritdoc />
        public string GetTechnology()
        {
            return GameStrings.Technology.Gases.HVAC;
        }
    }
}