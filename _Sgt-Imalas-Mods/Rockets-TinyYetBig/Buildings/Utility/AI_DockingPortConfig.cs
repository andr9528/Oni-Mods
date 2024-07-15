﻿using Rockets_TinyYetBig.Docking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.Utility
{
    public class AI_DockingPortConfig : IBuildingConfig
    {
        public const string ID = "RTB_AIModuleDockingPort";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] materialMass = new float[2]
            {
                200f,
                550f
            };
            string[] materialType = new string[2]
            {
                "RefinedMetal",
                "Transparent"
            };

            float[] MatCosts = {
                300f,
                100f
            };
            string[] Materials =
            {
                "RefinedMetal",
                "Plastic"
            };
            EffectorValues tieR2 = NOISE_POLLUTION.NONE;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 2, "rocket_cluster_oxidizer_tank_liquid_kanim", 1000, 60f, MatCosts, Materials, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.DefaultAnimState = "grounded";
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.ObjectLayer = ObjectLayer.Backwall;
            buildingDef.CanMove = true;
            buildingDef.Cancellable = false;

            buildingDef.attachablePosition = new CellOffset(0, 0);

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();

            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.Rocket, (AttachableBuilding) null)
            };

        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Prioritizable.AddRef(go);

            go.AddOrGet<VirtualDockable>();
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.MODERATE);
        }
    }
}
