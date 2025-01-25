using System.Runtime.InteropServices;

namespace RaceElement.HUD.Common.Overlays.Driving.DualSense;

internal static partial class Resources
{
    /*
        DS5W_CAPI int ds5w_init(void);
        DS5W_CAPI void ds5w_shutdown(void);
        DS5W_CAPI int ds5w_set_trigger_effect_off(int left);
        DS5W_CAPI int ds5w_set_trigger_effect_vibration(int left, int pos, int amp, int freq);
    */
    [LibraryImport("ds5w_x64.dll", SetLastError = true)]
    public static partial int ds5w_init();

    [LibraryImport("ds5w_x64.dll", SetLastError = true)]
    public static partial void ds5w_shutdown();

    [LibraryImport("ds5w_x64.dll", SetLastError = true)]
    public static partial int ds5w_set_trigger_effect_off(int left);

    [LibraryImport("ds5w_x64.dll", SetLastError = true)]
    public static partial int ds5w_set_trigger_effect_vibration(int left, int pos, int amp, int freq);
}
