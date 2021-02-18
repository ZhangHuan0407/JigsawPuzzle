using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace JigsawPuzzle
{
    [ShareScript]
    [Serializable]
    public class ServerRouteConfig : ISerializationCallbackReceiver
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

#if UNITY_EDITOR_WIN
        internal Uri BaseAddressUri => new Uri($"https://{BaseAddress}");
#else
        internal Uri BaseAddressUri => new Uri($"http://{BaseAddress}");
#endif

        /* ctor */
        public ServerRouteConfig()
        {
            ServerRoute = new Dictionary<string, Dictionary<string, ControllerAction>>();
        }

        /* func */
        public ControllerAction GetControllerAction(string controller, string action, string type)
        {
            if (string.IsNullOrWhiteSpace(controller))
                throw new ArgumentException($"{nameof(controller)}Can not be Null or white space", nameof(controller));
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException($"{nameof(action)}Can not be Null or white space", nameof(action));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException($"{nameof(type)}Can not be Null or white space", nameof(action));

            ControllerAction controllerAction = this[controller, action];
            if (controllerAction is null)
                throw new Exception($"Not found {controller}/{action} in {nameof(ServerRouteConfig)}");
            else if (!controllerAction.Type.Equals(type))
                throw new Exception($"{controller}/{action} type is not equal.");
            return controllerAction;
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