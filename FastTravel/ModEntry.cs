using System.Collections.Generic;
using System.Threading;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace FastTravel
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Accessors
        *********/
        public static ModConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // We need to perform atleast one 10 min clock update before allowing fast travel
            // so set the time back 10 mins and then perform one update when the day has started.
            Game1.timeOfDay -= 10;
            Game1.performTenMinuteClockUpdate();
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            // toggle balanced mode
            if (e.Button == SButton.N)
            {
                Config.BalancedMode = !Config.BalancedMode;
                Game1.showGlobalMessage($"Balanced Mode: {Config.BalancedMode}");
                return;
            }

            // handle map click
            if (e.Button == SButton.MouseLeft && Game1.activeClickableMenu is GameMenu menu && menu.currentTab == GameMenu.mapTab)
            {
                // Get the map page from the menu.
                var mapPage = (Helper.Reflection.GetField<List<IClickableMenu>>(menu, "pages").GetValue()[3]) as MapPage;
                if (mapPage == null) // Gotta be safe
                    return;

                // Do balanced behavior.
                // (This is done after getting the map/menu to prevent spamming notifications when the player isn't in the menu)
                if (Config.BalancedMode && Game1.player.mount == null)
                {
                    Game1.showGlobalMessage("You can't fast travel without a horse!");
                    Game1.exitActiveMenu();
                    return;
                }

                int x = (int)e.Cursor.ScreenPixels.X;
                int y = (int)e.Cursor.ScreenPixels.Y;
                foreach (ClickableComponent point in mapPage.points)
                {
                    // If the player isn't hovering over this point, don't worry about anything.
                    if (!point.containsPoint(x, y))
                        continue;

                    // Lonely Stone is blocked because it's not an actual place
                    // TODO - Fix the visual bug with Quarry
                    if (point.name == "Lonely Stone")
                        continue;

                    // Make sure the location is valid
                    if (!FastTravelUtils.PointExistsInConfig(point))
                    {
                        Monitor.Log($"Failed to find a warp for point [{point.name}]!", LogLevel.Warn);

                        // Right now this closes the map and opens the players bag and doesn't give
                        // the player any information in game about what just happened
                        // so we tell them a warp point wasnt found and close the menu.
                        Game1.showGlobalMessage("No warp point found.");
                        Game1.exitActiveMenu();
                        continue;
                    }

                    // Get the location, and warp the player to it's first warp.
                    var location = FastTravelUtils.GetLocationForMapPoint(point);
                    var fastTravelPoint = FastTravelUtils.GetFastTravelPointForMapPoint(point);

                    // If the player is in balanced mode, block warping to calico altogether.
                    if (Config.BalancedMode && fastTravelPoint.GameLocationIndex == 28)
                    {
                        Game1.showGlobalMessage("Fast-Travel to Calico Desert is disabled in balanced mode!");
                        Game1.exitActiveMenu();
                        return;
                    }

                    // Dismount the player if they're going to calico desert, since the bus glitches with mounts.
                    if (fastTravelPoint.GameLocationIndex == 28 && Game1.player.mount != null)
                        Game1.player.mount.dismount();

                    // Warp the player to their location, and exit the map.
                    Game1.warpFarmer(fastTravelPoint.RerouteName ?? location.Name, fastTravelPoint.SpawnPosition.X, fastTravelPoint.SpawnPosition.Y, false);
                    Game1.exitActiveMenu();

                    // Lets check for warp status and give the player feed back on what happened to the warp.
                    // We are doing this check on a thread because we have to wait untill the warp has finished
                    // to check its result.
                    var locationNames = new[] { fastTravelPoint.RerouteName, location.Name };
                    var t1 = new Thread(CheckIfWarped);
                    t1.Start(locationNames);
                }
            }
        }

        private void CheckIfWarped(object locationNames)
        {
            var locNames = (string[])locationNames;

            // We need to wait atleast 1.5 seconds to let the location change be complet before checking for it.
            Thread.Sleep(1500);

            // If RerouteName is null we want the LocationName instead.
            // 0 = RerouteName, 1 = LocationName
            var tmpLocName = locNames[0] ?? locNames[1];

            // Check if we are at the new location and if its a festival day.
            if (Game1.currentLocation.Name != tmpLocName && Utility.isFestivalDay(Game1.dayOfMonth, Game1.currentSeason))
                // If there is a festival and we werent able to warp let the player know.
                Game1.showGlobalMessage("Today's festival is being set up. Try going later.");
            else
                // Finally, if we managed to warp log that we were warped.
                this.Monitor.Log($"Warping player to {tmpLocName}");
        }
    }
}