using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using KSerialization;
using Microsoft.Build.Utilities;
using STRINGS;
using TooManySensors.Services;
using UnityEngine;
using UtilLibs;
using static STRINGS.MISC.STATUSITEMS;
using static STRINGS.UI;
using CODEX = STRINGS.CODEX;

namespace TooManySensors
{
    public class TmlConnectedElementTemperatureSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim4000ms,
        ISingleSliderControl
    {
        private static Dictionary<int, TmlConnectedElementTemperatureSensor> knownSensors = new();
        private static Dictionary<string, HashSet<int>> rooms = new();

        [CanBeNull] private string currentRoomId;
        private bool buildingRoom = false;
        private HandleVector<int>.Handle structureTemperature;
        [Serialize] [SerializeField] public float thresholdTemperature = 280f;
        [Serialize] [SerializeField] public int radius = 0;
        [Serialize] [SerializeField] public bool activateOnWarmerThan;
        public float minTemp;
        public float maxTemp = 373.15f;
        [Serialize] [SerializeField] public float averageTemp;
        private bool wasOn;
        public HashSet<int> roomCells;
        [MyCmpAdd] private CopyBuildingSettings copyBuildingSettings;
        [MyCmpGet] private LogicPorts logicPorts;

        private static readonly EventSystem.IntraObjectHandler<TmlConnectedElementTemperatureSensor>
            OnCopySettingsDelegate =
                new((Action<TmlConnectedElementTemperatureSensor, object>) ((component, data) =>
                    component.OnCopySettings(data)));

        private void OnCopySettings(object data)
        {
            var component = ((GameObject) data).GetComponent<TmlConnectedElementTemperatureSensor>();
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

        /// <inheritdoc />
        public override void OnCleanUp()
        {
            base.OnCleanUp();
            int myCell = Grid.PosToCell(this);
            knownSensors.Remove(myCell);
            RemoveFromRoom();
        }

        private void RemoveFromRoom()
        {
            int myCell = Grid.PosToCell(this);
            if (string.IsNullOrWhiteSpace(currentRoomId))
                return;

            rooms[currentRoomId].Remove(myCell);
            currentRoomId = "";
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            roomCells = new HashSet<int>();

            int myCell = Grid.PosToCell(this);
            knownSensors.Add(myCell, this);

            structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);
            OnToggle += new Action<bool>(OnSwitchToggled);
            UpdateVisualState(true);
            UpdateLogicCircuit();
            wasOn = switchedOn;
            OnRoomRebuild();
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

        private void OnRoomRebuild()
        {
            if (buildingRoom)
                return;
            buildingRoom = true;

            int myCell = Grid.PosToCell(this);
            if ((double) Grid.Mass[myCell] <= 0.0)
                return;

            roomCells.Clear();

            Element startElement = Grid.Element[myCell];

            var grapper = new CellGrapperService();
            roomCells = radius == 0
                ? grapper.GrabCellsIterative(myCell, startElement, EarlyBreak)
                : grapper.GrabCellsRadius(myCell, startElement, radius);

            if (radius == 0 && string.IsNullOrWhiteSpace(currentRoomId))
            {
                currentRoomId = Guid.NewGuid().ToString();
                rooms.Add(currentRoomId, new HashSet<int>());
                rooms[currentRoomId].Add(myCell);
            }

            buildingRoom = false;

            UpdateAverageTemp();
        }

        private bool EarlyBreak(int source, int cell)
        {
            return radius == 0 && knownSensors.ContainsKey(cell) && UpdateTempFromOtherSensor(source, cell);
        }

        private bool UpdateTempFromOtherSensor(int source, int cell)
        {
            if (source == cell)
                return false;

            var existingRoom = rooms.FirstOrDefault(x => x.Value.Contains(cell));
            if (existingRoom.Equals(default(KeyValuePair<string, HashSet<int>>)))
                return false;

            existingRoom.Value.Add(source);
            currentRoomId = existingRoom.Key;

            return SetAverageTempFromOtherSensor();
        }

        private bool SetAverageTempFromOtherSensor()
        {
            if (string.IsNullOrEmpty(currentRoomId))
                return false;

            int otherSensorCell = rooms[currentRoomId].First();
            int myCell = Grid.PosToCell(this);
            if (otherSensorCell == myCell)
                return false;

            TmlConnectedElementTemperatureSensor sensor = knownSensors[otherSensorCell];
            averageTemp = sensor.averageTemp;

            return true;
        }

        private void UpdateAverageTemp()
        {
            int myCell = Grid.PosToCell(this);
            var room = rooms.FirstOrDefault(x => x.Key == currentRoomId);

            if (radius != 0 || room.Value.First() == myCell)
            {
                float averageRoomTemperature = roomCells.Where(Grid.IsValidCell).Sum(cell => Grid.Temperature[cell]);
                averageRoomTemperature /= roomCells.Count;
                averageTemp = averageRoomTemperature;
                return;
            }

            SetAverageTempFromOtherSensor();
        }

        /// <inheritdoc />
        public void Sim4000ms(float dt)
        {
            int myCell = Grid.PosToCell(this);
            var room = rooms.FirstOrDefault(x => x.Key == currentRoomId);

            if (string.IsNullOrEmpty(currentRoomId) ||
                (!string.IsNullOrEmpty(currentRoomId) && room.Value.First() == myCell))
                OnRoomRebuild();
            else
                UpdateAverageTemp();

            UpdateLogicOutput();
        }

        private void UpdateLogicOutput()
        {
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
            if (radius != 0)
                RemoveFromRoom();
        }

        /// <inheritdoc />
        public string GetSliderTooltipKey(int index)
        {
            return STRINGS.BUILDINGS.PREFABS.TMLCONNECTEDELEMENTTEMPERATURESENSOR.SIDESCREEN_TOOLTIP;
        }

        /// <inheritdoc />
        public string GetSliderTooltip(int index)
        {
            return STRINGS.BUILDINGS.PREFABS.TMLCONNECTEDELEMENTTEMPERATURESENSOR.SIDESCREEN_TOOLTIP;
        }

        /// <inheritdoc />
        public string SliderTitleKey =>
            "STRINGS.BUILDINGS.PREFABS.TMLCONNECTEDELEMENTTEMPERATURESENSOR.SIDESCREEN_TITTLE";

        /// <inheritdoc />
        public string SliderUnits => "";
    }
}