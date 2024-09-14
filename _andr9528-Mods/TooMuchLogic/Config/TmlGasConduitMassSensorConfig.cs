using System.Collections.Generic;
using TooMuchLogic.Logic;
using UnityEngine;
using Utilities.Abstractions;
using UtilLibs;

namespace TooMuchLogic.Config
{
    public class TmlGasConduitMassSensorConfig : ConduitSensorConfig, IPatchMe
    {
        public static string ID = "TmlGasConduitMassSensor";

        /// <inheritdoc />
        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = CreateBuildingDef(ID, "gas_germs_sensor_kanim", new float[2]
            {
                TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER0[0],
                TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0],
            }, new string[2] {"RefinedMetal", "Plastic",}, new List<LogicPorts.Port>()
            {
                LogicPorts.Port.OutputPort(LogicSwitch.PORT_ID, new CellOffset(0, 0),
                    (string) STRINGS.BUILDINGS.PREFABS.TMLGASCONDUITMASSSENSOR.LOGIC_PORT,
                    (string) STRINGS.BUILDINGS.PREFABS.TMLGASCONDUITMASSSENSOR.LOGIC_PORT_ACTIVE,
                    (string) STRINGS.BUILDINGS.PREFABS.TMLGASCONDUITMASSSENSOR.LOGIC_PORT_INACTIVE, true),
            });
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
            return buildingDef;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            base.DoPostConfigureComplete(go);
            var conduitDiseaseSensor = go.AddComponent<TmlConduitMassSensor>();
            conduitDiseaseSensor.conduitType = ConduitType;
            conduitDiseaseSensor.Threshold = 0.0f;
            conduitDiseaseSensor.ActivateAboveThreshold = true;
            conduitDiseaseSensor.manuallyControlled = false;
            conduitDiseaseSensor.defaultState = false;
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits);
        }

        /// <inheritdoc />
        public override ConduitType ConduitType => ConduitType.Gas;

        /// <inheritdoc />
        public string GetId()
        {
            return ID;
        }

        /// <inheritdoc />
        public string GetPlanMenuCategory()
        {
            return GameStrings.PlanMenuCategory.Ventilation;
        }

        /// <inheritdoc />
        public string GetTechnology()
        {
            return GameStrings.Technology.Gases.PressureManagement;
        }
    }
}