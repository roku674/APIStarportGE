﻿
//Created by Alexander Fields 
using APIStarportGE.Filters;
using APIStarportGE.Models;
using Microsoft.AspNetCore.Mvc;
using Optimization.Objects.Logging;
using Optimization.Utility;

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
            return Content($"{LogMessage.MessageSourceSetter} " + "\n" +
                $"Deployed At: {Program.deployTime}" + "\n" +
                $"Uptime: {System.DateTime.Now - Program.deployTime}");
        }

        [HttpGet("logs")]
        public ActionResult GetLogs()
        {
            return Content(Utility.ConvertDataTableToHTML(Utility.ConvertListToDataTable(Program.Logs), 3, 2, 1, null));
        }

    }
}
