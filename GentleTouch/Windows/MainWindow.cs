using System;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Windowing;
using ImGuiNET;
namespace GentleTouch.Windows;

public class MainWindow : Window, IDisposable
{
    private IDalamudTextureWrap LushImage;
    private GentleTouch Plugin;

    public MainWindow(GentleTouch plugin, IDalamudTextureWrap lushImage) : base(
        "Gentle Touch", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        this.SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        this.LushImage = lushImage;
        this.Plugin = plugin;
    }

    public void Dispose()
    {
        this.LushImage.Dispose();
    }

    public override void Draw()
    {
        if (ImGui.Button("Show Settings")) {
            this.Plugin.DrawConfigUI();
        }

        ImGui.Text("Server Status: ");
        ImGui.SameLine();
        if (Connection.getInstance().client != null) {
            if (Connection.getInstance().IsServerRunning() ||
                // Lost ownership over the server due some plugin crashed us, but its still running indipendent.
                // If the client can connect to the server we know its still there or the user allready runs the Intiface Central Server or a similair service
                Connection.getInstance().client.Connected
                ) {
                ImGui.Text("Running");
                ImGui.Text($"Port: {Connection.getInstance().GetPort()}");

                ImGui.Spacing();

                float scale = ImGui.GetTextLineHeight() / ((float)this.LushImage.Height);
                if (ImGui.BeginTable("devices", 3, ImGuiTableFlags.RowBg)) {
                    for (int d = 0; d < Connection.getInstance().client.Devices.Length; d++) {
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Image(this.LushImage.ImGuiHandle, new Vector2(this.LushImage.Width * scale, this.LushImage.Height * scale));
                        ImGui.TableSetColumnIndex(1);
                        ImGui.Text(Connection.getInstance().client.Devices[d].Name);
                        ImGui.TableSetColumnIndex(2);
                        if (ImGui.Button("Test Vibrate 5s"))
                            Connection.getInstance().Vibrate(5000);
                    }
                    ImGui.EndTable();
                }
            } else {
                ImGui.Text("Offline");
                if (Connection.getInstance().ServerStartCD) {
                    ImGui.Text("Starting server... please wait.");
                } else {
                    if (ImGui.Button("Connect"))
                        Connection.getInstance().CreateConnection(); // Async on purpose
                }
            }
        }

        if (ImGui.CollapsingHeader("Debug Output")) {
            string str = Connection.getInstance().sb.ToString();
            ImGui.InputTextMultiline("##source", ref str, (uint)str.Length, new Vector2(ImGui.GetContentRegionMax().X, ImGui.GetTextLineHeight() * 16), ImGuiInputTextFlags.ReadOnly);
        }
    }
}
