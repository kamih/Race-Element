using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RaceElement.HUD.Common.Overlays.Driving.DualSense;

internal static class Resources
{
    /*
        DS5W_CAPI int ds5w_init(void);
        DS5W_CAPI void ds5w_shutdown(void);
        DS5W_CAPI int ds5w_set_trigger_effect_off(int left);
        DS5W_CAPI int ds5w_set_trigger_effect_vibration(int left, int pos, int amp, int freq);
    */
    [DllImport("ds5w_x64.dll", SetLastError = true)]
    public static extern int ds5w_init();

    [DllImport("ds5w_x64.dll", SetLastError = true)]
    public static extern void ds5w_shutdown();

    [DllImport("ds5w_x64.dll", SetLastError = true)]
    public static extern int ds5w_set_trigger_effect_off(int left);

    [DllImport("ds5w_x64.dll", SetLastError = true)]
    public static extern int ds5w_set_trigger_effect_vibration(int left, int pos, int amp, int freq);
}
