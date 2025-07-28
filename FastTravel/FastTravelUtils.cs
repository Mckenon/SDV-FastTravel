using System;
using System.Linq;
using StardewValley;
using StardewValley.Menus;

namespace FastTravel
{
	public class FastTravelUtils
	{
        /// <summary>Checks if a map point exists within the config.</summary>
        /// <param name="point">The map point to check.</param>
        public static bool PointExistsInConfig(ClickableComponent point)
		{
            return ModEntry.Config.FastTravelPoints.Any(t => string.Equals(point.name, t.PointName, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>Checks if a player contains needed requirements to warp.</summary>
		/// <param name="mails">Array of strings.</param>
		public static ValidationPointResult CheckPointRequiredMails(string[] mails)
		{
            string lastErrorMessageKey = null;
			bool isValidWarp = true;
			foreach (var mail in mails)
			{
				if (!Game1.player.mailReceived.Contains(mail))
				{
                    lastErrorMessageKey = mail;
                    isValidWarp = false;
					break;
				}
			}

            return new ValidationPointResult(isValidWarp, lastErrorMessageKey);
        }

		/// <summary>Gets a location for a corresponding point on the map.</summary>
		/// <param name="point">The map point to check.</param>
		public static GameLocation GetLocationForMapPoint(ClickableComponent point)
		{
			return Game1.locations[ModEntry.Config.FastTravelPoints.First(t => string.Equals(point.name, t.PointName, StringComparison.OrdinalIgnoreCase)).GameLocationIndex];
		}

		/// <summary>Gets the fast travel info for a corresponding point on the map.</summary>
		/// <param name="point">The map point to check.</param>
		public static FastTravelPoint GetFastTravelPointForMapPoint(ClickableComponent point)
		{
			FastTravelPoint fastTravelPointResult = ModEntry.Config.FastTravelPoints.First(t => string.Equals(point.name, t.PointName, StringComparison.OrdinalIgnoreCase));
            fastTravelPointResult.MapName = point.name;

            return fastTravelPointResult;
        }
	}
}