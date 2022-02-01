﻿using Database;
using KnastoronOniMods;
using System.Collections.Generic;
using TUNING;
using UnityEngine;
namespace Robo_Rockets 
{
    class RoboRocketConfig : IBuildingConfig
    {
        public const string ID = "AiModule";
        public const string DisplayName = "Ai Module";
        public const string Description = "A Module to control your Rockets without dupes.";
        public const string Effect = "Fly far away...";


        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            float[] denseTieR0 = BUILDINGS.ROCKETRY_MASS_KG.DENSE_TIER0;
            string[] refinedMetals = MATERIALS.REFINED_METALS;
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;

            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("AiModule", 3, 3, "rocket_pioneer_cargo_module_kanim", 1000, 400f, denseTieR0, refinedMetals, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.PowerOutputOffset = new CellOffset(0, 0);
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.RequiresPowerOutput = true;
            buildingDef.UseWhitePowerOutputConnectorColour = true;
            buildingDef.attachablePosition = new CellOffset(0, 0);
            buildingDef.CanMove = true;
            buildingDef.Cancellable = false;
            buildingDef.ShowInBuildMenu = false;
            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            go.GetComponent<KPrefabID>().AddTag(GameTags.LaunchButtonRocketModule);

            go.AddOrGet<AssignmentGroupController>().generateGroupOnStart = true;
            go.AddOrGet<PassengerRocketModule>().interiorReverbSnapshot = AudioMixerSnapshots.Get().MediumRocketInteriorReverbSnapshot;
            go.AddOrGet<ClustercraftExteriorDoor>().interiorTemplateName = "expansion1::interiors/habitat_small";
            go.AddOrGetDef<SimpleDoorController.Def>();
            go.AddOrGet<NavTeleporter>();
            go.AddOrGet<LaunchableRocketCluster>();
            go.AddOrGet<RocketAiConditions>();
            go.AddOrGet<AccessControl>();

            go.AddOrGet<RocketProcessConditionDisplayTarget>();
            go.AddOrGet<CharacterOverlay>().shouldShowName = true;

            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1] //top module attaches here
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 3), GameTags.Rocket, (AttachableBuilding) null)
            };
        }
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            go.AddOrGet<BuildingCellVisualizer>();
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
           
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MODERATE);
            ModuleBattery moduleBattery = go.AddOrGet<ModuleBattery>();
            moduleBattery.capacity = 25000f;
            moduleBattery.joulesLostPerSecond = 0.6666667f; 
            WireUtilitySemiVirtualNetworkLink virtualNetworkLink = go.AddOrGet<WireUtilitySemiVirtualNetworkLink>();
            virtualNetworkLink.link1 = new CellOffset(0, 0);
            virtualNetworkLink.visualizeOnly = true;
            go.AddOrGet<BuildingCellVisualizer>();
            go.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition)new LimitOneCommandModule());
        }
        public void DoPostConfigureOfInternalControlModule(GameObject go)
        {

            RocketControlStationLaunchWorkable stationLaunchWorkable = (RocketControlStationLaunchWorkable)null;
            List<RocketControlStation> worldItems = Components.RocketControlStations.GetWorldItems(go.GetComponent<ClustercraftExteriorDoor>().GetTargetWorld().id);
            if (worldItems != null && worldItems.Count > 0)
                stationLaunchWorkable = worldItems[0].GetComponent<RocketControlStationLaunchWorkable>();
            Worker test = new Worker();
            stationLaunchWorkable.StartWork(test);

        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<BuildingCellVisualizer>();
        }
    }

}