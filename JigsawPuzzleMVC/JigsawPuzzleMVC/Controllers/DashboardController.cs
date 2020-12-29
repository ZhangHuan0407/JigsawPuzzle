using System.Threading.Tasks;
using System.Web.Mvc;
using JigsawPuzzle.Analysis;
using JigsawPuzzle.Models;
using Newtonsoft.Json;

namespace JigsawPuzzle.Controllers
{
    /// <summary>
    /// <see cref="DashboardController"/> 负责展示网站当前状态的摘要数据
    /// <para>摘要数据使用 <see cref="DashboardInfo"/> 来包装</para>
    /// <para>远程进程可以通过 <see cref="GetDetailInfo"/> WebAPI 获取网站当前状态的摘要数据</para>
    /// </summary>
    public class DashboardController : Controller
    {
        [HttpGet]
        public Task<ActionResult> Index()
        {
            return Task.Run(() =>
            {
                DashboardInfo info = new DashboardInfo();
                return View(info) as ActionResult;
            });
        }

        /// <summary>
        /// 获取网站当前状态的摘要数据
        /// </summary>
        /// <returns><see cref="DashboardInfo"/> 实例的 Json 数据</returns>
        [WebAPI]
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