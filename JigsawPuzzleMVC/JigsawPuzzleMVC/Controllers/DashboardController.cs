using JigsawPuzzle.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JigsawPuzzle.Controllers
{
    public class DashboardController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            DashboardInfo info = new DashboardInfo();
            return View(info);
        }

        [HttpGet]
        public Task<ActionResult> GetDetailInfo()
        {
            return Task.Run(() => 
            {
                DashboardInfo info = new DashboardInfo();
                string content = JsonConvert.SerializeObject(info);
                return Content(content) as ActionResult;
            });
        }
    }
}