using System;
using System.Collections.Generic;
using System.Linq;
using KSerialization;
using STRINGS;
using UnityEngine;
using static STRINGS.MISC.STATUSITEMS;
using static STRINGS.UI;

namespace SensoryOverload
{
    public class LogicElementTemperatureSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim4000ms
    {
        private HandleVector<int>.Handle structureTemperature;
        [Serialize] public float thresholdTemperature = 280f;
        [Serialize] public bool activateOnWarmerThan;
        public float minTemp;
        public float maxTemp = 373.15f;
        private const int NumFrameDelay = 8;
        private float averageTemp;
        private bool wasOn;
        private int sampleIdx;
        private HashSet<int> RoomCells = new();
        [MyCmpAdd] private CopyBuildingSettings copyBuildingSettings;
        [MyCmpGet] private LogicPorts logicPorts;

        private static readonly EventSystem.IntraObjectHandler<LogicElementTemperatureSensor> OnCopySettingsDelegate =
            new((Action<LogicElementTemperatureSensor, object>) ((component, data) => component.OnCopySettings(data)));

        private void OnCopySettings(object data)
        {
            var component = ((GameObject) data).GetComponent<LogicElementTemperatureSensor>();
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
            return GameUtil.GetConvertedTemperature(RangeMin);
        }

        /// <inheritdoc />
        public float GetRangeMaxInputField()
        {
            return GameUtil.GetConvertedTemperature(RangeMax);
        }

        /// <inheritdoc />
        public LocString ThresholdValueUnits()
        {
            var locString = (LocString) null;
            switch (GameUtil.temperatureUnit)
            {
                case GameUtil.TemperatureUnit.Celsius:
                    locString = UNITSUFFIXES.TEMPERATURE.CELSIUS;
                    break;
                case GameUtil.TemperatureUnit.Fahrenheit:
                    locString = UNITSUFFIXES.TEMPERATURE.FAHRENHEIT;
                    break;
                case GameUtil.TemperatureUnit.Kelvin:
                    locString = UNITSUFFIXES.TEMPERATURE.KELVIN;
                    break;
            }

            return locString;
        }

        /// <inheritdoc />
        public string Format(float value, bool units)
        {
            return GameUtil.GetFormattedTemperature(value, displayUnits: units, roundInDestinationFormat: true);
        }

        /// <inheritdoc />
        public float ProcessedSliderValue(float input)
        {
            return Mathf.Round(input);
        }

        /// <inheritdoc />
        public float ProcessedInputValue(float input)
        {
            return GameUtil.GetTemperatureConvertedToKelvin(input);
        }

        /// <inheritdoc />
        public float Threshold
        {
            get => thresholdTemperature;
            set => thresholdTemperature = value;
        }

        /// <inheritdoc />
        public bool ActivateAboveThreshold
        {
            get => activateOnWarmerThan;
            set => activateOnWarmerThan = value;
        }

        /// <inheritdoc />
        public float CurrentValue => GetTemperature();

        private float GetTemperature()
        {
            return averageTemp;
        }

        /// <inheritdoc />
        public float RangeMin => minTemp;

        /// <inheritdoc />
        public float RangeMax => maxTemp;

        /// <inheritdoc />
        public LocString Title => UISIDESCREENS.TEMPERATURESWITCHSIDESCREEN.TITLE;

        /// <inheritdoc />
        public LocString ThresholdValueName => UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE;

        /// <inheritdoc />
        public string AboveToolTip => (string) UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_ABOVE;

        /// <inheritdoc />
        public string BelowToolTip => (string) UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.TEMPERATURE_TOOLTIP_BELOW;

        /// <inheritdoc />
        public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

        /// <inheritdoc />
        public int IncrementScale => 1;

        /// <inheritdoc />
        public NonLinearSlider.Range[] GetRanges => new NonLinearSlider.Range[4]
        {
            new(25f, 260f),
            new(50f, 400f),
            new(12f, 1500f),
            new(13f, 10000f),
        };

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Subscribe(-905833192, OnCopySettingsDelegate);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);
            OnToggle += new Action<bool>(OnSwitchToggled);
            UpdateVisualState(true);
            UpdateLogicCircuit();
            wasOn = switchedOn;
            OnRoomRebuild(null);
            averageTemp = RecalculateAverageRoomTemperature();
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

        public void OnRoomRebuild(object data)
        {
            int myCell = Grid.PosToCell(this);
            if ((double) Grid.Mass[myCell] <= 0.0)
                return;

            RoomCells.Clear();

            var visited = new HashSet<int>();

            Element startElement = Grid.Element[myCell];

            GrabCellsRecursive(myCell, ref visited, startElement);

            sampleIdx = 0;
        }

        private void GrabCellsRecursive(int source, ref HashSet<int> visitedCells, Element sourceElement)
        {
            if (visitedCells.Contains(source))
                return;

            visitedCells.Add(source);

            if ((!Grid.IsGas(source) && !Grid.IsLiquid(source)) || Grid.IsCellOpenToSpace(source)) return;
            if (Grid.Element[source] != sourceElement /*|| visitedCells.Count >= 10000*/)
                return;

            RoomCells.Add(source);

            int above = Grid.CellAbove(source);
            int below = Grid.CellBelow(source);
            int left = Grid.CellLeft(source);
            int right = Grid.CellRight(source);

            if (Grid.IsValidCell(above))
                GrabCellsRecursive(above, ref visitedCells, sourceElement);
            if (Grid.IsValidCell(below))
                GrabCellsRecursive(below, ref visitedCells, sourceElement);
            if (Grid.IsValidCell(left))
                GrabCellsRecursive(left, ref visitedCells, sourceElement);
            if (Grid.IsValidCell(right))
                GrabCellsRecursive(right, ref visitedCells, sourceElement);
        }

        /// <inheritdoc />
        public void Sim4000ms(float dt)
        {
            OnRoomRebuild(null);
            averageTemp = RecalculateAverageRoomTemperature();

            if (activateOnWarmerThan)
            {
                if ((averageTemp <= thresholdTemperature || IsSwitchedOn) &&
                    (averageTemp > thresholdTemperature || !IsSwitchedOn))
                    return;
                Toggle();
            }
            else
            {
                if ((averageTemp <= thresholdTemperature || !IsSwitchedOn) &&
                    (averageTemp > thresholdTemperature || IsSwitchedOn))
                    return;
                Toggle();
            }
        }

        private float RecalculateAverageRoomTemperature()
        {
            float averageRoomTemperature = RoomCells.Where(Grid.IsValidCell).Sum(cell => Grid.Temperature[cell]);

            averageRoomTemperature /= RoomCells.Count;

            return averageRoomTemperature;
        }
    }
}