using KSerialization;
using STRINGS;

namespace TooMuchLogic.Logic
{
    public class TmlConduitMassSensor : ConduitThresholdSensor, IThresholdSwitch
    {
        private const float rangeMin = 0.0f;
        internal float rangeMax;

        [Serialize] private float lastMeassuredMass;

        /// <inheritdoc />
        public float GetRangeMinInputField()
        {
            return rangeMin;
        }

        /// <inheritdoc />
        public float GetRangeMaxInputField()
        {
            return rangeMax;
        }

        /// <inheritdoc />
        public override void OnSpawn()
        {
            base.OnSpawn();
            switch (conduitType)
            {
                case ConduitType.Gas:
                case ConduitType.Liquid:
                {
                    ConduitFlow manager = Conduit.GetFlowManager(conduitType);
                    rangeMax = manager.MaxMass;
                    break;
                }
                case ConduitType.Solid:
                {
                    rangeMax = 1f;
                    break;
                }
            }
        }

        /// <inheritdoc />
        public LocString ThresholdValueUnits()
        {
            return STRINGS.MODDED_UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.MASS_UNITS;
        }

        /// <inheritdoc />
        public string Format(float value, bool units)
        {
            return GameUtil.GetFormattedMass(value);
        }

        /// <inheritdoc />
        public float ProcessedSliderValue(float input)
        {
            return input;
        }

        /// <inheritdoc />
        public float ProcessedInputValue(float input)
        {
            return input;
        }

        /// <inheritdoc />
        public override float CurrentValue
        {
            get
            {
                lastMeassuredMass = GetContainedMass();
                return lastMeassuredMass;
            }
        }

        /// <inheritdoc />
        public float RangeMin => rangeMin;

        /// <inheritdoc />
        public float RangeMax => rangeMax;

        /// <inheritdoc />
        public LocString Title => STRINGS.MODDED_UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.MASS_TITLE;

        /// <inheritdoc />
        public LocString ThresholdValueName => STRINGS.MODDED_UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.CONTENT_MASS;

        /// <inheritdoc />
        public string AboveToolTip =>
            (string) STRINGS.MODDED_UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.MASS_TOOLTIP_ABOVE;

        /// <inheritdoc />
        public string BelowToolTip =>
            (string) STRINGS.MODDED_UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.MASS_TOOLTIP_BELOW;

        /// <inheritdoc />
        public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

        /// <inheritdoc />
        public int IncrementScale => 1;

        /// <inheritdoc />
        public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(RangeMax);
    }
}