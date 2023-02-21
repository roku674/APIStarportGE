//Created by Alexander Fields 

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
using System.Threading;

namespace APIStarportGE.Controllers
{
    [APIKey]
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
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
            FileModel fileModel = new FileModel(database, Settings.Configuration["MongoDB:Databases:Collections:csv"]);

            List<FileObj> files = fileModel.GetCsv(name.Trim());

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
                Program.Logs.Add(new LogMessage("GalaxyContrller.GetByName", MessageType.Error, e.ToString()));
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
            FileModel fileModel = new FileModel(database, Settings.Configuration["MongoDB:Databases:Collections:csv"]);

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
                Program.Logs.Add(new LogMessage("GalaxyContrller.GetByDate", MessageType.Error, e.ToString()));
                return StatusCode(500);
            }
        }


        [HttpGet("getpicture")]
        public ActionResult GetPicture(string name, string server)
        {
            try
            {
                string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

                if (string.IsNullOrEmpty(database))
                {
                    return BadRequest($"{server} was not a valid server!");

                }
                FileModel planetModel = new FileModel(database, Settings.Configuration["MongoDB:Databases:Collections:pictures"]);

                FileObj file = planetModel.GetFile(name);

                if (file != null)
                {
                    return Ok(file);
                }
                else if (file == null)
                {
                    return StatusCode(404, $"No Planet were found");
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

        [HttpGet("getpictures")]
        public ActionResult GetPictures(string server)
        {
            try
            {
                string database = Settings.Configuration[$"MongoDB:Databases:{server}"];

                if (string.IsNullOrEmpty(database))
                {
                    return BadRequest($"{server} was not a valid server!");

                }
                FileModel planetModel = new FileModel(database, Settings.Configuration["MongoDB:Databases:Collections:pictures"]);

                List<FileObj> files = planetModel.GetAllPictures();

                if (files != null)
                {
                    return Ok(files);
                }
                else if (files == null)
                {
                    return StatusCode(404, $"No Planet were found");
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
            FileModel fileModel = new FileModel(database, Settings.Configuration["MongoDB:Databases:Collections:csv"]);

            bool suceeded = fileModel.InsertFile(file);

            if (suceeded)
                {
                    Program.Logs.Add(new LogMessage("ColoniesContrller.PutCol", MessageType.Success, $"updated {file.FileName}"));

                    return Ok();
            }
            else
            {
                return StatusCode(503);
            }
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("GalaxyContrller.Post", MessageType.Error, e.ToString()));
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
            FileModel fileModel = new FileModel(database, Settings.Configuration["MongoDB:Databases:Collections:csv"]);
            ColonyModel colonyModel = new ColonyModel(database);
            GalaxyModel galaxyModel = new GalaxyModel(database);

            UpdateResult result = fileModel.UpdateFile(file);
            ThreadPool.QueueUserWorkItem(colonyModel.StartColonyUpdates);
            ThreadPool.QueueUserWorkItem(galaxyModel.StartGalaxyUpdates);

            if (result.IsAcknowledged)
                {
                    Program.Logs.Add(new LogMessage("ColoniesContrller.PutCol", MessageType.Success, $"updated {file.FileName}"));

                    return Ok(result);
            }
            else
            {
                return StatusCode(503);
            }
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("GalaxyContrller.PutCsv", MessageType.Error, e.ToString()));
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
            FileModel fileModel = new FileModel(database, Settings.Configuration["MongoDB:Databases:Collections:csv"]);

            DeleteResult result = fileModel.DeleteCsv(name.Trim());
         
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
                    Program.Logs.Add(new LogMessage("ColoniesContrller.PutCol", MessageType.Success, $"Deleted {name}"));

                    return Ok("Deleted: " + result.DeletedCount + "file(s)");
            }
            else
            {
                return StatusCode(503);
            }
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("GalaxyContrller.Delete", MessageType.Error, e.ToString()));
                return StatusCode(500);
            }
        }
    }
}
