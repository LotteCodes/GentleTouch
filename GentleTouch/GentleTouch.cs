using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using GentleTouch.Windows;
using GentleTouch.Parser;

namespace GentleTouch
{
    public sealed class GentleTouch : IDalamudPlugin
    {
        public string Name => "Gentle Touch";
        private const string CommandName = "/gentle";
        private const string ConfigCommandName = "/gentlec";

        private DalamudPluginInterface PluginInterface { get; init; }
        private ICommandManager CommandManager { get; init; }
        public Configuration Configuration { get; init; }
        public WindowSystem WindowSystem = new("GentleTouch");

        private ConfigWindow ConfigWindow { get; init; }
        private MainWindow MainWindow { get; init; }


        public GentleTouch(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] ICommandManager commandManager)
        {
            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            pluginInterface.Create<Service>();

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);


            // you might normally want to embed resources and load them from the manifest stream
            var imagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "lush-3-1.png");
            var lushImage = this.PluginInterface.UiBuilder.LoadImage(imagePath);

            var connection = Connection.getInstance();

            ConfigWindow = new ConfigWindow(this);
            MainWindow = new MainWindow(this, lushImage);

            connection.Init(this.Configuration, PluginInterface.AssemblyLocation.Directory?.FullName!,MainWindow);

            CombatParser.getInstance().init();

            WindowSystem.AddWindow(ConfigWindow);
            WindowSystem.AddWindow(MainWindow);

            this.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
            {
                HelpMessage = "/gentle: Open the Gentle Touch Interface"
            });

            CommandManager.AddHandler(ConfigCommandName, new CommandInfo(OnConfigCommand)
            {
                HelpMessage = "/gentlec: Opens the Gentle Touch Configurations",
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

        }

        public void Dispose()
        {
            this.WindowSystem.RemoveAllWindows();
            
            ConfigWindow.Dispose();
            MainWindow.Dispose();

            Connection.Close();
            CombatParser.Close();

            this.CommandManager.RemoveHandler(CommandName);
            this.CommandManager.RemoveHandler(ConfigCommandName);
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            MainWindow.IsOpen = true;
        }
        private void OnConfigCommand(string command, string args)
        {
            ConfigWindow.IsOpen = true;
        }

        private void DrawUI()
        {
            this.WindowSystem.Draw();
        }

        public void DrawConfigUI()
        {
            ConfigWindow.IsOpen = true;
        }
    }
}
