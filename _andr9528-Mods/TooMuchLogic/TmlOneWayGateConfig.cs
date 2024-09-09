using UnityEngine;
using Utilities.Abstractions;
using UtilLibs;

namespace TooMuchLogic
{
    public class TmlOneWayGateConfig : LogicGateBaseConfig, IPatchMe
    {
        public static string ID = "TmlOneWayGate";

        /// <inheritdoc />
        public override BuildingDef CreateBuildingDef()
        {
            return CreateBuildingDef(ID, "logic_not_kanim", height: 1);
        }

        /// <inheritdoc />
        public override LogicGateBase.Op GetLogicOp()
        {
            return LogicGateBase.Op.CustomSingle;
        }


        /// <inheritdoc />
        public override LogicGate.LogicGateDescriptions GetDescriptions()
        {
            return new LogicGate.LogicGateDescriptions
            {
                outputOne = new LogicGate.LogicGateDescriptions.Description()
                {
                    name = (string) STRINGS.BUILDINGS.PREFABS.TMLONEWAYGATE.OUTPUT_NAME,
                    active = (string) STRINGS.BUILDINGS.PREFABS.TMLONEWAYGATE.OUTPUT_ACTIVE,
                    inactive = (string) STRINGS.BUILDINGS.PREFABS.TMLONEWAYGATE.OUTPUT_INACTIVE,
                },
            };
        }

        /// <inheritdoc />
        public override CellOffset[] InputPortOffsets => new CellOffset[1]
        {
            CellOffset.none,
        };

        /// <inheritdoc />
        public override CellOffset[] OutputPortOffsets => new CellOffset[1]
        {
            new(1, 0),
        };

        /// <inheritdoc />
        public override CellOffset[] ControlPortOffsets => (CellOffset[]) null;

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
            return GameStrings.Technology.Computers.GenericSensors;
        }
    }
}