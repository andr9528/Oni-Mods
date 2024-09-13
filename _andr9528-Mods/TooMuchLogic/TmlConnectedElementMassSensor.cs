using JetBrains.Annotations;
using KSerialization;
using static STRINGS.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using TooMuchLogic.Services;
using UnityEngine;

namespace TooMuchLogic
{
    public class TmlConnectedElementMassSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim4000ms,
        ISingleSliderControl
    {
        private static Dictionary<int, TmlConnectedElementMassSensor> knownSensors = new();
        private static Dictionary<string, HashSet<int>> rooms = new();

        private int myCell;
        private ConjoinedSensorLogicService<TmlConnectedElementMassSensor> conjoinedSensorLogicService;
        public float minMass = 0;
        public float maxMass = int.MaxValue;
        private bool wasOn;
        private HandleVector<int>.Handle structureTemperature;

        [Serialize] [SerializeField] public float thresholdMass = 20f;
        [Serialize] [SerializeField] public int radius = 0;
        [Serialize] [SerializeField] public bool activateAboveThreshold;
        [Serialize] [SerializeField] public float totalMass;
        [MyCmpAdd] private CopyBuildingSettings copyBuildingSettings;
        [MyCmpGet] private LogicPorts logicPorts;

        private static readonly EventSystem.IntraObjectHandler<TmlConnectedElementMassSensor> OnCopySettingsDelegate =
            new((Action<TmlConnectedElementMassSensor, object>) ((component, data) => component.OnCopySettings(data)));

        private void OnCopySettings(object data)
        {
            var component = ((GameObject) data).GetComponent<TmlConnectedElementMassSensor>();
            if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
                return;
            Threshold = component.Threshold;
            ActivateAboveThreshold = component.ActivateAboveThreshold;
        }

        public float StructureTemperature =>
            GameComps.StructureTemperatures.GetPayload(structureTemperature).Temperature;

        /// <inheritdoc />
        public float GetRangeMinInputField()
        {
            return RangeMin;
        }

        /// <inheritdoc />
        public float GetRangeMaxInputField()
        {
            return GetRanges.Last().peakValue;
        }

        /// <inheritdoc />
        public LocString ThresholdValueUnits()
        {
            return (LocString) "";
        }

        /// <inheritdoc />
        public string Format(float value, bool units)
        {
            return GameUtil.GetFormattedMass(value);
        }

        /// <inheritdoc />
        public float ProcessedSliderValue(float input)
        {
            return Mathf.Round(input);
        }

        /// <inheritdoc />
        public float ProcessedInputValue(float input)
        {
            return input;
        }

        /// <inheritdoc />
        public float Threshold
        {
            get => thresholdMass;
            set => thresholdMass = value;
        }

        /// <inheritdoc />
        public bool ActivateAboveThreshold
        {
            get => activateAboveThreshold;
            set => activateAboveThreshold = value;
        }

        /// <inheritdoc />
        public float CurrentValue => GetTotalMass();

        private float GetTotalMass()
        {
            return totalMass;
        }

        /// <inheritdoc />
        public float RangeMin => minMass;

        /// <inheritdoc />
        public float RangeMax => maxMass;

        /// <inheritdoc />
        public LocString Title => UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TITLE;

        /// <inheritdoc />
        public LocString ThresholdValueName => UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE;

        /// <inheritdoc />
        public string AboveToolTip => (string) UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_ABOVE;

        /// <inheritdoc />
        public string BelowToolTip => (string) UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.PRESSURE_TOOLTIP_BELOW;

        /// <inheritdoc />
        public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

        /// <inheritdoc />
        public int IncrementScale => 1;

        /// <inheritdoc />
        public NonLinearSlider.Range[] GetRanges => new NonLinearSlider.Range[4]
        {
            new(25f, 1000f),
            new(25f, 10000f),
            new(25f, 100000f),
            new(25f, 1000000f),
        };

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe(-905833192, OnCopySettingsDelegate);
        }

        /// <inheritdoc />
        public override void OnCleanUp()
        {
            base.OnCleanUp();
            knownSensors.Remove(myCell);
            conjoinedSensorLogicService.RemoveFromRoom();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            myCell = Grid.PosToCell(this);
            conjoinedSensorLogicService = new ConjoinedSensorLogicService<TmlConnectedElementMassSensor>(radius,
                ref rooms, ref knownSensors, myCell);

            knownSensors.Add(myCell, this);

            structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);
            OnToggle += new Action<bool>(OnSwitchToggled);
            UpdateVisualState(true);
            UpdateLogicCircuit();
            wasOn = switchedOn;
            conjoinedSensorLogicService.CollectConnectedElementCells(UpdateTotalMass, SetTotalMassFromOtherSensor);
        }

        private void OnSwitchToggled(bool toggled_on)
        {
            UpdateLogicCircuit();
            UpdateVisualState();
        }

        private void UpdateVisualState(bool force = false)
        {
            if (!((wasOn != switchedOn) | force))
                return;
            wasOn = switchedOn;
            var component = GetComponent<KBatchedAnimController>();
            component.Play((HashedString) (switchedOn ? "on_pre" : "on_pst"));
            component.Queue((HashedString) (switchedOn ? "on" : "off"));
        }

        public override void UpdateSwitchStatus()
        {
            StatusItem status_item = switchedOn
                ? Db.Get().BuildingStatusItems.LogicSensorStatusActive
                : Db.Get().BuildingStatusItems.LogicSensorStatusInactive;
            GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
        }

        private void UpdateLogicCircuit()
        {
            logicPorts.SendSignal(LogicSwitch.PORT_ID, switchedOn ? 1 : 0);
        }

        private bool SetTotalMassFromOtherSensor()
        {
            if (string.IsNullOrEmpty(conjoinedSensorLogicService.currentRoomId))
                return false;

            int otherSensorCell = rooms[conjoinedSensorLogicService.currentRoomId].First();
            if (otherSensorCell == myCell)
                return false;

            TmlConnectedElementMassSensor sensor = knownSensors[otherSensorCell];
            totalMass = sensor.totalMass;

            return true;
        }

        private void UpdateTotalMass()
        {
            var room = rooms.FirstOrDefault(x => x.Key == conjoinedSensorLogicService.currentRoomId);
            //SgtLogger.log(
            //    $"Found following room ({room})(Mass), when looking for room with following id ({conjoinedSensorLogicService.currentRoomId})");

            if (radius != 0 || room.Value.First() == myCell)
            {
                totalMass = conjoinedSensorLogicService.roomCells.Where(Grid.IsValidCell).Sum(cell => Grid.Mass[cell]);
                return;
            }

            SetTotalMassFromOtherSensor();
        }

        /// <inheritdoc />
        public void Sim4000ms(float dt)
        {
            if (conjoinedSensorLogicService.ShouldCellsBeRecollected())
                conjoinedSensorLogicService.CollectConnectedElementCells(UpdateTotalMass, SetTotalMassFromOtherSensor);
            else
                UpdateTotalMass();

            UpdateLogicOutput();
        }


        private void UpdateLogicOutput()
        {
            if (activateAboveThreshold)
            {
                if ((totalMass <= thresholdMass || IsSwitchedOn) && (totalMass > thresholdMass || !IsSwitchedOn))
                    return;
                Toggle();
            }
            else
            {
                if ((totalMass <= thresholdMass || !IsSwitchedOn) && (totalMass > thresholdMass || IsSwitchedOn))
                    return;
                Toggle();
            }
        }

        /// <inheritdoc />
        public int SliderDecimalPlaces(int index)
        {
            return 0;
        }

        /// <inheritdoc />
        public float GetSliderMin(int index)
        {
            return 0.0f;
        }

        /// <inheritdoc />
        public float GetSliderMax(int index)
        {
            return 100.0f;
        }

        /// <inheritdoc />
        public float GetSliderValue(int index)
        {
            return radius;
        }

        /// <inheritdoc />
        public void SetSliderValue(float percent, int index)
        {
            radius = (int) Math.Round(percent);
            conjoinedSensorLogicService.radius = radius;
            if (radius != 0)
                conjoinedSensorLogicService.RemoveFromRoom();

            conjoinedSensorLogicService.CollectConnectedElementCells(UpdateTotalMass, SetTotalMassFromOtherSensor);
        }

        /// <inheritdoc />
        public string GetSliderTooltipKey(int index)
        {
            return STRINGS.BUILDINGS.PREFABS.TMLCONNECTEDELEMENTMASSSENSOR.SIDESCREEN_TOOLTIP;
        }

        /// <inheritdoc />
        public string GetSliderTooltip(int index)
        {
            return STRINGS.BUILDINGS.PREFABS.TMLCONNECTEDELEMENTMASSSENSOR.SIDESCREEN_TOOLTIP;
        }

        /// <inheritdoc />
        public string SliderTitleKey =>
            "STRINGS.BUILDINGS.PREFABS.TMLCONNECTEDELEMENTMASSSENSOR.SIDESCREEN_TITTLE";

        /// <inheritdoc />
        public string SliderUnits => "";
    }
}