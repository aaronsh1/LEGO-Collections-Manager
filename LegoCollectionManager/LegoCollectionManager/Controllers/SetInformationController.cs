using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LegoCollectionManager.SetInformation;

namespace LegoCollectionManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SetInformationController : ControllerBase
    {
        [HttpGet("{id}")]
        public ActionResult<SetInformation.SetInformation> GetSetInformation(string id) {
            SetInformationUtil util = new SetInformationUtil();
            return util.GetSetInformation(id);
        } 
    }
}
