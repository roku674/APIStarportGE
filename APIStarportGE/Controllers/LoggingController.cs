
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

    }
}
