using JigsawPuzzle.Analysis;
using JigsawPuzzle.Models;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace JigsawPuzzleMVC
{
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// ��վ�����¼�
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            PortConfig.ReloadPortConfig();
            Application.Add(nameof(Log), Log.CreateAppLog());
        }

        /// <summary>
        /// ��վ������
        /// </summary>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError().GetBaseException();
            Server.ClearError();
            if (Application[nameof(Log)] is Log log)
            {
                try
                {
                    log.WriteData(Session, exception);
                    log.WriteOut();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// �Ự�����¼�
        /// </summary>
        protected void Session_Start(object sender, EventArgs e)
        {
            Log log = Application[nameof(Log)] as Log;
            Session[nameof(Log)] = log;
            log.WriteData(Session, $"Session_Start\nUserHostAddress : {Request.UserHostAddress}\nUserAgent : {Request.UserAgent}\nUserHostName : {Request.UserHostName}");
        }
        /// <summary>
        /// �Ự�����¼�
        /// </summary>
        protected void Session_End(object sender, EventArgs e)
        {
            Log log = Application[nameof(Log)] as Log;
            log.WriteData(Session, "Session_End");
        }
    }
}
