using System;

namespace JigsawPuzzle.Models
{
    [Serializable]
    public class DashboardInfo
    {
        /* field */
        public PortConfig PortConfig;

        /* ctor */
        public DashboardInfo()
        {
            PortConfig = PortConfig.Value;
        }

        /* operator */

    }
}