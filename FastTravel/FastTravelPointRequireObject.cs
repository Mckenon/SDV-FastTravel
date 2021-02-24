using System;

namespace FastTravel
{
    /// <summary>A fast travel point on the map.</summary>
    [Serializable]
    public struct FastTravelPointRequireObject
    {
        /// <summary>List of required mail flags, in <see cref="StardewValley.Farmer.mailReceived"/>.</summary>
        public string[] mails;
    }
}