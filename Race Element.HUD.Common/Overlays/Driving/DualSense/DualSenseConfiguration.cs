using RaceElement.HUD.Overlay.Configuration;

namespace RaceElement.HUD.Common.Overlays.Driving.DualSense;

internal sealed class DualSenseConfiguration : OverlayConfiguration
{
    public DualSenseConfiguration()
    {
        GenericConfiguration.AlwaysOnTop = false;
        GenericConfiguration.Window = false;
        GenericConfiguration.Opacity = 1.0f;
        GenericConfiguration.AllowRescale = false;
    }

    [HideForGame(Game.RaceRoom | Game.AssettoCorsa1)]
    [ConfigGrouping("Rumble", "Adjust the rumble effects.")]
    public RumbleParams Rumble { get; init; } = new();
    public sealed class RumbleParams
    {
        /// <summary>
        /// Kerb vibration coef
        /// </summary>
        [ToolTip("Kerb vibration coef.")]
        [FloatRange(0.0f, 2.0f, 0.01f, 2)]
        public float KerbCoef { get; init; } = 1.2f;

        /// <summary>
        /// ABS vibration coef
        /// </summary>
        [ToolTip("ABS vibration coef.")]
        [FloatRange(0.0f, 2.0f, 0.01f, 2)]
        public float ABSCoef { get; init; } = 1f;
    }

    [ConfigGrouping("Brake Slip", "Adjust the slip effect whilst applying the brakes.")]
    public BrakeSlipHaptics BrakeSlip { get; init; } = new();
    public sealed class BrakeSlipHaptics
    {
        /// <summary>
        /// The brake in percentage (divide by 100f if you want 0-1 value)
        /// </summary>
        [ToolTip("The minimum brake percentage before any effects are applied. See this like a deadzone.")]
        [FloatRange(0.1f, 99f, 0.1f, 1)]
        public float BrakeThreshold { get; init; } = 3f;

        [FloatRange(0.0f, 1.0f, 0.002f, 3)]
        public float MinSlipRatio { get; init; } = 0.240f;

        [FloatRange(1.0f, 10.0f, 0.002f, 3)]
        public float MaxSlipRatio { get; init; } = 2.0f;

        [ToolTip("Sets the frequency of the trigger vibration effect at the MinSlipRatio value.")]
        [FloatRange(10.0f, 255.0f, 0.5f, 2)]
        public float FreqAtMinSlipRatio { get; init; } = 150.0f;

        [ToolTip("Sets the frequency of the trigger vibration effect at the MaxSlipRatio value.")]
        [FloatRange(10.0f, 255.0f, 0.5f, 2)]
        public float FreqAtMaxSlipRatio { get; init; } = 20.0f;

        [ToolTip("Gain for the effect amplitude as SlipRatio increases.")]
        [FloatRange(1.0f, 10.0f, 0.1f, 1)]
        public float AmpGain { get; init; } = 2.8f;
    }

    [ConfigGrouping("Throttle Slip", "Adjust the slip effect whilst applying the throttle.\nModify the threshold to increase or decrease sensitivity in different situations.")]
    public ThrottleSlipHaptics ThrottleSlip { get; init; } = new();
    public sealed class ThrottleSlipHaptics
    {
        /// <summary>
        /// The throttle in percentage (divide by 100f if you want 0-1 value)
        /// </summary>
        [ToolTip("The minimum throttle percentage before any effects are applied. See this like a deadzone.")]
        [FloatRange(0.1f, 99f, 0.1f, 1)]
        public float ThrottleThreshold { get; init; } = 3f;

        [FloatRange(0.0f, 1.0f, 0.002f, 3)]
        public float MinSlipRatio { get; init; } = 0.5f;

        [FloatRange(1.0f, 10.0f, 0.002f, 3)]
        public float MaxSlipRatio { get; init; } = 2.5f;

        [ToolTip("Sets the frequency of the trigger vibration effect at the MinSlipRatio value.")]
        [FloatRange(10.0f, 255.0f, 0.5f, 2)]
        public float FreqAtMinSlipRatio { get; init; } = 165.0f;

        [ToolTip("Sets the frequency of the trigger vibration effect at the MaxSlipRatio value.")]
        [FloatRange(10.0f, 255.0f, 0.5f, 2)]
        public float FreqAtMaxSlipRatio { get; init; } = 20.0f;

        [ToolTip("Gain for the effect amplitude as SlipRatio increases.")]
        [FloatRange(1.0f, 10.0f, 0.1f, 1)]
        public float AmpGain { get; init; } = 2.5f;
    }

}
