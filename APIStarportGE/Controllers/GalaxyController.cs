using APIAccount.Models;
using APIStarportGE.Filters;
using APIStarportGE.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Optimization.Objects;
using Optimization.Objects.Logging;
using StarportObjects;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace APIStarportGE.Controllers
{
    [APIKey]
    [Route("api/galaxy")]
    [ApiController]
    public class GalaxyController : ControllerBase
    {
        [HttpGet("getgalaxy")]
        public ActionResult GetGalaxy(string server)
        {
            try{
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            GalaxyModel galaxyModel = new GalaxyModel(database);

            List<StarSystem> starSystem = galaxyModel.GetStarSystems();

            if (starSystem.Count > 0)
            {
                return Ok(starSystem);
            }
            else if (starSystem.Count == 0)
            {
                return StatusCode(404, $"No Galaxy data was found!");
            }
            else
            {
                return StatusCode(503);
            }
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("GalaxyContrller.GetGalaxy", MessageType.Error, e.ToString()));
                return StatusCode(500);
            }
        }

        [HttpGet("getsystembyname")]
        public ActionResult GetSystemByName(string name, string server)
        {
            try{
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            GalaxyModel galaxyModel = new GalaxyModel(database);

            StarSystem starSystem = galaxyModel.GetSystemByName(name.Trim());

            if (starSystem != null)
            {
                return Ok(starSystem);
            }
            else if (starSystem == null)
            {
                return StatusCode(404, $"No Star System was found by {name}");
            }
            else
            {
                return StatusCode(503);
            }
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("GalaxyContrller.GetSystemByName", MessageType.Error, e.ToString()));
                return StatusCode(500);
            }
        }

        [HttpGet("getplanetbyname")]
        public ActionResult GetPlanet(string name, string server)
        {
            try{
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            GalaxyModel galaxyModel = new GalaxyModel(database);

            Planet planet = galaxyModel.GetPlanetByName(name.Trim());

            if (planet != null)
            {
                return Ok(planet);
            }
            else if (planet == null)
            {
                return StatusCode(404, $"No Planet was found by {name}");
            }
            else
            {
                return StatusCode(503);
            }
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("GalaxyContrller.GetPlanet", MessageType.Error, e.ToString()));
                return StatusCode(500);
            }
        }

        [HttpPut("putsystem")]
        public IActionResult Put([FromBody] StarSystem starSystem, string server)
        {
            try{
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            GalaxyModel galaxyModel = new GalaxyModel(database);

            UpdateResult result = galaxyModel.UpdateStarSystem(starSystem);

            if (result.IsAcknowledged)
                {
                    Program.Logs.Add(new LogMessage("ColoniesContrller.PutCol", MessageType.Success, $"Updated {starSystem.Name} on {server}"));
                    return Ok(result);
            }
            else
            {
                return StatusCode(503);
            }
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("GalaxyContrller.Put(StarSystem, string)", MessageType.Error, e.ToString()));
                return StatusCode(500);
            }
        }

        [HttpPut("putplanet")]
        public IActionResult Put([FromBody] Planet planet, string server)
        {
            try{
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            GalaxyModel galaxyModel = new GalaxyModel(database);

            UpdateResult result = galaxyModel.UpdatePlanet(planet);

            if (result.IsAcknowledged)
            {
                Program.Logs.Add(new LogMessage("ColoniesContrller.PutCol", MessageType.Success, $"Updated {planet.Name} on {server}"));
                return Ok(result);
            }
            else
            {
                return StatusCode(503);
            }
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("GalaxyContrller.Put(Planet, string)", MessageType.Error, e.ToString()));
                return StatusCode(500);
            }
        }
    }
}
