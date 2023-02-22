using APIAccount.Models;
using APIStarportGE.Filters;
using Microsoft.AspNetCore.Mvc;
using Optimization.Objects;
using StarportObjects;
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

        [HttpGet("getshrinkingore")]
        public ActionResult GetShrinkingOre(string server)
        {
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");
            }
            ColonyModel colonyModel = new ColonyModel(database);

            List<string> shrinkignOre = colonyModel.GetShrinkingOre();

            if (shrinkignOre.Count > 0)
            {
                return Ok(shrinkignOre);
            }
            else if (shrinkignOre.Count == 0)
            {
                return StatusCode(404, $"No planet were found!");
            }
            else
            {
                return StatusCode(503);
            }
        }

        [HttpGet("getsolarlowerthan")]
        public ActionResult GetSolarOff(int solarRate, string server)
        {
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");
            }
            ColonyModel colonyModel = new ColonyModel(database);

            List<string> lowSolars = colonyModel.GetLessthanSolar(solarRate);

            if (lowSolars.Count > 0)
            {
                return Ok(lowSolars);
            }
            else if (lowSolars.Count == 0)
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
