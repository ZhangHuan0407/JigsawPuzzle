using JigsawPuzzle.Analysis;
using JigsawPuzzle.Models;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JigsawPuzzle.Controllers
{
    /// <summary>
    /// <see cref="TaskController"/> 负责对接执行任务的数据接收，任务递交，任务状态访问，运行结果回传
    /// <para>此控制器行为大多返回 <see cref="HttpStatusCodeResult"/> 以报告执行结果，有时以重定向作为下一步访问目标。</para>
    /// <para>对于无法执行，已过时或无效的请求，操作器行为返回 <see cref="HttpStatusCodeResult"/> 以报告执行结果</para>
    /// </summary>
    public class TaskController : Controller
    {
        [WebAPI]
        [HttpGet]
        public Task<ActionResult> StartNew()
        {
            return Task.Run(() => 
            {
                if (!(Session[nameof(FileInfo)] is FileInfo[] fileInfos)
                    || fileInfos.Length != 1)
                {
                    Session[nameof(Redirect)] = new string[2] { "StartNew", "Task" };
                    return RedirectToAction("FileMap", "Explorer") as ActionResult;
                }

                FileInfo infoDataFile = fileInfos[0];
                Session[nameof(FileInfo)] = null;
                if (!infoDataFile.FullName.StartsWith(PortConfig.Value.DataDirectory))
                    return new HttpStatusCodeResult(400, "Argument Exception") as ActionResult;

                FileInfo binDataFile = new FileInfo(infoDataFile.FullName.Replace(".json", ".bytes"));
                JPTask task = new JPTask(infoDataFile, binDataFile);

#if DEBUG && MVC
                task.Log = Session[nameof(Log)] as Log;
#endif
                task.Start();
                return Content("Start New Task successful") as ActionResult;
            });
        }
    }
}