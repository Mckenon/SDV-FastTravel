using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastTravel
{
    [Serializable]
    public class ModConfig
    {
        /// <summary>
        /// List of locations which can be teleported to.
        /// </summary>
        public FastTravelPoint[] FastTravelPoints;
    }
}
