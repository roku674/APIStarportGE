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
            }
        }

        public List<Holding> GetAll()
        {
            return collection.Find(new BsonDocument()).ToList();
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
        public List<Holding> GetPlanetsBySystem(string name)
        {
            List<Holding> holdings = new List<Holding>();
            try
            {
                holdings = collection.AsQueryable().Where(x=> x.Location == name).ToList();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
            }
            return holdings;
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
                    .Set(y => y.Location, holding.Location)
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
                    .Set(y => y.PollutionRate, holding.PollutionRate)
                    .Set(y => y.Disaster, holding.Disaster)
                    .Set(y => y.UnProtect, holding.UnProtect)
                    .Set(y => y.PercConstruct, holding.PercConstruct)
                    .Set(y => y.PercResearch, holding.PercResearch)
                    .Set(y => y.PercMilitary, holding.PercMilitary)
                    .Set(y => y.PercHarvest, holding.PercHarvest)
                    .Set(y => y.CurrentlyBuilding, holding.CurrentlyBuilding)
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
                    .Set(y => y.SolarShots, holding.SolarShots)
                    .Set(y => y.SolarFreq, holding.SolarFreq)
                    .Set(y => y.NumDiscoveries, holding.NumDiscoveries)
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

        public UpdateResult UpdateHoldings(List<Holding> holdings)
        {
            UpdateResult result = null;
            try
            {
                List<Task> taskPool = new List<Task>();

                foreach (Holding holding in holdings)
                {
                    if (GetPlanetByName(holding.Location) == null)
                    {
                        Task task = Task.Run(() => InsertHolding(holding));

                        taskPool.Add(task);

                    }
                    else
                    {
                    UpdateDefinition<Holding> updateDefinition = Builders<Holding>.Update
                        .Set(y => y.PlanetType, holding.PlanetType)
                        .Set(y => y.HopsAway, holding.HopsAway)
                        .Set(y => y.Name, holding.Name)
                        .Set(y => y.Location, holding.Location)
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
                        .Set(y => y.PollutionRate, holding.PollutionRate)
                        .Set(y => y.Disaster, holding.Disaster)
                        .Set(y => y.UnProtect, holding.UnProtect)
                        .Set(y => y.PercConstruct, holding.PercConstruct)
                        .Set(y => y.PercResearch, holding.PercResearch)
                        .Set(y => y.PercMilitary, holding.PercMilitary)
                        .Set(y => y.PercHarvest, holding.PercHarvest)
                        .Set(y => y.CurrentlyBuilding, holding.CurrentlyBuilding)
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
                        .Set(y => y.SolarShots, holding.SolarShots)
                        .Set(y => y.SolarFreq, holding.SolarFreq)
                        .Set(y => y.NumDiscoveries, holding.NumDiscoveries)
                        .Set(y => y.Discoveries, holding.Discoveries)
                        .Set(y => y.Environment, holding.Environment)
                        ;

                    Task task = Task.Run(() => collection.UpdateOne(
                        x => x.Location.Equals(holding.Location),
                        updateDefinition));

                    taskPool.Add(task); 
                    }
                }
                Task.WaitAll(taskPool.ToArray());

            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("UpdateHoldings", MessageType.Error, e.ToString()));
            }

            return result;
        }

        public DeleteResult DeleteColony(string name)
        {
            DeleteResult result = null;
            try
            {
                collection.DeleteMany(Builders<Holding>.Filter.Eq(f => f.Location, name));
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

        public void StartColonyUpdates()
        {
            HoldingsFileModel fileModel = new HoldingsFileModel(databaseName);
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
    }
}