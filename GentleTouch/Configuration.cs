using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace GentleTouch
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

        public bool UseEmbeeded { get; set; } = true;
        public int ServerPort { get; set; } = 12345;
        public bool BluetoothLEManager { get; set; } = true;
        public bool SerialPortManager { get; set; } = false;
        public bool HIDManager { get; set; } = false;
        public bool HIDLovenseDongleManager { get; set; } = false;
        public bool XInputManager { get; set; } = false;
        public bool LovenseConnectManager { get; set; } = false;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.PluginInterface!.SavePluginConfig(this);
        }
    }
}
