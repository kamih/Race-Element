using RaceElement.Data.Common;
using RaceElement.Util.SystemExtensions;
using static RaceElement.HUD.Common.Overlays.Driving.DualSense.Resources;

namespace RaceElement.HUD.Common.Overlays.Driving.DualSense;

internal static class TriggerHaptics
{
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
            if (percentage >= 0.05f)
                ds5w_set_trigger_effect_vibration(1, 0, (int)(config.ThrottleSlip.FeedbackStrength * percentage), 0);

            int freq = (int)(config.BrakeSlip.MaxFrequency * percentage);
            freq.ClipMin(config.BrakeSlip.MinFrequency);
            ds5w_set_trigger_effect_vibration(1, 0, config.BrakeSlip.Amplitude, freq);
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
            if (percentage >= 0.05f)
                ds5w_set_trigger_effect_vibration(0, 0, (int)(config.ThrottleSlip.FeedbackStrength * percentage), 0);

            int freq = (int)(config.ThrottleSlip.MaxFrequency * percentage);
            freq.ClipMin(config.ThrottleSlip.MinFrequency);
            ds5w_set_trigger_effect_vibration(0, 0, config.ThrottleSlip.Amplitude, freq);
        }
        else
        {
            ds5w_set_trigger_effect_off(0);
        }
    }
}
