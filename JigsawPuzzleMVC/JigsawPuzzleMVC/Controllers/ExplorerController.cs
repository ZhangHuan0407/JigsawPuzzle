using JigsawPuzzle.Analysis;
using JigsawPuzzle.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
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

        /// <summary>
        /// 选中复数个用户数据文件夹内的文件
        /// <para>如果有预留的重定向目标，将立即重定向</para>
        /// <para>选中0个有效文件，将被认为选择成功</para>
        /// </summary>
        /// <param name="form">包含所有选中文件的表单</param>
        /// <returns>执行结果</returns>
        [WebAPI]
        [HttpPost]
        public Task<ActionResult> SelectFiles(FormCollection form)
        {
            return Task.Run(() =>
            {
                string[] files = form.GetValues(nameof(System.IO.File));
                if (files is null)
                    return new HttpStatusCodeResult(400, $"Select Files 0?");
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

        [WebAPI]
        [HttpPost]
        public Task<ActionResult> UploadFiles()
        {
            return Task.Run(() => 
            {
                for (int index = 0; index < Request.Files.Count; index++)
                {
                    HttpPostedFileBase fileBase = Request.Files[index];
                    FileInfo fileInfo = new FileInfo($"{PortConfig.Value.DataDirectory}/{fileBase.FileName}");
                    if (fileInfo.Directory.Exists)
                        fileBase.SaveAs(fileInfo.FullName);
                }
                return Content("UploadFile successful") as ActionResult;
            });
        }
        [WebAPI]
        [HttpPost]
        public Task<ActionResult> DownloadFile()
        {
            return Task.Run(() => 
            {
                FileInfo[] selectedFileInfos = Session[nameof(FileInfo)] as FileInfo[];
                if (selectedFileInfos is null)
                {
                    Session[nameof(Redirect)] = new string[] { nameof(DownloadFile), "Explorer" };
                    return RedirectToAction(nameof(FileMap)) as ActionResult;
                }
                else if (selectedFileInfos.Length == 0)
                    return new HttpStatusCodeResult(400, $"Select Files 0?") as ActionResult;

                FileInfo selectedFile = selectedFileInfos[0];
                if (!System.IO.File.Exists(selectedFile.FullName))
                    return new HttpStatusCodeResult(400, $"Files Not Exists.{selectedFile.Name}") as ActionResult;
                return File(System.IO.File.ReadAllBytes(selectedFile.FullName), "application/octet-stream");
            });
        }
    }
}