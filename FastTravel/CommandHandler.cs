﻿using System.Linq;
using StardewModdingAPI;
using StardewValley;
using System.Text;

namespace FastTravel
{
    internal class CommandHandler
	{
		private readonly ModEntry modEntry;
        private readonly IMonitor Monitor;
        private readonly ModConfig ModConfig;

        public CommandHandler(ModEntry modEntry, IMonitor monitor, ModConfig modConfig)
		{
			this.modEntry = modEntry;
            this.Monitor = monitor;
            this.ModConfig = modConfig;
        }

        /// <summary>Handle a console command.</summary>
        /// <param name="command">The command name entered by the player.</param>
        /// <param name="args">The command arguments.</param>
        public void HandleCommand(string command, string[] args)
        {
            Monitor.Log(command, LogLevel.Warn);
            switch (args.FirstOrDefault())
            {
                case "locations":
                    HandleLocations();
                    break;
                case "playerlocation":
                    HandlePlayerLocation();
                    break;
                case "debugmode":
                    HandleDebugMode(args);
                    break;
				case "reload":
					HandleReloadConfig();
					break;
                default:
                    HandleHelp();
                    break;
            }
        }

        /// <summary>Handle the 'ft_helper locations' command.</summary>
        private void HandleLocations()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("\n#### LIST ALL LOCATIONS ####");
            report.AppendLine("This location ID can be used on 'GameLocationIndex' field, on config.json\n");
            foreach (var location in Game1.locations)
            {
                report.AppendLine($"  ID: {Game1.locations.IndexOf(location)} / Name: {location.name}");
            }
            report.AppendLine("################");
            this.Monitor.Log(report.ToString(), LogLevel.Info);
        }


        /// <summary>Handle the 'ft_helper playerlocation' command.</summary>
        private void HandlePlayerLocation()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("\n#### PLAYER LOCATION ####");
            report.AppendLine("The tile position X and Y, you can be used on 'SpawnPosition' field, on config.json\n");
            report.AppendLine($"  - currentLocation.Name: {Game1.currentLocation.Name}");
            report.AppendLine($"  - player tile position: X => {Game1.player.Tile.X} | Y => {Game1.player.Tile.Y}");
            report.AppendLine("################");
            this.Monitor.Log(report.ToString(), LogLevel.Info);
        }

        /// <summary>Handle the 'ft_helper debugmode' command.</summary>
        /// <param name="args">The command arguments.</param>
        private void HandleDebugMode(string[] args)
        {
            StringBuilder report = new StringBuilder();
            string firstArg = "";
            if (args.Length >= 2)
            {
                firstArg = args.GetValue(1).ToString();
            }
            string ArgNotValidMessage = $"\nError, argument {firstArg} not found. \nTry send 0 to disable, or 1 to enable.";

            int arg;
            if (int.TryParse(firstArg,out arg))
            {
                report.AppendLine($"arg parsed: {arg}");
                switch (arg)
                {
                    case 0:
                        if (ModConfig.DebugMode)
                        {
                            ModConfig.DebugMode = false;
                            report.AppendLine(" DebugMode changed to FALSE.");
                        }
                        break;
                    case 1:
                        if (!ModConfig.DebugMode)
                        {
                            ModConfig.DebugMode = true;
                            report.AppendLine(" DebugMode changed to TRUE.");
                        }
                        break;

                    default:
                        report.AppendLine(ArgNotValidMessage);
                        break;
                }
            } else
            {
                report.AppendLine(ArgNotValidMessage);
            }

            this.Monitor.Log(report.ToString(), LogLevel.Info);
        }

		/// <summary>Reloads the mod configuration</summary>
		private void HandleReloadConfig()
		{
			modEntry.ReloadConfig();
			Monitor.Log("Attempted config reload", LogLevel.Info);
		}
		
        /// <summary>Handle the 'ft_helper' command.</summary>
        private void HandleHelp()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine("\n#### FAST TRAVEL HELPER ####");
            report.AppendLine("You can try:");
            report.AppendLine("    ft_helper locations: To list all locations on stardew valley.");
            report.AppendLine("    ft_helper playerlocation: To get a current player location informations.");
            report.AppendLine("    ft_helper debugmode: To change debug mode. Send 0 to disable, 1 to enable.\n");
			report.AppendLine("    ft_helper reload: To reload the mod configuration.");
            this.Monitor.Log(report.ToString(), LogLevel.Info);
        }
    }
}
