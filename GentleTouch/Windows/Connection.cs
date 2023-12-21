using System;
using System.Threading.Tasks;
using Buttplug.Client;
using Buttplug.Core;
using Buttplug.Client.Connectors.WebsocketConnector;
using System.Text;
using System.Diagnostics;
using System.IO;
using Dalamud.Plugin.Services;
using Dalamud.Game.Gui.Dtr;

namespace GentleTouch.Windows {
    internal class Connection : IDisposable {
        public StringBuilder sb = new StringBuilder();
        private static Connection? sharedInstance = null;
        public ButtplugClient? client = null;
        private Process? ServerProcess = null;
        private Configuration? Configuration;
        private string? PluginDirectoryAbsolute;
        private DeviceActions deviceAction;
        public bool ServerStartCD;
        private MainWindow? MainWindow;
        private DtrBarEntry? dtrEntry = null;
        private bool KeepRunning = true;
        private static readonly string serverIP = "localhost";

        public void OpenMainWindow() {
            MainWindow.IsOpen = true;
        }

        public struct DeviceActions {
            public bool batteryUpdated;
            public int batteryStatus;
            public int vibrateTime;
        }

        public bool Init(Configuration configs,string dir,MainWindow mw) {


            deviceAction.batteryStatus = -1;

            MainWindow = mw;
            PluginDirectoryAbsolute = dir;
            Configuration = configs;

            client = new ButtplugClient("FFXIV Gentle Touch");
            client.DeviceAdded += OnDeviceAdded;
            client.DeviceRemoved += OnDeviceRemoved;

            StartServer();

            BatteryCheckup(); // Async

            dtrEntry = Service.DtrBar.Get("Gentle Touch");
            dtrEntry.OnClick = OpenMainWindow;

            Service.Framework.Update += OnFrameworkUpdate;

            return true;
        }
        public void OnFrameworkUpdate(IFramework framework) {
            try {
                if (client != null && client.Connected) {
                    if (deviceAction.batteryStatus == -1 ||  // Doesnt have battery support
                        client.Devices.Length == 0) {
                        dtrEntry.Shown = false;
                    } else {
                        if (deviceAction.batteryUpdated) {
                            dtrEntry.Text = $"♥ {deviceAction.batteryStatus}%";
                            dtrEntry.Tooltip = client.Devices[0].Name;
                            dtrEntry.Shown = true;
                            deviceAction.batteryUpdated = false;
                        }
                    }
                }
            } catch (Exception ex) {
                Service.PluginLog.Error(ex.Message);
            }
        }

        public int GetPort() {
            return Configuration.ServerPort;
        }

        public async void StartServer() {
            ServerStartCD = true;
            if (client != null)
                client.DisconnectAsync().Wait();

            if (ServerProcess != null) {
                ServerProcess.Kill();
                ServerProcess.Dispose();
            }

            deviceAction.vibrateTime = 0;

            if (Configuration.UseEmbeeded) {
                StringBuilder serverConfig = new StringBuilder();
                serverConfig.Append($"--websocket-port {Configuration.ServerPort}");
                if (Configuration.BluetoothLEManager)
                    serverConfig.Append(" --use-bluetooth-le");
                if (Configuration.HIDManager)
                    serverConfig.Append(" --use-hid");
                if (Configuration.LovenseConnectManager)
                    serverConfig.Append(" --use-lovense-connect");
                if (Configuration.HIDLovenseDongleManager)
                    serverConfig.Append(" --use-lovense-dongle");
                if (Configuration.SerialPortManager)
                    serverConfig.Append(" --use-serial");
                if (Configuration.XInputManager)
                    serverConfig.Append(" --use-xinput");

                ProcessStartInfo ServerProcessInfo = new ProcessStartInfo(
                    Path.Combine(PluginDirectoryAbsolute, "intiface_engine.exe"),
                    serverConfig.ToString());
                ServerProcessInfo.CreateNoWindow = true;
                ServerProcessInfo.UseShellExecute = true;
                ServerProcessInfo.WindowStyle = ProcessWindowStyle.Hidden;

                ServerProcess = Process.Start(ServerProcessInfo);
            }

            CreateConnection();

            await Task.Delay(3000);
            ServerStartCD = false;
        }

        public bool IsServerRunning() {
            return ServerProcess != null && !(ServerProcess.HasExited);
        }


        public void Dispose() {

            Service.Framework.Update -= OnFrameworkUpdate;

            KeepRunning = false;

            client?.DisconnectAsync();
            client = null!;

            sb?.Clear();
            sb = null!;

            dtrEntry?.Dispose();
            dtrEntry = null!;

            if (ServerProcess != null) {
                ServerProcess.Kill();
                ServerProcess.Dispose();
                ServerProcess = null!;
            }

            GC.SuppressFinalize(this);
        }

        public static Connection getInstance() {
            if (sharedInstance == null)
                sharedInstance = new Connection();
            return sharedInstance;
        }

        public static void Close() {
            if (sharedInstance != null) {
                sharedInstance.Dispose();
                sharedInstance = null!;
            }
        }

        public void Vibrate(int ms) {
            if (client?.Devices.Length > 0) {
                var device = client.Devices[0];
                var duration = ms;
                if (device != null)
                    Vibrate(duration, device); // Async
            }
        }

        private async Task Vibrate(int duration, ButtplugClientDevice device) {
            if (deviceAction.vibrateTime > 0) {
                deviceAction.vibrateTime += duration;
            } else {
                deviceAction.vibrateTime = duration;
                try {
                    await device.VibrateAsync(0.5);
                    while (deviceAction.vibrateTime > 0) {
                        await Task.Delay(100);
                        deviceAction.vibrateTime -= 100;
                    }
                    await device.VibrateAsync(0);
                    deviceAction.vibrateTime = 0;
                } catch (Exception e) {
                    sb.Append($"Problem vibrating: {e}\n");
                }
            }
        }

        private async Task ResetVibration(ButtplugClientDevice device) {
            deviceAction.vibrateTime = 0;
            try {
                await device.VibrateAsync(0);
            } catch (Exception e) {
                sb.Append($"Problem vibrating: {e}\n");
            }
        }

        private async Task BatteryCheckup() {
            while (KeepRunning) { 
                if(client != null && 
                    client.Connected && 
                    client.Devices.Length > 0) {
                    try {
                        if (client.Devices[0].HasBattery)
                        {
                            deviceAction.batteryStatus = (int)((await client.Devices[0].BatteryAsync()) * 100);
                            deviceAction.batteryUpdated = true;
                        }
                        else
                            deviceAction.batteryStatus = -1; // Doesnt support battery
                    }
                    catch (Exception e) {
                        sb.Append($"Problem checking battery: {e}\n");
                        deviceAction.batteryStatus = -1; // Turn it off then...
                    }
                }
                await Task.Delay(10000); // Wait 10 sec before checking again
            }
        }

        void OnDeviceAdded(object? o, DeviceAddedEventArgs args) {
           sb.Append($"Device ${args.Device.Name} connected\n");
            /*
            if (deviceAction.vibrateTime <= 0) // Might have been disconnected and is back online now but the timer ran out while it was disconnected...
                ResetVibration(args.Device);
            */
            Vibrate(5000,args.Device);
        }

        void OnDeviceRemoved(object? o, DeviceRemovedEventArgs args) {
            sb.Append($"Device ${args.Device.Name} disconnected\n");
        }

        public async Task CreateConnection() {
            sb.Append($"Attempt to connect ws://{serverIP}:{Configuration.ServerPort}\n");
            ButtplugWebsocketConnector connector = new ButtplugWebsocketConnector(new Uri($"ws://{serverIP}:{Configuration.ServerPort}/"));
            try {
                await client.ConnectAsync(connector);
            } catch (ButtplugClientConnectorException ex) {
                sb.Append(
                    "Can't connect to Buttplug Server, exiting!\n" +
                    $"Message: {ex.InnerException.Message}\n");
            } catch (ButtplugHandshakeException ex) {
                sb.Append(
                    "Failed Handshake with Buttplug Server, exiting!\n" +
                    $"Message: {ex.InnerException.Message}\n");
            }
            var startScanningTask = client.StartScanningAsync();
            try {
                await startScanningTask;
            } catch (ButtplugException ex) {
                sb.Append(
                    $"Scanning failed:\n\t{ex.InnerException.Message}\n");
            }
        }
    }
}
