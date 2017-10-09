using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace FastTravel
{
    /// <summary>
    /// The mod entry point.
    /// </summary>
    public class ModEntry : Mod
    {
        public static ModConfig Config;

        /// <summary>
        /// The mod entry point, called after the mod is first loaded.
        /// </summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            ControlEvents.MouseChanged += this.ControlEvents_MouseChanged;
            ControlEvents.KeyPressed += this.ControlEvents_KeyPressed;
        }

        private void ControlEvents_MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            // If the world isn't ready, or the player didn't left click, we have nothing to work with.
            if (!Context.IsWorldReady || e.NewState.LeftButton != ButtonState.Pressed)
                return;

            // Create a reference to the current menu, and make sure it isn't null.
            var menu = (Game1.activeClickableMenu as GameMenu);
            if (menu == null || menu.currentTab != GameMenu.mapTab)   // Also make sure it's on the right tab(Map)
                return;

            // Get the map page from the menu.
            var mapPage = (Helper.Reflection.GetPrivateField<List<IClickableMenu>>(menu, "pages").GetValue()[3]) as MapPage;
            if (mapPage == null)    // Gotta be safe
                return;

            // Do balanced behavior.
            // (This is done after getting the map/menu to prevent spamming notifications when the player isn't in the menu)
            if (Config.BalancedMode && Game1.player.getMount() == null)
            {
                Game1.showGlobalMessage("You can't fast travel without a horse!");
                Game1.exitActiveMenu();
                return;
            }

            int x = Game1.getMouseX();
            int y = Game1.getMouseY();
            foreach (ClickableComponent point in mapPage.points)
            {
                // If the player isn't hovering over this point, don't worry about anything.
                if (!point.containsPoint(x, y))
                    continue;

                // Lonely Stone is blocked because it's not an actual place
                // Quarry is blocked because it's broken currently.
                // TODO - Fix the visual bug with Quarry
                if (point.name == "Lonely Stone")
                    continue;

                // Make sure the location is valid
                if (!FastTravelUtils.PointExistsInConfig(point))
                {
                    Monitor.Log($"Failed to find a warp for point [{point.name}]!", LogLevel.Warn);
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
                if (fastTravelPoint.GameLocationIndex == 28 && Game1.player.getMount() != null)
                    Game1.player.dismount();

                // Warp the player to their location, and exit the map.
                Game1.warpFarmer(fastTravelPoint.RerouteName == null ? location.name : fastTravelPoint.RerouteName, fastTravelPoint.SpawnPosition.X, fastTravelPoint.SpawnPosition.Y, false);
                Game1.exitActiveMenu();

                // Finally, log that we were warped.
                this.Monitor.Log($"Warping player to " + point.name);
            }
        }

        private void ControlEvents_KeyPressed(object sender, EventArgsKeyPressed e)
        {
            if (e.KeyPressed == Keys.N)
            {
                Config.BalancedMode = !Config.BalancedMode;
                Game1.showGlobalMessage("Balanced Mode: " + Config.BalancedMode);
            }
        }
    }
}