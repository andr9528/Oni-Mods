﻿using LogicSatellites.Behaviours;
using LogicSatellites.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static ComplexRecipe;

namespace LogicSatellites.Buildings
{
    class SatelliteCarrierModuleConfig : IBuildingConfig
    {
        public const string ID = "LS_SatelliteCarrierModule";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            float[] MatCosts = new[] { 400f, 200f };
            string[] Materials = new string[2]
            {
                "RefinedMetal",
                "Plastic"
            };
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 5, "satelite_deployer_module_kanim", 1000, 30f, MatCosts, Materials, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.DefaultAnimState = "satelite_construction";
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.RequiresPowerInput = false;
            buildingDef.CanMove = true;
            buildingDef.Cancellable = false;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.AddOrGet<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            Storage storage = go.AddComponent<Storage>();
            storage.showInUI = true; //??
            storage.allowItemRemoval = false;
            storage.useWideOffsets = true;
            storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);


            go.AddOrGet<SatelliteCarrierModule>();

            BuildingInternalConstructorRocket.Def def1 = go.AddOrGetDef<BuildingInternalConstructorRocket.Def>();
            def1.constructionUnits = 20f;
            def1.ConstructionMatID = SatelliteComponentConfig.ID;
            def1.outputIDs = new List<Tag>()
            {
                SatelliteKitConfig.ID.ToTag()
            };
            def1.spawnIntoStorage = true;
            def1.storage = (DefComponent<Storage>)storage;
            def1.constructionSymbol = "under_construction";
            go.AddOrGet<BuildingInternalConstructorRocketWorkable>().SetWorkTime(30f);


            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, (AttachableBuilding) null)
            };
            //go.AddComponent<SatelliteSelection>();

        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Prioritizable.AddRef(go);
            AddFakeFloor(go);
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MODERATE);
        }

        private void AddFakeFloor(GameObject go)
        {
            FakeFloorAdder fakeFloorAdder = go.AddOrGet<FakeFloorAdder>();
            fakeFloorAdder.floorOffsets = new CellOffset[5]
            {
                new CellOffset(-2, -1),
                new CellOffset(-1, -1),
                new CellOffset(0, -1),
                new CellOffset(1, -1),
                new CellOffset(2, -1)
            };
            fakeFloorAdder.initiallyActive = false;
        }
    }
}
