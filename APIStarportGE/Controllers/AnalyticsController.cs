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
        public ActionResult GetBuilds(string server, bool research)
        {
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");
            }
            ColonyModel colonyModel = new ColonyModel(database);

            List<string> builds = colonyModel.GetBuildables(research);

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

        [HttpGet("getlosingmorale")]
        public ActionResult GetLosingMorale(string server)
        {
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");
            }
            ColonyModel colonyModel = new ColonyModel(database);

            List<string> shrinkingMorale = colonyModel.GetShrinkingMorale();

            if (shrinkingMorale.Count > 0)
            {
                return Ok(shrinkingMorale);
            }
            else if (shrinkingMorale.Count == 0)
            {
                return StatusCode(404, $"No planet were found!");
            }
            else
            {
                return StatusCode(503);
            }
        }

        [HttpGet("getpolluting")]
        public ActionResult GetPolluting(string server)
        {
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");
            }
            ColonyModel colonyModel = new ColonyModel(database);

            List<string> pollutingColonies = colonyModel.GetPolluting();

            if (pollutingColonies.Count > 0)
            {
                return Ok(pollutingColonies);
            }
            else if (pollutingColonies.Count == 0)
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
        public ActionResult GetSolarOff(int solarRate,int population, string server)
        {
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");
            }
            ColonyModel colonyModel = new ColonyModel(database);

            List<string> lowSolars = colonyModel.GetLessthanSolar(solarRate, population);

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

        [HttpGet("gettotals")]
        public ActionResult GetPlanetTotals(string owner, string server, bool isEnemy)
        {
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];
            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");
            }
            ColonyModel colonyModel = new ColonyModel(database);

            string planetTotals = null;

            if (isEnemy)
            {
                planetTotals = colonyModel.GetEnemyPlanets(owner);
            }
            else
            {
                planetTotals = colonyModel.GetPlanetTotals(owner);
            }

            if (!string.IsNullOrWhiteSpace(planetTotals))
            {
                return Ok(planetTotals);
            }
            else if (string.IsNullOrWhiteSpace(planetTotals))
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
