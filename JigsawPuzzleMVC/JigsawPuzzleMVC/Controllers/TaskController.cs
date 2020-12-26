using JigsawPuzzle.Models;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace JigsawPuzzle.Controllers
{
    public class TaskController : Controller
    {
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