//Copyright © 2022, Alexander Fields. All rights reserved.

using MongoDB.Driver;
using System.Linq;
using Optimization.Objects;
using StarportObjects;
using APIStarportGE.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Optimization.Utility;
using MongoDB.Bson;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using APIStarportGE.Models;
using System.Xml.Linq;
using System.Reflection;
using APIStarportGE;
using Optimization.Objects.Logging;
using System.Net.Http;

namespace APIAccount.Models
{
    public class ColonyModel
    {
        private readonly IMongoCollection<Holding> collection;
        private readonly IMongoDatabase database;
        private readonly string databaseName;

        public ColonyModel(string _databaseName)
        {
            databaseName = _databaseName;
            string colonies = Settings.Configuration["MongoDB:Databases:Collections:colonies"];

            database = new Repository().EstablishConnection().GetDatabase(databaseName);
            if (database != null)
            {
                collection = database.GetCollection<Holding>(colonies);
            }
            else
            {
                System.Console.WriteLine("ERROR: database was not found!");
                Program.Logs.Add(new LogMessage("ColonyModel", MessageType.Critical, "Database Not Found!"));
                Program.SendWebhook(new HttpClient(), Newtonsoft.Json.JsonConvert.SerializeObject(new LogMessage("ColonyModel", MessageType.Critical, "Database Not Found!")));
            }
        }

        public List<Holding> GetAll()
        {
            return collection.Find(new BsonDocument()).ToList();
        }

        public List<string> GetBuildables()
        {
            List<Holding> holdings = collection.Find(h => h.Population <= 5000).ToList();

            List<string> planetNames = new List<string>();
            foreach(Holding holding in holdings)
            {
                bool adding = false;
                if (holding.Discoveries.Contains("Arch lvl 5"))
                    adding = true;
                else if (holding.Discoveries.Contains("Arch lvl 4") && holding.PlanetType != "mountainous" && holding.PlanetType != "desert")
                    adding = true;
                else if (holding.Discoveries.Contains("Arch lvl 3") && holding.PlanetType != "mountainous" && holding.PlanetType != "desert" && holding.PlanetType != "volcanic")
                    adding = true;
                else if (holding.Discoveries.Contains("Arch lvl 2") && holding.PlanetType == "arctic")
                    adding = true;
                else if (holding.Discoveries.Contains("MT lvl 3") || holding.Discoveries.Contains("MT lvl 4") || holding.Discoveries.Contains("MT lvl 5"))
                    adding = true;
                else if ((holding.Discoveries.Contains("Ore 3") || holding.Discoveries.Contains("Ore 4") || holding.Discoveries.Contains("Ore 5")) && holding.PlanetType != "rocky")
                    adding = true;
                else if (holding.Discoveries.Contains("WP 3") || holding.Discoveries.Contains("WP 2"))
                    adding = true;

                if (adding)
                {
                    planetNames.Add(holding.Location);
                }
            }
            return planetNames;
        }

        public Holding GetPlanetByName(string planetName)
        {
            Holding holding = null;
            try
            {
                holding = collection.AsQueryable().FirstOrDefault(x => x.Location.Equals(planetName));
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("GetPlanetByName", MessageType.Error, e.ToString()));
            }
            return holding;
        }

        public List<string> GetPolluting()
        {
            List<Holding> holdings = GetAll();

            List<Holding> offlineSolars = holdings.FindAll(p => p.PolluteRate >0 && p.Population > 1000);

            List<string> colonyNames = new List<string>();
            foreach (Holding holding in offlineSolars)
            {
                colonyNames.Add(holding.Location);
            }

            return colonyNames;
        }

        public List<string> GetShrinkingMorale()
        {
            List<Holding> holdings = GetAll();

            List<Holding> offlineSolars = holdings.FindAll(p => p.MoraleChange < 0 && p.Population > 1000);

            List<string> colonyNames = new List<string>();
            foreach (Holding holding in offlineSolars)
            {
                colonyNames.Add(holding.Location);
            }

            return colonyNames;
        }

        public List<string> GetShrinkingOre()
        {
            //i need to get yesterday's holdings

            DataTable csvDt = GetYesterdaysFileAsDataTable();
            List<Holding> yesterdaysHoldings = Utility.ConvertDataTableToList<Holding>(csvDt);
            List<Holding> todaysHoldings = GetAll();

            List<string> losingOre = new List<string>();

            foreach(Holding holding in todaysHoldings)
            {
                Holding planetYesterday = yesterdaysHoldings.Find(h => h.Location == holding.Location);
                if (planetYesterday != null)         
                {
                    int remainder = holding.Ore - planetYesterday.Ore;

                    if(remainder < 0)
                    {
                        losingOre.Add(holding.Location);
                    }
                }
            }

            return losingOre;

        }

        public List<string> GetLessthanSolar(int solarNum)
        {
            List<Holding> holdings = GetAll();

            List<Holding> offlineSolars = holdings.FindAll(p => p.Solarshots <= solarNum && p.Population > 1000);

            List<string> colonyNames = new List<string>();
            foreach(Holding holding in offlineSolars)
            {
                colonyNames.Add(holding.Location);
            }

            return colonyNames;
        }

        public bool InsertHolding(Holding holding)
        {
            if (holding.Location == null)
            {
                return false;
            }
            return Repository.Insert(collection, holding);
        }
        public bool InsertHoldings(List<Holding> holdings)
        {
            if(holdings.Any(x => x.Location == null))
            {
                return false;
            }
            return Repository.Insert(collection, holdings);
        }

        public UpdateResult UpdateHolding(Holding holding)
        {
            UpdateResult result = null;
            try
            {
                if (GetPlanetByName(holding.Location) == null)
                {
                    InsertHolding(holding);
                }
                
                UpdateDefinition<Holding> updateDefinition = Builders<Holding>.Update
                    .Set(y => y.PlanetType, holding.PlanetType)
                    .Set(y => y.HopsAway, holding.HopsAway)
                    .Set(y => y.Name, holding.Name)
                    .Set(y => y.Location, holding.Location.Trim())
                    .Set(y => y.GalaxyX, holding.GalaxyX)
                    .Set(y => y.GalaxyY, holding.GalaxyY)
                    .Set(y => y.Owner, holding.Owner)
                    .Set(y => y.Corporation, holding.Corporation)
                    .Set(y => y.Founded, holding.Founded)
                    .Set(y => y.Population, holding.Population)
                    .Set(y => y.PopGrowth, holding.PopGrowth)
                    .Set(y => y.Morale, holding.Morale)
                    .Set(y => y.MoraleChange, holding.MoraleChange)
                    .Set(y => y.Government, holding.Government)
                    .Set(y => y.Credits, holding.Credits)
                    .Set(y => y.CredGrowth, holding.CredGrowth)
                    .Set(y => y.Pollution, holding.Pollution)
                    .Set(y => y.PolluteRate, holding.PolluteRate)
                    .Set(y => y.Disasters, holding.Disasters)
                    .Set(y => y.UNProtect, holding.UNProtect)
                    .Set(y => y.PercConstruct, holding.PercConstruct)
                    .Set(y => y.PercResearch, holding.PercResearch)
                    .Set(y => y.PercMilitary, holding.PercMilitary)
                    .Set(y => y.PercHarvest, holding.PercHarvest)
                    .Set(y => y.CurrentBuild, holding.CurrentBuild)
                    .Set(y => y.BuildMinutes, holding.BuildMinutes)
                    .Set(y => y.Ore, holding.Ore)
                    .Set(y => y.Ana, holding.Ana)
                    .Set(y => y.Med, holding.Med)
                    .Set(y => y.Org, holding.Org)
                    .Set(y => y.Oil, holding.Oil)
                    .Set(y => y.Ura, holding.Ura)
                    .Set(y => y.Equ, holding.Equ)
                    .Set(y => y.Spi, holding.Spi)
                    .Set(y => y.Nukes, holding.Nukes)
                    .Set(y => y.Negotiators, holding.Negotiators)
                    .Set(y => y.Mines, holding.Mines)
                    .Set(y => y.CompoundMines, holding.CompoundMines)
                    .Set(y => y.FlakCannons, holding.FlakCannons)
                    .Set(y => y.LaserCannons, holding.LaserCannons)
                    .Set(y => y.Shields, holding.Shields)
                    .Set(y => y.Solarshots, holding.Solarshots)
                    .Set(y => y.Solarfreq, holding.Solarfreq)
                    .Set(y => y.NumDiscovs, holding.NumDiscovs)
                    .Set(y => y.Discoveries, holding.Discoveries)
                    .Set(y => y.Environment, holding.Environment)
                    ;

                result = collection.UpdateOne(
                    x => x.Location.Equals(holding.Location),
                    updateDefinition);
              
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("UpdateHolding", MessageType.Error, e.ToString()));
            }

            return result;
        }

        public List<UpdateResult> UpdateHoldings(List<Holding> holdings)
        {
            List<UpdateResult> results = new List<UpdateResult>();
            try
            {
                List<Task> taskPool = new List<Task>();
                List<Task<UpdateResult>> updateTasks = new List<Task<UpdateResult>>();

                foreach (Holding holding in holdings)
                {
                    if (GetPlanetByName(holding.Location) == null)
                    {
                        Task task = Task.Run(() => InsertHolding(holding));

                        taskPool.Add(task);
                    }
                    else
                    {                      
                        Task<UpdateResult> updateTask = Task.Run(() => UpdateHolding(holding));

                        updateTasks.Add(updateTask);
                    }
                }
                Task.WaitAll(taskPool.ToArray());
                Task.WaitAll(updateTasks.ToArray());

                foreach (var task in updateTasks)
                {
                    results.Add(task.Result);
                }
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("UpdateHoldings", MessageType.Error, e.ToString()));
            }

            return results;
        }

        public DeleteResult DeleteColony(string name)
        {
            DeleteResult result = null;
            try
            {
                collection.DeleteMany(Builders<Holding>.Filter.Eq(f => f.Location, name));
                System.Console.WriteLine("Deleted Colony: " + name);
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("DeleteColony", MessageType.Error, e.ToString()));
            }
            return result;
        }

        public void RunUpdateHoldings(string fileContents)
        {
            string tempFile = Path.GetTempFileName().Replace(".tmp", ".csv");

            try
            {
                System.IO.File.WriteAllText(tempFile, fileContents);

                DataTable dataTable = Utility.ConvertCSVtoDataTable(tempFile);
                dataTable.Columns.RemoveAt(dataTable.Columns.Count - 1);

                foreach (DataRow row in dataTable.Rows)
                {
                    int environmentValue = Convert.ToInt32(row["Environment"]);
                    row["Environment"] = Convert.ToBoolean(environmentValue);
                }

                List<Holding> holdings = Utility.ConvertDataTableToList<Holding>(dataTable);

                List<Holding> currentHoldings = new ColonyModel(databaseName).GetAll();

                List<Holding> dontExist = currentHoldings.Except(holdings, new HoldingComparer()).ToList();

                int removed = 0;
                foreach(Holding holding in dontExist)
                {
                    DeleteColony(holding.Location);
                    removed+= holdings.RemoveAll(item => item.Location == holding.Location);
                }
                
                UpdateHoldings(holdings);

                System.IO.File.Delete(tempFile);
                Program.Logs.Add(new LogMessage("RunUpdateGalaxyColonies", MessageType.Success, "Ran to completion."));
            }
            catch (System.Exception exc)
            {
                Program.Logs.Add(new LogMessage("RunUpdateHoldings", MessageType.Error, exc.ToString()));
                System.IO.File.Delete(tempFile);
            }

        }

        public void StartColonyUpdates(object hollerback)
        {
            FileModel fileModel = new FileModel(databaseName, Settings.Configuration["MongoDB:Databases:Collections:csv"]);
            GalaxyModel galaxyModel = new GalaxyModel(databaseName);

            string month = "";
            string day = "";

            if (DateTime.Now.Month < 10)
            {
                month = "0" + DateTime.Now.Month;
            }
            else
            {
                month = DateTime.Now.Month.ToString();
            }
            if (DateTime.Now.Day < 10)
            {
                day = "0" + DateTime.Now.Day;
            }
            else
            {
                day = DateTime.Now.Day.ToString();
            }

            List<FileObj> files = fileModel.GetCsv($"holdings_{DateTime.Now.Year}{month}{day}.csv");
            System.DateTime dateTime = new(DateTime.Now.Year, int.Parse(month), int.Parse(day));

            while (files.Count <= 0)
            {
                System.Console.WriteLine($"{month}/{day}/{DateTime.Now.Year} wasn't found!");

                dateTime = dateTime.AddDays(-1);

                if (dateTime.Month < 10)
                {
                    month = "0" + dateTime.Month;
                }
                else
                {
                    month = dateTime.Month.ToString();
                }
                if (dateTime.Day < 10)
                {
                    day = "0" + dateTime.Day;
                }
                else
                {
                    day = dateTime.Day.ToString();
                }
                System.Console.WriteLine($"Attempting holdings_{dateTime.Year}{month}{day}.csv in StartColonyUpdates");

                files = fileModel.GetCsv($"holdings_{dateTime.Year}{month}{day}.csv");
            }
            RunUpdateHoldings(files[0].FileContents);
        }

        private DataTable GetYesterdaysFileAsDataTable()
        {
            string month = "";
            string day = "";
            DateTime dateTime = DateTime.Now.AddDays(-1);

            if (dateTime.Month < 10)
            {
                month = "0" + dateTime.Month;
            }
            else
            {
                month = dateTime.Month.ToString();
            }
            if (dateTime.Day < 10)
            {
                day = "0" + dateTime.Day;
            }
            else
            {
                day = dateTime.Day.ToString();
            }
            string yesterdayCsv = $"holdings_{DateTime.Now.Year}{month}{day}.csv";

            FileModel fileModel = new FileModel(databaseName, Settings.Configuration["MongoDB:Databases:Collections:csv"]);
            FileObj csvFile = fileModel.GetFile(yesterdayCsv);
            string tempPath = Path.GetTempFileName().Replace(".tmp", ".csv");
            System.IO.File.WriteAllText(tempPath, csvFile.FileContents);
            DataTable csvDt = Utility.ConvertCSVtoDataTable(tempPath);
            System.IO.File.Delete(tempPath);

            foreach (DataRow row in csvDt.Rows)
            {
                int environmentValue = Convert.ToInt32(row["Environment"]);
                row["Environment"] = Convert.ToBoolean(environmentValue);
            }

            return csvDt;
        }

        
    }
}