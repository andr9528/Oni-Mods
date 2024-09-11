using JetBrains.Annotations;
using KSerialization;
using static STRINGS.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using TooMuchLogic.Services;
using UnityEngine;

namespace TooMuchLogic
{
    public class TmlConnectedElementMassSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim4000ms,
        ISingleSliderControl
    {
        private static Dictionary<int, TmlConnectedElementMassSensor> knownSensors = new();
        private static Dictionary<string, HashSet<int>> rooms = new();

        [CanBeNull] private string currentRoomId;
        private bool buildingRoom = false;
        [Serialize] [SerializeField] public float thresholdMass = 20f;
        [Serialize] [SerializeField] public int radius = 0;
        [Serialize] [SerializeField] public bool activateAboveThreshold;
        public float minMass = 0;
        public float maxMass = int.MaxValue;
        [Serialize] [SerializeField] public float totalMass;
        private bool wasOn;
        public HashSet<int> roomCells;
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

            UpdateTotalMass();
        }

        private bool EarlyBreak(int source, int cell)
        {
            return radius == 0 && knownSensors.ContainsKey(cell) && UpdateMassFromOtherSensor(source, cell);
        }

        private bool UpdateMassFromOtherSensor(int source, int cell)
        {
            if (source == cell)
                return false;

            var existingRoom = rooms.FirstOrDefault(x => x.Value.Contains(cell));
            if (existingRoom.Equals(default(KeyValuePair<string, HashSet<int>>)))
                return false;

            existingRoom.Value.Add(source);
            currentRoomId = existingRoom.Key;

            return SetTotalMassFromOtherSensor();
        }

        private bool SetTotalMassFromOtherSensor()
        {
            if (string.IsNullOrEmpty(currentRoomId))
                return false;

            int otherSensorCell = rooms[currentRoomId].First();
            int myCell = Grid.PosToCell(this);
            if (otherSensorCell == myCell)
                return false;

            TmlConnectedElementMassSensor sensor = knownSensors[otherSensorCell];
            totalMass = sensor.totalMass;

            return true;
        }

        private void UpdateTotalMass()
        {
            int myCell = Grid.PosToCell(this);
            var room = rooms.FirstOrDefault(x => x.Key == currentRoomId);

            if (radius != 0 || room.Value.First() == myCell)
            {
                totalMass = roomCells.Where(Grid.IsValidCell).Sum(cell => Grid.Mass[cell]);
                return;
            }

            SetTotalMassFromOtherSensor();
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
            if (radius != 0)
                RemoveFromRoom();
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