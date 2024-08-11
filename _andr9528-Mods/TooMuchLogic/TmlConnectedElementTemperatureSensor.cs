using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using KSerialization;
using Microsoft.Build.Utilities;
using STRINGS;
using UnityEngine;
using UtilLibs;
using static STRINGS.MISC.STATUSITEMS;
using static STRINGS.UI;
using CODEX = STRINGS.CODEX;

namespace TooManySensors
{
    public class TmlConnectedElementTemperatureSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim4000ms
    {
        private static Dictionary<int, TmlConnectedElementTemperatureSensor> knownSensors = new();
        private static Dictionary<string, List<int>> rooms = new();

        [CanBeNull] private string currentRoomId;
        private bool buildingRoom = false;
        private HandleVector<int>.Handle structureTemperature;
        [Serialize] [SerializeField] public float thresholdTemperature = 280f;
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
            if (!string.IsNullOrWhiteSpace(currentRoomId))
                rooms[currentRoomId].Remove(myCell);
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

            GrabCellsIterative(myCell, startElement);

            if (string.IsNullOrWhiteSpace(currentRoomId))
            {
                currentRoomId = Guid.NewGuid().ToString();
                rooms.Add(currentRoomId, new List<int>());
                rooms[currentRoomId].Add(myCell);
            }

            buildingRoom = false;
        }

        private bool IsCellGood(int source, Element sourceElement)
        {
            if (!Grid.IsValidCell(source))
                return false;
            if (Grid.IsCellOpenToSpace(source))
                return false;
            if (Grid.Mass[source] <= 0.0)
                return false;
            if (!(Grid.IsGas(source) || Grid.IsLiquid(source)))
                return false;
            if (Grid.Element[source] != sourceElement)
                return false;
            return true;
        }

        private void GrabCellsIterative(int source, Element sourceElement)
        {
            //CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(source);
            //SgtLogger.log($"Cell {source} is inside Cavity with {cavityForCell.numCells} total cells");

            HashSet<int> visited = new();
            Queue<int> queue = new();
            queue.Enqueue(source);

            while (queue.Any())
            {
                int cell = queue.Dequeue();
                if (knownSensors.ContainsKey(cell) && UpdateTempFromOtherSensor(source, cell))
                    break;

                if (visited.Contains(cell))
                    continue;
                visited.Add(cell);

                if (!IsCellGood(cell, sourceElement))
                    continue;

                roomCells.Add(cell);
                queue.Enqueue(Grid.CellAbove(cell));
                queue.Enqueue(Grid.CellBelow(cell));
                queue.Enqueue(Grid.CellLeft(cell));
                queue.Enqueue(Grid.CellRight(cell));
            }
        }

        private bool UpdateTempFromOtherSensor(int source, int cell)
        {
            var existingRoom = rooms.FirstOrDefault(x => x.Value.Contains(cell));
            if (existingRoom.Equals(default(KeyValuePair<string, List<int>>)))
                return false;

            existingRoom.Value.Add(source);
            currentRoomId = existingRoom.Key;

            return SetAverageTempFromOtherSensor();
        }

        private bool SetAverageTempFromOtherSensor()
        {
            if (string.IsNullOrEmpty(currentRoomId))
                return false;

            TmlConnectedElementTemperatureSensor sensor = knownSensors[rooms[currentRoomId].First()];
            averageTemp = sensor.averageTemp;

            return true;
        }

        private void UpdateAverageTemp()
        {
            int myCell = Grid.PosToCell(this);
            var room = rooms.First(x => x.Key == currentRoomId);

            if (room.Value.First() == myCell)
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
            if (string.IsNullOrEmpty(currentRoomId))
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
    }
}