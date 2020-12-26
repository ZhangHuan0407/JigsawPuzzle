#if DEBUG
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace JigsawPuzzle.Controllers
{
    public class UnitTestController : Controller
    {
        // GET: UnitTest
        public ActionResult File()
        {
            List<object> buffer = new List<object>();
            buffer.Add(("File", Session[nameof(File)]));
            buffer.Add(("Redirect", Session[nameof(Redirect)]));
            string content = JsonConvert.SerializeObject(buffer);
            return Content(content);
        }
    }
}
#endif