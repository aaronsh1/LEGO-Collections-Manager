using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegoCollectionManager.SetInformation;
using System;

namespace LegoCollectionManager.Controllers
{
    [Route("api/setInformation")]
    [ApiController]
    public class SetInformationApiController : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult<SetInformation.SetInformation> GetSetInformation(string id) {
            SetInformationUtil util = new SetInformationUtil();
            return util.GetSetInformation(id);
        }
    }

    [Route("setInformation")]
    public class SetInformationController : Controller {

        public ActionResult Index() {

            return View();
        }

        [Route("{id}")]
        public ActionResult SetInformation(string id) {
            SetInformationUtil util = new SetInformationUtil();

            return View(util.GetSetInformation(id));
        }
    }
}
