#if DEBUG
using JigsawPuzzle.Analysis;
using JigsawPuzzle.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Web.Mvc;

namespace JigsawPuzzle.Controllers
{
    public class UnitTestController : Controller
    {
        [HttpGet]
        public ActionResult File()
        {
            List<object> buffer = new List<object>();
            buffer.Add(("File", Session[nameof(File)]));
            buffer.Add(("Redirect", Session[nameof(Redirect)]));
            string content = JsonConvert.SerializeObject(buffer);
            return Content(content);
        }

        [WebAPI(DebugOnly = true)]
        [HttpGet]
        public ActionResult RebuildServerRouteConfig()
        {
            ServerRouteConfig oldConfig = GetOldServerRouteConfig();
            ServerRouteConfig newConfig = GetNewServerRouteConfig();

            foreach (ControllerAction newInfo in newConfig.WebAPI)
            {
                ControllerAction oldInfo = oldConfig[newInfo.Controller, newInfo.Action];

                if (newInfo.Type == "HttpGet")
                {
                    newInfo.FormKeys = new string[0];
                    newInfo.FormValues = new string[0];
                }
                else if (newInfo.Type == "HttpPost")
                {
                    newInfo.FormKeys = oldInfo?.FormKeys ?? new string[0];
                    newInfo.FormValues = oldInfo?.FormValues ?? new string[0];
                }

                newInfo.ReturnType = oldInfo?.ReturnType ?? string.Empty;
            }
            return Content(JsonConvert.SerializeObject(newConfig));
        }
        private ServerRouteConfig GetOldServerRouteConfig()
        {
            string path = HttpContext.Server.MapPath(JPTaskConnector.Path);
            string content = System.IO.File.ReadAllText(path);
            ServerRouteConfig oldConfig = JsonConvert.DeserializeObject<ServerRouteConfig>(content);
            oldConfig.OnAfterDeserialize();
            return oldConfig;
        }
        private ServerRouteConfig GetNewServerRouteConfig()
        {
            List<WebAPIAttribute> WebAPIList = new List<WebAPIAttribute>();
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
                WebAPIList.AddRange(WebAPIAttribute.GetWebAPI(type));

            List<ControllerAction> newWebAPIList = new List<ControllerAction>();
            foreach (WebAPIAttribute webAPI in WebAPIList)
                newWebAPIList.Add(new ControllerAction(webAPI));
            ServerRouteConfig newConfig = new ServerRouteConfig
            {
                WebAPI = newWebAPIList.ToArray(),
                BaseAddress = PortConfig.Value.ServerIPAddressV4,
            };
            return newConfig;
        }
    }
}
#endif