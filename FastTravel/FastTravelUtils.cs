using System.Linq;
using StardewModdingAPI;
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
            return ModEntry.Config.FastTravelPoints.Any(t => point.myID == t.pointId);
		}

		/// <summary>Checks if a player contains needed requirements to warp.</summary>
		/// <param name="mails">Array of strings.</param>
		public static bool CheckPointRequiredMails(string[] mails)
		{
			bool isValidWarp = true;
			foreach (var mail in mails)
			{
				if (!Game1.player.mailReceived.Contains(mail))
				{
					isValidWarp = false;
					break;
				}
			}
			return isValidWarp;
		}

		/// <summary>Gets a location for a corresponding point on the map.</summary>
		/// <param name="point">The map point to check.</param>
		public static GameLocation GetLocationForMapPoint(ClickableComponent point)
		{
			string pointName = point.name;
			return Game1.locations[ModEntry.Config.FastTravelPoints.First(t => t.pointId == point.myID).GameLocationIndex];
		}

		/// <summary>Gets the fast travel info for a corresponding point on the map.</summary>
		/// <param name="point">The map point to check.</param>
		public static FastTravelPoint GetFastTravelPointForMapPoint(ClickableComponent point)
		{
			return ModEntry.Config.FastTravelPoints.First(t => t.pointId == point.myID);
		}
	}
}