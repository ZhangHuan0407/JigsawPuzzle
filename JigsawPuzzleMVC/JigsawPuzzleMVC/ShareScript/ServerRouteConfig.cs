using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace JigsawPuzzle
{
    [ShareScript]
    [Serializable]
#if UNITY_EDITOR
    public class ServerRouteConfig : ISerializationCallbackReceiver
#else
    public class ServerRouteConfig
#endif
    {
        /* field */
        internal readonly Dictionary<string, Dictionary<string, ControllerAction>> ServerRoute;
        public string BaseAddress;

        /// <summary>
        /// 当前服务器路由配置中全部的控制器行为
        /// </summary>
        public ControllerAction[] WebAPI;

        /* inter */
        /// <summary>
        /// 获取目标控制器行为，如果此控制器不存在或行为不存在，将返回 null
        /// </summary>
        /// <param name="controller">目标控制器</param>
        /// <param name="action">目标行为</param>
        /// <returns>控制器行为数据或 null</returns>
        public ControllerAction this[string controller, string action]
        {
            get 
            {
                if (ServerRoute.TryGetValue(controller, out Dictionary<string, ControllerAction> controllerRoute)
                    && controllerRoute.TryGetValue(action, out ControllerAction controllerAction))
                    return controllerAction;
                else
                    return null;
            }
        }

        internal Uri BaseAddressUri => new Uri(BaseAddress);

        /* ctor */
        public ServerRouteConfig()
        {
            ServerRoute = new Dictionary<string, Dictionary<string, ControllerAction>>();
        }

        /* ISerializationCallbackReceiver */
        public void OnBeforeSerialize()
        {
            ServerRoute.Clear();
        }
        public void OnAfterDeserialize()
        {
            foreach (ControllerAction controllerAction in WebAPI)
            {
                if (!ServerRoute.ContainsKey(controllerAction.Controller))
                    ServerRoute.Add(controllerAction.Controller, new Dictionary<string, ControllerAction>());
                Dictionary<string, ControllerAction> controllerRoute = ServerRoute[controllerAction.Controller];
                controllerRoute.Add(controllerAction.Action, controllerAction);
            }
        }
    }
}