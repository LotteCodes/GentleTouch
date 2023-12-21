using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace GentleTouch.Windows;

public class ConfigWindow : Window, IDisposable {

    private Configuration Configuration;

    public ConfigWindow(GentleTouch plugin) : base(
        "Gentle Touch Configurations",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
        ImGuiWindowFlags.NoScrollWithMouse) {

        this.Size = new Vector2(232, 300);
        this.SizeCondition = ImGuiCond.Always;

        this.Configuration = plugin.Configuration;
    }

    public void Dispose() { }


    public override void Draw() {
        // can't ref a property, so use a local copy
        if (ImGui.CollapsingHeader("Communication Managers")) {
            var ble = this.Configuration.BluetoothLEManager;
            if (ImGui.Checkbox("Bluetooth LE", ref ble)) {
                this.Configuration.BluetoothLEManager = ble;
                this.Configuration.Save();
            }
            ImGui.SetTooltip("General Bluetooth or Bluetooth dongle.");
            var serial = this.Configuration.SerialPortManager;
            if (ImGui.Checkbox("Serial Port", ref serial)) {
                this.Configuration.SerialPortManager = serial;
                this.Configuration.Save();
            }
            ImGui.SetTooltip("General Bluetooth or Bluetooth dongle.");
            var hid = this.Configuration.HIDManager;
            if (ImGui.Checkbox("HID", ref hid)) {
                this.Configuration.HIDManager = hid;
                this.Configuration.Save();
            }
            var hidlov = this.Configuration.HIDLovenseDongleManager;
            if (ImGui.Checkbox("HID Lovense Dongle", ref hidlov)) {
                this.Configuration.HIDLovenseDongleManager = hidlov;
                this.Configuration.Save();
            }
            ImGui.SetTooltip("Lovense official Bluetooth dongle. They dont work like normal dongles.");
            var lovc = this.Configuration.LovenseConnectManager;
            if (ImGui.Checkbox("Lovense Connect", ref lovc)) {
                this.Configuration.LovenseConnectManager = lovc;
                this.Configuration.Save();
            }
            ImGui.SetTooltip("Lovense Connect app");
            var xin = this.Configuration.XInputManager;
            if (ImGui.Checkbox("XInput", ref xin)) {
                this.Configuration.XInputManager = xin;
                this.Configuration.Save();
            }
            ImGui.SetTooltip("No idea! Some ancient stuff but since it is supported ill add it.");
        }
    }
}
