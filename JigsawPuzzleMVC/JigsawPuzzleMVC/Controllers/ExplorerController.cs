using JigsawPuzzle.Analysis;
using JigsawPuzzle.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JigsawPuzzle.Controllers
{
    /// <summary>
    /// <see cref="ExplorerController"/> 负责对接用户文件的浏览，创建，删除，选中操作
    /// <para>远程进程可以通过 <see cref="GetFileMapJson"/>, <see cref="SelectFiles"/> WebAPI 直接进行资源文件操作</para>
    /// <para>对于无法执行，已过时或无效的请求，不同操作器行为具有独立的应答规则</para>
    /// </summary>
    public class ExplorerController : Controller
    {
        /* func */
        [HttpGet]
        public Task<ActionResult> FileMap()
        {
            return Task.Run(() =>
            {
                Dictionary<string, string[]> fileMap = GetFileMap();
                ViewData["FileMultiple"] = Session["FileMultiple"] ?? false;
                return View(fileMap) as ActionResult;
            });
        }
        [WebAPI]
        [HttpGet]
        public Task<ActionResult> GetFileMapJson()
        {
            return Task.Run(() =>
            {
                Dictionary<string, string[]> fileMap = GetFileMap();
                string content = JsonConvert.SerializeObject(fileMap);
                return Content(content) as ActionResult;
            });
        }
        private Dictionary<string, string[]> GetFileMap()
        {
            string[] directories = Directory.GetDirectories(PortConfig.Value.DataDirectory);
            Dictionary<string, string[]> fileMap = new Dictionary<string, string[]>();
            List<string> fileNameBuffer = new List<string>();
            for (int index = 0; index < directories.Length; index++)
            {
                foreach (string file in Directory.GetFiles(directories[index], "*.json"))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    fileNameBuffer.Add(fileInfo.Name);
                }
                DirectoryInfo directoryInfo = new DirectoryInfo(directories[index]);
                fileMap.Add(directoryInfo.Name, fileNameBuffer.ToArray());
                fileNameBuffer.Clear();
            }
            return fileMap;
        }

        [WebAPI]
        [HttpPost]
        public Task<ActionResult> SelectFiles(FormCollection form)
        {
            return Task.Run(() =>
            {
                string[] files = form.GetValues(nameof(System.IO.File));
                List<FileInfo> fileInfos = new List<FileInfo>();
                string dataDirectory = PortConfig.Value.DataDirectory;
                foreach (string filePath in files)
                {
                    FileInfo fileInfo = new FileInfo($"{dataDirectory}/{filePath}");
                    if (fileInfo.Exists
                        && fileInfo.FullName.StartsWith(dataDirectory))
                        fileInfos.Add(fileInfo);
                }

                FileInfo[] selectedFileInfos = fileInfos.ToArray();
                Session[nameof(FileInfo)] = selectedFileInfos;
                string[] redirectAction = Session[nameof(Redirect)] as string[];
                Session[nameof(Redirect)] = null;
                if (redirectAction is null)
                    return Content($"Select Files successful, selected files :{selectedFileInfos.Length}") as ActionResult;
                else if (redirectAction.Length == 2)
                    return RedirectToAction(redirectAction[0], redirectAction[1]) as ActionResult;
                else
                    // Log this exception
                    return new HttpStatusCodeResult(400, "Select Files successful, but Redirect link have argument exception") as ActionResult;
            });
        }
    }
}