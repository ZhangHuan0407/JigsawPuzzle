﻿using JigsawPuzzle.Analysis;
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
        [HttpPost]
        public Task<ActionResult> CreateNewData(FormCollection form)
        {
            return Task.Run(() => 
            {
                string dataName = form["Name"];
                if (string.IsNullOrWhiteSpace(dataName)
                    || dataName.Contains("."))
                    return new HttpStatusCodeResult(412, "Name not found") as ActionResult;

                string infoData = form["InfoData"];
                if (string.IsNullOrWhiteSpace(infoData))
                    return new HttpStatusCodeResult(412, "InfoData not found") as ActionResult;
                JigsawPuzzleInfoData jPInfoData = JsonConvert.DeserializeObject<JigsawPuzzleInfoData>(infoData);

                byte[] binData = form.GetValue("BinData").ConvertTo(typeof(byte[])) as byte[];
                if (binData is null)
                    return new HttpStatusCodeResult(412, "BinData not found") as ActionResult;
                else if (binData.Length != jPInfoData.BinDataLength)
                    return new HttpStatusCodeResult(412, "BinData.Length is not equal than InfoData.BinDataLength") as ActionResult;
                // 序列化版本已不再支持

                System.IO.File.WriteAllText($"{jPInfoData.DataName}.json", infoData);
                System.IO.File.WriteAllBytes($"{jPInfoData.DataName}.bytes", binData);

                return new HttpStatusCodeResult(200, "Create New Data successful") as ActionResult;
            });
        }

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
                task.Start();
                return new HttpStatusCodeResult(200, "Start New Task successful") as ActionResult;
            });
        }
    }
}