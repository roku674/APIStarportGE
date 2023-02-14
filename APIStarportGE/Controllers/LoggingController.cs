
//Created by Alexander Fields 
using APIStarportGE.Filters;
using APIStarportGE.Models;
using Microsoft.AspNetCore.Mvc;
using Optimization.Objects.Logging;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace APIStarportGE.Controllers
{
    [APIKey]
    [ApiController]
    [Route("logging")]
    public class LoggingController : ControllerBase
    {
        LoggingModel model;

        public LoggingController()
        {
            model = new LoggingModel();
        }

        [HttpGet("ping")]
        public ActionResult Ping()
        {
            return Content(LogMessage.MessageSourceSetter);
        }

        [HttpPost("postlog")]
        public IActionResult PostLogAzure([FromBody] List<LogMessage> logMessages)
        {
            if (logMessages.Count <= 0)
            {
                return BadRequest("Empty");
            }

            /*Task.Run(() => model.CheckForCrits(logMessages));

            int result = model.InsertLogAzure(logMessages);

            if (result >= 0)
            {
                return StatusCode(result);
            }*/

            return StatusCode(500);
        }

    }
}
