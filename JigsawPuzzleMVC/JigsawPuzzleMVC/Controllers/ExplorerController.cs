﻿using JigsawPuzzle.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JigsawPuzzle.Controllers
{
    public class ExplorerController : Controller
    {
        /* func */
        [HttpGet]
        public Task<ActionResult> FileMap()
        {
            return Task.Run(() =>
            {
                Dictionary<string, string[]> fileMap = GetFileMap();
                return View(fileMap) as ActionResult;
            });
        }
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

        [HttpPost]
        public Task<ActionResult> SelectFiles(FormCollection form)
        {
            return Task.Run(() =>
            {
                string[] files = form.GetValues(nameof(File));
                List<FileInfo> fileInfos = new List<FileInfo>();
                string dataDirectory = PortConfig.Value.DataDirectory;
                foreach (string filePath in files)
                {
                    FileInfo fileInfo = new FileInfo($"{dataDirectory}/{filePath}");
                    if (fileInfo.Exists
                        && fileInfo.FullName.StartsWith(dataDirectory))
                        fileInfos.Add(fileInfo);
                }
                Session[nameof(File)] = fileInfos.ToArray();
                string[] redirectAction = Session[nameof(Redirect)] as string[];
                Session[nameof(Redirect)] = null;
                if (redirectAction is null)
                    return Content("Select Files successful") as ActionResult;
                else if (redirectAction.Length == 2)
                    return RedirectToAction(redirectAction[0], redirectAction[1]) as ActionResult;
                else
                    // Log this exception
                    return new HttpStatusCodeResult(400, "Select Files successful, but Redirect link have argument exception") as ActionResult;
            });
        }
    }
}