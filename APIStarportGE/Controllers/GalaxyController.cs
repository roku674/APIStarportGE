using APIAccount.Models;
using APIStarportGE.Filters;
using APIStarportGE.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Optimization.Objects;
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
                Console.WriteLine(e);
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

            StarSystem starSystem = galaxyModel.GetSystemByName(name);

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
                Console.WriteLine(e);
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

            Planet planet = galaxyModel.GetPlanetByName(name);

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
                Console.WriteLine(e);
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
                return Ok(result);
            }
            else
            {
                return StatusCode(503);
            }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
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
                return Ok(result);
            }
            else
            {
                return StatusCode(503);
            }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500);
            }
        }

    }
}
