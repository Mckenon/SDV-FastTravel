using System;
using Microsoft.Xna.Framework;

namespace FastTravel
{
	/// <summary>A fast travel point on the map.</summary>
	[Serializable]
	public struct FastTravelPoint
	{
		[NonSerialized] public string MapName;

		/// <summary>The index of this location in <see cref="StardewValley.Game1.locations"/>.</summary>
		public int GameLocationIndex;

		/// <summary>
		/// The point name on the world map, mapped to <see cref="StardewValley.Menus.ClickableComponent.name"/>
		/// </summary>
		public string PointName;

		/// <summary>The tile position at which to place the player.</summary>
		public Point SpawnPosition;

        // <summary>Used to block location access to player without requirements</summary>
        public FastTravelPointRequireObject? Requires;
    }
}