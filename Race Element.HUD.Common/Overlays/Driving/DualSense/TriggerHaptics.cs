using RaceElement.Data.Common;
using RaceElement.Util.SystemExtensions;
using static RaceElement.Util.SystemExtensions.FloatExtensions;
using static RaceElement.HUD.Common.Overlays.Driving.DualSense.DS5W;
using static RaceElement.HUD.Common.Overlays.Driving.DualSense.Util;
using static RaceElement.HUD.Common.Overlays.Driving.DualSense.DualSenseConfiguration;

namespace RaceElement.HUD.Common.Overlays.Driving.DualSense;

internal static class TriggerHaptics
{
    public static void HandleRumble(RumbleParams config)
    {
        float kerb = Math.Max(0.0f, SimDataProvider.LocalCar.Physics.KerbVibration) * config.KerbCoef;
        float abs = Math.Abs(SimDataProvider.LocalCar.Physics.AbsVibrations) * config.ABSCoef;
        int lRumble = (int)Math.Round(Clip(kerb * 255.0f, 0.0f, 255.0f));
        int rRumble = (int)Math.Round(Clip(abs * 255.0f, 0.0f, 255.0f));
        //DebugOut("kerb: " + SimDataProvider.LocalCar.Physics.KerbVibration + ", lRumble: " + lRumble);
        //DebugOut("abs: " + SimDataProvider.LocalCar.Physics.AbsVibrations + ", rRumble: " + rRumble);
        ds5w_set_rumble(1, lRumble);
        ds5w_set_rumble(0, rRumble);
    }
    public static void HandleAcceleration(ThrottleSlipHaptics config)
    {
        float[] slipRatios = SimDataProvider.LocalCar.Tyres.SlipRatio;
        if (SimDataProvider.LocalCar.Inputs.Throttle <= config.ThrottleThreshold / 100f ||
            slipRatios.Length != 4)
        {
            ds5w_set_trigger_effect_off(0);
            return;
        }
        //DebugOut("Slip Ratios: " + slipRatios[0] + ", " + slipRatios[1] + ", " + slipRatios[2] + ", " + slipRatios[3]);
        float slipRatioFront = 0.5f * (slipRatios[0] + slipRatios[1]);
        if (slipRatioFront < config.MinSlipRatio)
        {
            ds5w_set_trigger_effect_off(0);
            return;
        }
        // Calculate frequency from slipRatio
        float srWeight = SmoothStep(slipRatioFront, config.MinSlipRatio, config.MaxSlipRatio);
        float freqf = Lerp(srWeight, config.FreqAtMinSlipRatio, config.FreqAtMaxSlipRatio);
        int freq = (int)Math.Round(Clip(freqf, 0.0f, 255.0f));
        // Calculate amplitude from slipRatio
        int amp = 1 + (int)Math.Round(Math.Sqrt(slipRatioFront - config.MinSlipRatio ) * config.AmpGain);
        amp.ClipMax(8);
        DebugOut("slipRatioFront: " + slipRatioFront + ", freq: " + freq + ", amp: " + amp);
        ds5w_set_trigger_effect_vibration(0, 0, amp, freq);
    }
    public static void HandleBraking(BrakeSlipHaptics config)
    {
        float[] slipRatios = SimDataProvider.LocalCar.Tyres.SlipRatio;
        if (SimDataProvider.LocalCar.Inputs.Brake <= config.BrakeThreshold / 100f ||
            slipRatios.Length != 4)
        {
            ds5w_set_trigger_effect_off(1);
            return;
        }
        //DebugOut("Slip Ratios: " + slipRatios[0] + ", " + slipRatios[1] + ", " + slipRatios[2] + ", " + slipRatios[3]);
        float slipRatioBack = 0.5f * (slipRatios[2] + slipRatios[3]);
        if (slipRatioBack < config.MinSlipRatio)
        {
            ds5w_set_trigger_effect_off(1);
            return;
        }
        // Calculate frequency from slipRatio
        float srWeight = SmoothStep(slipRatioBack, config.MinSlipRatio, config.MaxSlipRatio);
        float freqf = Lerp(srWeight, config.FreqAtMinSlipRatio, config.FreqAtMaxSlipRatio);
        int freq = (int)Math.Round(Clip(freqf, 0.0f, 255.0f));
        // Calculate amplitude from slipRatio
        int amp = 1 + (int)Math.Round(Math.Sqrt(slipRatioBack - config.MinSlipRatio) * config.AmpGain);
        amp.ClipMax(8);
        DebugOut("slipRatioBack: " + slipRatioBack + ", freq: " + freq + ", amp: " + amp);
        ds5w_set_trigger_effect_vibration(1, 0, amp, freq);
    }
}
