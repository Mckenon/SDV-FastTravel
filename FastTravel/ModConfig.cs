using System;

namespace FastTravel
{
    [Serializable]
    public class ModConfig
    {
        /// <summary>Whether the game should run in balanced mode. See the mod page for an explanation.</summary>
        public bool BalancedMode { get; set; }

        /// <summary>A list of locations which can be teleported to.</summary>
        public FastTravelPoint[] FastTravelPoints { get; set; }
    }
}
