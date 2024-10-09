﻿using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.RocketFueling
{
	internal class SpacefarerLoaderConfig : IBuildingConfig
	{
		public const string ID = "RTB_SpacefarerLoader";
		public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
		public override BuildingDef CreateBuildingDef()
		{

			string[] Materials = new string[]
			{
				MATERIALS.REFINED_METAL
			};
			float[] MaterialCosts = new float[] { 750f };

			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
					ID,
					3,
					2,
					"conduit_link_cross_kanim",
					200,
					60f,
					MaterialCosts,
					Materials,
					1600f,
					BuildLocationRule.Anywhere,
					noise: NOISE_POLLUTION.NONE,
					decor: BUILDINGS.DECOR.PENALTY.TIER0);

			//BuildingTemplates.CreateLadderDef(buildingDef);
			buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
			//buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
			//buildingDef.ForegroundLayer = Grid.SceneLayer.FXFront;
			//buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.Overheatable = false;
			buildingDef.Entombable = false;
			buildingDef.DefaultAnimState = "on";
			buildingDef.AudioCategory = "Metal";
			buildingDef.AudioSize = "small";
			buildingDef.BaseTimeUntilRepair = -1f;

			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.CanMove = false;
			return buildingDef;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			GeneratedBuildings.MakeBuildingAlwaysOperational(go);
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);

			KPrefabID component = go.GetComponent<KPrefabID>();
			component.AddTag(BaseModularLaunchpadPortConfig.LinkTag);
			component.AddTag(GameTags.ModularConduitPort);

			ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
			def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;
			def.linkBuildingTag = BaseModularLaunchpadPortConfig.LinkTag;
			def.objectLayer = ObjectLayer.Building;
			go.AddOrGet<AnimTileable>();
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<SpacefarerLoader>();
		}
	}
}
