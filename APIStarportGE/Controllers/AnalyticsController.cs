using APIAccount.Models;
using APIStarportGE.Filters;
using Microsoft.AspNetCore.Mvc;
using Optimization.Objects;
using System.Collections.Generic;

namespace APIStarportGE.Controllers
{
    [APIKey]
    [Route("api/analysis")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {

        [HttpGet("getbuilds")]
        public ActionResult GetBuilds(string server)
        {
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");
            }
            ColonyModel colonyModel = new ColonyModel(database);

            List<string> builds = colonyModel.GetBuildables();

            if (builds.Count > 0)
            {
                return Ok(builds);
            }
            else if (builds.Count == 0)
            {
                return StatusCode(404, $"No planet were found!");
            }
            else
            {
                return StatusCode(503);
            }
        }
    }
}
