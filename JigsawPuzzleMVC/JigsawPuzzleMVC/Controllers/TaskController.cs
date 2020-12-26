using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace JigsawPuzzle.Controllers
{
    public class TaskController : Controller
    {
        [HttpPost]
        public Task<ActionResult> CreateNewTask(FormCollection form)
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
                    return new HttpStatusCodeResult(412, "BinData.Length is not equal than InfoData.Defined") as ActionResult;
                // 序列化版本已不再支持

                // 

                return new HttpStatusCodeResult(200, "Create New Task successful") as ActionResult;
            });
        }
    }
}