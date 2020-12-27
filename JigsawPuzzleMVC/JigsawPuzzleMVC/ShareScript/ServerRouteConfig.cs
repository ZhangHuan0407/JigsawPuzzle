using System;
using System.Collections.Generic;

namespace JigsawPuzzle
{
    [ShareScript]
    [Serializable]
    public class ServerRouteConfig
    {
        /* field */
        public readonly Dictionary<string, Dictionary<string, ControllerAction>> ServerRoute;
        public Uri BaseAddress;

        /* inter */
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

        /* ctor */
        public ServerRouteConfig()
        {
            ServerRoute = new Dictionary<string, Dictionary<string, ControllerAction>>();
        }
    }
}