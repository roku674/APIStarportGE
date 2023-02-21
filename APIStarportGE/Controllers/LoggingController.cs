
//Created by Alexander Fields 
using APIStarportGE.Filters;
using APIStarportGE.Models;
using Microsoft.AspNetCore.Mvc;
using Optimization.Objects.Logging;

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
            return Content($"{LogMessage.MessageSourceSetter} " +
                $"Last Updated: 2/21/2023:0125" +
                $"Deployed At: {Program.deployTime}" +
                $"Uptime: {System.Convert.ToDateTime(System.DateTime.Now - Program.deployTime)}");
        }

    }
}
