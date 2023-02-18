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
using System.Threading;

namespace APIStarportGE.Controllers
{
    [APIKey]
    [Route("api/file")]
    [ApiController]
    public class HoldingsFileController : ControllerBase
    {      
        [HttpGet("getcsvbyname")]
        public ActionResult GetByName(string name, string server)
        {
            try{
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            HoldingsFileModel fileModel = new HoldingsFileModel(database);

            List<FileObj> files = fileModel.GetCsv(name);

            if (files.Count > 0)
            {
                return Ok(files);
            }
            else if (files.Count == 0)
            {
                return StatusCode(404,$"No file was found by {name}");
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
        [HttpGet("getcsvbydate")]
        public ActionResult GetByDate(DateTime date, string server)
        {
            try{
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            HoldingsFileModel fileModel = new HoldingsFileModel(database);

            List<FileObj> files = fileModel.GetCsv(date);

            if (files.Count > 0)
            {
                return Ok(files);
            }
            else if (files.Count == 0)
            {
                return StatusCode(404, $"No file was found by {date.ToString()}");
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
        [HttpPost("postcsv")]
        public IActionResult Post([FromBody] FileObj file, string server)
        {
            try{
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            HoldingsFileModel fileModel = new HoldingsFileModel(database);

            bool suceeded = fileModel.InsertFile(file);

            if (suceeded)
            {
                return Ok();
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
        [HttpPut("putcsv")]
        public IActionResult PutCsv([FromBody] FileObj file, string server)
        {
            try{
            string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

            if (string.IsNullOrEmpty(database))
            {
                return BadRequest($"{server} was not a valid server!");

            }
            HoldingsFileModel fileModel = new HoldingsFileModel(database);
            ColonyModel colonyModel = new ColonyModel(database);
            GalaxyModel galaxyModel = new GalaxyModel(database);

            UpdateResult result = fileModel.UpdateCsv(file);
            Thread colonyThread = new Thread(colonyModel.StartColonyUpdates);
            Thread galaxyThread = new Thread(galaxyModel.StartGalaxyUpdates);

            colonyThread.Start();
            galaxyThread.Start();

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
            HoldingsFileModel fileModel = new HoldingsFileModel(database);

            DeleteResult result = fileModel.DeleteCsv(name);
         
            if (result == null)
            {
                return StatusCode(404, $"No file was found by {name}");
            }
            else if (result.DeletedCount == 0)
            {
                return StatusCode(404, $"No file was found by {name}");
            }
            else if (result.DeletedCount > 0)
            {
                return Ok("Deleted: " + result.DeletedCount + "file(s)");
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
