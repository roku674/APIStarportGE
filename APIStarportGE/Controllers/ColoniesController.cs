//Created by Alexander Fields 

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
    [Route("api/colony")]
    [ApiController]
    public class ColoniesController : ControllerBase
    {
        [HttpGet("getall")]
        public ActionResult GetAll(string server)
        {
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            ColonyModel colonyModel = new ColonyModel(database);

            List<Holding> holdings = colonyModel.GetAll();

            if (holdings.Count > 0)
            {
                return Ok(holdings);
            }
            else if (holdings.Count == 0)
            {
                return StatusCode(404, $"No planet were found!");
            }
            else
            {
                return StatusCode(503);
            }
        }

        [HttpGet("getbyname")]
        public ActionResult GetByName(string name, string server)
        {
            try
            {       
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            ColonyModel colonyModel = new ColonyModel(database);

            Holding holding = colonyModel.GetPlanetByName(name);

            if (holding != null)
            {
                return Ok(holding);
            }
            else if (holding == null)
            {
                return StatusCode(404, $"No planet was found by {name}");
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

        [HttpGet("getcoloniesbysystem")]
        public ActionResult GetBySystem(string name, string server)
        {
            try
            {

            
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            ColonyModel colonyModel = new ColonyModel(database);

            List<Holding> holdings = colonyModel.GetPlanetsBySystem(name);

            if (holdings.Count > 0)
            {
                return Ok(holdings);
            }
            else if (holdings.Count == 0)
            {
                return StatusCode(404, $"No planet was found by {name}");
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

        // POST api/<FileController>
        [HttpPost("post")]
        public IActionResult PostCol([FromBody] Holding holding, string server)
        {
            try
            {

            
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            ColonyModel colonyModel = new ColonyModel(database);

            bool succeeded = colonyModel.InsertHolding(holding);

            if (succeeded)
            {
                return Ok(succeeded);
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

        // PUT api/<FileController>/5
        [HttpPut("put")]
        public IActionResult PutCol([FromBody] Holding holding, string server)
        {
            try{
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            ColonyModel colonyModel = new ColonyModel(database);

            UpdateResult result = colonyModel.UpdateHolding(holding);

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

        // DELETE api/<FileController>/5
        [HttpDelete("deleteByName")]
        public IActionResult Delete(string name, string server)
        {
            try{
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            ColonyModel colonyModel = new ColonyModel(database);

            DeleteResult result = colonyModel.DeleteColony(name);

            if (result == null)
            {
                return StatusCode(404, $"No planet was found by {name}");
            }
            else if (result.DeletedCount == 0)
            {
                return StatusCode(404, $"No planet was found by {name}");
            }
            else if (result.DeletedCount > 0)
            {
                return Ok("Deleted: " + result.DeletedCount + "planet(s)");
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
