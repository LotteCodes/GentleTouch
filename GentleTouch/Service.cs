using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace GentleTouch
{
    public class Service
    {
        [PluginService] public static IFramework Framework { get; private set; } = null!;
        [PluginService] public static IPluginLog PluginLog { get; private set; } = null!;
        [PluginService] public static IDtrBar DtrBar { get; private set; } = null!;
    }
}
