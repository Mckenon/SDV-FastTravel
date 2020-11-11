using System;
using Microsoft.Xna.Framework;

namespace FastTravel
{
	/// <summary>A fast travel point on the map.</summary>
	[Serializable]
	public struct FastTravelPoint
	{
		/// <summary>The display name.</summary>
		public string MapName;

		/// <summary>The index of this location in <see cref="StardewValley.Game1.locations"/>.</summary>
		public int GameLocationIndex;

		/// <summary>The tile position at which to place the player.</summary>
		public Point SpawnPosition;

		/// <summary>The location name in which to place the player, or null to check the map point.</summary>
		public string RerouteName;
	}
}