using System;

namespace JigsawPuzzle.Models
{
    [Serializable]
    public class DashboardInfo
    {
        /* field */
        /// <summary>
        /// 反馈网站启动时获取到的端口配置
        /// </summary>
        public PortConfig PortConfig;
        /// <summary>
        /// 反馈编译时是否具有 DEBUG 符号
        /// </summary>
#if DEBUG
        public readonly bool DebugMode = true;
#else  
        public readonly bool DebugMode = false;
#endif

        /* ctor */
        public DashboardInfo()
        {
            PortConfig = PortConfig.Value;
        }

        /* operator */

    }
}