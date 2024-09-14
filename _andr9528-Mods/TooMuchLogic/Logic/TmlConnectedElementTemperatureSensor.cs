using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using KSerialization;
using Microsoft.Build.Utilities;
using STRINGS;
using TooMuchLogic.Services;
using UnityEngine;
using UtilLibs;
using static STRINGS.MISC.STATUSITEMS;
using static STRINGS.UI;
using CODEX = STRINGS.CODEX;

namespace TooMuchLogic.Logic
{
    public class TmlConnectedElementTemperatureSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim4000ms,
        ISingleSliderControl
    {
        private static Dictionary<int, TmlConnectedElementTemperatureSensor> knownSensors = new();
        private static Dictionary<string, HashSet<int>> rooms = new();

        private int myCell;
        private ConjoinedSensorLogicService<TmlConnectedElementTemperatureSensor> conjoinedSensorLogicService;
        public float minTemp;
        public float maxTemp = 373.15f;
        private bool wasOn;
        private HandleVector<int>.Handle structureTemperature;

        [Serialize] [SerializeField] public float thresholdTemperature = 280f;
        [Serialize] [SerializeField] public int radius = 0;
        [Serialize] [SerializeField] public bool activateOnWarmerThan;
        [Serialize] [SerializeField] public float averageTemp;

        [MyCmpAdd] private CopyBuildingSettings copyBuildingSettings;
        [MyCmpGet] private LogicPorts logicPorts;

        private static readonly EventSystem.IntraObjectHandler<TmlConnectedElementTemperatureSensor>
            OnCopySettingsDelegate = new((component, data) => component.OnCopySettings(data));

        private void OnCopySettings(object data)
        {
            var component = ((GameObject) data).GetComponent<TmlConnectedElementTemperatureSensor>();
            if (!(component != null))
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
            conjoinedSensorLogicService.RemoveFromRoom();
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            myCell = Grid.PosToCell(this);
            conjoinedSensorLogicService =
                new ConjoinedSensorLogicService<TmlConnectedElementTemperatureSensor>(radius, ref rooms,
                    ref knownSensors, myCell);

            knownSensors.Add(myCell, this);

            structureTemperature = GameComps.StructureTemperatures.GetHandle(gameObject);
            OnToggle += new Action<bool>(OnSwitchToggled);
            UpdateVisualState(true);
            UpdateLogicCircuit();
            wasOn = switchedOn;
            conjoinedSensorLogicService.CollectConnectedElementCells(UpdateAverageTemp, SetAverageTempFromOtherSensor);
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

        private bool SetAverageTempFromOtherSensor()
        {
            if (string.IsNullOrEmpty(conjoinedSensorLogicService.currentRoomId))
                return false;

            int otherSensorCell = rooms[conjoinedSensorLogicService.currentRoomId].First();
            if (otherSensorCell == myCell)
                return false;

            TmlConnectedElementTemperatureSensor sensor = knownSensors[otherSensorCell];
            averageTemp = sensor.averageTemp;

            return true;
        }

        private void UpdateAverageTemp()
        {
            var room = rooms.FirstOrDefault(x => x.Key == conjoinedSensorLogicService.currentRoomId);
            //SgtLogger.log(
            //    $"Found following room ({room})(Temp), when looking for room with following id ({conjoinedSensorLogicService.currentRoomId})");

            if (radius != 0 || room.Value.First() == myCell)
            {
                float averageRoomTemperature = conjoinedSensorLogicService.roomCells.Where(Grid.IsValidCell)
                    .Sum(cell => Grid.Temperature[cell]);
                averageRoomTemperature /= conjoinedSensorLogicService.roomCells.Count;
                averageTemp = averageRoomTemperature;
                return;
            }

            SetAverageTempFromOtherSensor();
        }

        /// <inheritdoc />
        public void Sim4000ms(float dt)
        {
            if (conjoinedSensorLogicService.ShouldCellsBeRecollected())
                conjoinedSensorLogicService.CollectConnectedElementCells(UpdateAverageTemp,
                    SetAverageTempFromOtherSensor);
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
            conjoinedSensorLogicService.radius = radius;
            if (radius != 0)
                conjoinedSensorLogicService.RemoveFromRoom();

            conjoinedSensorLogicService.CollectConnectedElementCells(UpdateAverageTemp, SetAverageTempFromOtherSensor);
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