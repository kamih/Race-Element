using RaceElement.Data.Common;
using RaceElement.Util.SystemExtensions;
using static RaceElement.Util.SystemExtensions.FloatExtensions;
using static RaceElement.HUD.Common.Overlays.Driving.DualSense.DS5W;
using static RaceElement.HUD.Common.Overlays.Driving.DualSense.Util;
using static RaceElement.HUD.Common.Overlays.Driving.DualSense.DualSenseConfiguration;

namespace RaceElement.HUD.Common.Overlays.Driving.DualSense;

internal static class TriggerHaptics
{
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
#if false
    public static void HandleBraking(DualSenseConfiguration config)
    {
        // TODO: add either an option to threshold it on brake input or based on some curve?
        if (SimDataProvider.LocalCar.Inputs.Brake <= config.BrakeSlip.BrakeThreshold / 100f)
        {
            ds5w_set_trigger_effect_off(1);
            return;
        }
        float[] slipRatios = SimDataProvider.LocalCar.Tyres.SlipRatio;
        if (slipRatios.Length != 4)
        {
            ds5w_set_trigger_effect_off(1);
            return;
        }

        float slipRatioFront = Math.Max(slipRatios[0], slipRatios[1]);
        float slipRatioRear = Math.Max(slipRatios[2], slipRatios[3]);

        // TODO: add option for front and rear ratio threshold.
        if (slipRatioFront > config.BrakeSlip.FrontSlipThreshold || slipRatioRear > config.BrakeSlip.RearSlipThreshold)
        {
            float frontslipCoefecient = slipRatioFront * 4f;
            frontslipCoefecient.ClipMax(10);

            float rearSlipCoefecient = slipRatioFront * 2f;
            rearSlipCoefecient.ClipMax(7.5f);

            float magicValue = frontslipCoefecient + rearSlipCoefecient;
            float percentage = magicValue * 1.0f / 17.5f;
            int strength = (int)(config.BrakeSlip.FeedbackStrength * percentage);
            if (strength >= 1) {
                ds5w_set_trigger_effect_feedback(1, 0, strength);
                //ds5w_set_trigger_effect_off(1);
            }

            /*int freq = (int)(config.BrakeSlip.MaxFrequency * percentage);
            freq.ClipMin(config.BrakeSlip.MinFrequency);
            ds5w_set_trigger_effect_vibration(1, 0, config.BrakeSlip.Amplitude, freq);*/
        }
        else
        {
            ds5w_set_trigger_effect_off(1);
        }
    }
    public static void HandleAcceleration(DualSenseConfiguration config)
    {
        if (SimDataProvider.LocalCar.Inputs.Throttle <= config.ThrottleSlip.ThrottleThreshold / 100f)
        {
            ds5w_set_trigger_effect_off(0);
            return;
        }
        float[] slipRatios = SimDataProvider.LocalCar.Tyres.SlipRatio;
        if (slipRatios.Length != 4)
        {
            ds5w_set_trigger_effect_off(0);
            return;
        }

        float slipRatioFront = Math.Max(slipRatios[0], slipRatios[1]);
        float slipRatioRear = Math.Max(slipRatios[2], slipRatios[3]);

        if (slipRatioFront > config.ThrottleSlip.FrontSlipThreshold || slipRatioRear > config.ThrottleSlip.RearSlipThreshold)
        {
            float frontslipCoefecient = slipRatioFront * 3f;
            frontslipCoefecient.ClipMax(5);
            float rearSlipCoefecient = slipRatioFront * 5f;
            rearSlipCoefecient.ClipMax(7.5f);

            float magicValue = frontslipCoefecient + rearSlipCoefecient;
            float percentage = magicValue * 1.0f / 12.5f;
            /*if (percentage >= 0.05f)
            {
                ds5w_set_trigger_effect_feedback(0, 0, (int)(config.ThrottleSlip.FeedbackStrength * percentage));
                ds5w_set_trigger_effect_off(0);
            }*/

            int freq = (int)(config.ThrottleSlip.MaxFrequency * percentage);
            freq.ClipMin(config.ThrottleSlip.MinFrequency);
            ds5w_set_trigger_effect_vibration(0, 0, config.ThrottleSlip.Amplitude, freq);
        }
        else
        {
            ds5w_set_trigger_effect_off(0);
        }
    }
#endif
}
