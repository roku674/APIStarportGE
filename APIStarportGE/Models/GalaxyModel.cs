using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Optimization.Objects;
using Optimization.Objects.Logging;
using Optimization.Utility;
using StarportObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace APIStarportGE.Models
{
    public class GalaxyModel
    {
        private readonly IMongoCollection<StarSystem> collection;
        private readonly IMongoDatabase database;
        private readonly string databaseName;

        public GalaxyModel(string _databaseName)
        {
            databaseName = _databaseName;
            string colonies = Settings.Configuration["MongoDB:Databases:Collections:galaxy"];

            database = new Repository.Repository().EstablishConnection().GetDatabase(databaseName);

            if (database != null)
            {
                collection = database.GetCollection<StarSystem>(colonies);
            }
            else
            {
                System.Console.WriteLine("ERROR: database was not found!");
                Program.Logs.Add(new LogMessage("GalaxyModel", MessageType.Critical, "Database Not Found!"));
                Program.SendWebhook(new HttpClient(), Newtonsoft.Json.JsonConvert.SerializeObject(new LogMessage("ColonyModel", MessageType.Critical, "Database Not Found!")));

            }
        }

        public Planet GetPlanetByName(string name)
        {
            Planet planet = null;
            try
            {
                StarSystem starSystem = GetSystemByName(StarSystem.GetSystemNameFromPlanet(name));
                if (starSystem == null)
                {
                    return null;
                }
                planet = starSystem.Planets.Find(p=> p.Name ==name);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
            }

            //if the planet is null we can try to get the picture

            if (planet != null)
            {
                planet.Picture = new FileModel(databaseName, Settings.Configuration["MongoDB:Databases:Collections:pictures"]).GetFile(name + ".png");
            }

            return planet;
        }

        public StarSystem GetSystemByName(string name)
        {
            StarSystem system = null;
            try
            {
                system = collection.Find(x => x.Name == name).FirstOrDefault();                             
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
            }

            if (system != null)
            {
                foreach (Planet planet in system.Planets)
                {
                    FileModel fileModel = new FileModel(databaseName, Settings.Configuration["MongoDB:Databases:Collections:pictures"]);
                    if (planet.Picture != null)
                    {
                        planet.Picture = fileModel.GetFile(planet.Name + ".png");
                    }
                }
            }

            return system;
        }

        public StarSystem GetSystemByNameNoPlanetPics(string name)
        {
            StarSystem system = null;
            try
            {
                system = collection.Find(x => x.Name == name).FirstOrDefault();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
            }          
            return system;
        }


        public List<StarSystem> GetStarSystems()
        {
            List<StarSystem> system = new List<StarSystem>();
            try
            {
                system = collection.Find(new BsonDocument()).ToList();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
            }
            return system;
        }

        public bool InsertStarSystem(StarSystem starSystem)
        {
            List<Planet> planetsWPictures = starSystem.Planets.FindAll(p => p.Picture != null);
            if(planetsWPictures.Count > 0)
            {
                new FileModel(databaseName, Settings.Configuration["MongoDB:Databases:Collections:pictures"]).UpdatePictures(planetsWPictures);
            }

            foreach (Planet planet in starSystem.Planets)
            {
                planet.Picture = null;
            }

            return Repository.Repository.Insert<StarSystem>(collection, starSystem);
        }

        public void RunUpdateGalaxyColonies(string fileContents)
        {
            string tempFile = Path.GetTempFileName().Replace(".tmp", ".csv");

            try
            {
                System.IO.File.WriteAllText(tempFile, fileContents);

                string[] lines = File.ReadAllLines(tempFile);
                for (int i = 0;i < lines.Length;i++)
                {
                    lines[i] = lines[i].Substring(0, lines[i].Length - 1);
                }

                File.WriteAllLines(tempFile, lines);

                DataTable dataTable = Utility.ConvertCSVtoDataTable(tempFile);

                foreach (DataRow row in dataTable.Rows)
                {
                    int environmentValue = Convert.ToInt32(row["Environment"]);
                    row["Environment"] = Convert.ToBoolean(environmentValue);
                }              

                List<Holding> holdings = Utility.ConvertDataTableToList<Holding>(dataTable);

                List<StarSystem> starSystems = new List<StarSystem>();
             
                foreach (Holding holding in holdings)
                {
                    StarSystem starSystem = GetSystemByNameNoPlanetPics(StarSystem.GetSystemNameFromPlanet(holding.Location));
                        
                    if (starSystem == null)
                    {
                        string systemName = StarSystem.GetSystemNameFromPlanet(holding.Location);

                        Planet planet = new Planet(holding,0, 0, true, false, holding.Morale.ToString(), holding.Location, holding.Owner,holding.PlanetType, holding.Population.ToString(), null, null);
                        List<Planet> planets = new List<Planet>();
                        planets.Add(planet);

                        starSystem = new StarSystem(systemName,
                            0,
                            planets,
                            null,
                            new List<string>(),
                            null,
                            null,
                            null,
                            null,
                            new Coordinate(holding.GalaxyX, holding.GalaxyY)
                            );
                    }
                    else
                    {
                        Planet planet = starSystem.Planets.Find(p => p.Name == holding.Location);
                        if (planet == null)
                        {
                            planet = new Planet(holding, 0, 0, true, false, holding.Morale.ToString(), holding.Location, holding.Owner,holding.PlanetType, holding.Population.ToString(), null, null);

                            starSystem.Planets.Add(planet);
                        }
                    }
                    starSystems.Add(starSystem);
                }

                foreach (StarSystem starSystem in starSystems)
                {
                    if (starSystem.Planets != null)
                    {
                        if (starSystem.Planets.Count > 0)
                        {
                            foreach (Planet planet in starSystem.Planets)
                            {
                                Holding holding = holdings.Find(x => x.Location == planet.Name);

                                if (holding != null)
                                {
                                    planet.Holding = holding;
                                }
                            }
                        }
                    }
                    UpdateResult result = UpdateStarSystem(starSystem);
                }

                System.IO.File.Delete(tempFile);
                Program.Logs.Add(new LogMessage("RunUpdateGalaxyColonies", MessageType.Success, "Ran to completion."));
            }
            catch (System.Exception exc)
            {
                System.IO.File.Delete(tempFile);
                Program.Logs.Add(new LogMessage("RunUpdateGalaxyColonies", MessageType.Error, exc.ToString()));
            }
        }

        public UpdateResult UpdateStarSystem(StarSystem starSystem)
        {
            UpdateResult result = null;
            try
            {
                if (GetSystemByName(starSystem.Name) == null)
                {
                    InsertStarSystem(starSystem);
                }

                List<Planet> planetsWPictures = starSystem.Planets.FindAll(p => p.Picture != null);
                if (planetsWPictures.Count > 0)
                {
                    new FileModel(databaseName, Settings.Configuration["MongoDB:Databases:Collections:pictures"]).UpdatePictures(planetsWPictures);
                }
                foreach (Planet planet in starSystem.Planets)
                {
                    planet.Picture = null;
                }

                UpdateDefinition<StarSystem> updateDefinition = Builders<StarSystem>.Update
                    .Set(y => y.Name, starSystem.Name.Trim())
                    .Set(y => y.MiniMap , starSystem.MiniMap)
                    .Set(y => y.ConnectedSystems , starSystem.ConnectedSystems)
                    .Set(y => y.Coordinates , starSystem.Coordinates)
                    .Set(y => y.CurrentDefenses , starSystem.CurrentDefenses)
                    .Set(y => y.StarColor , starSystem.StarColor)
                    .Set(y => y.StarType , starSystem.StarType)
                    .Set(y => y.Picture , starSystem.Picture)
                    .Set(y => y.Port , starSystem.Port)
                    .Set(y => y.Planets , starSystem.Planets)
                    ;

                result = collection.UpdateOne(
                      x => x.Name.Equals(starSystem.Name),
                      updateDefinition);

            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("UpdateStarSystem", MessageType.Error, e.ToString()));
            }

            return result;
        }

        public UpdateResult UpdatePlanet(Planet planet)
        {
            UpdateResult result = null;
            planet.Name = planet.Name.Trim();
            try
            {
                // if doens't exist Create it
                string systemName = StarSystem.GetSystemNameFromPlanet(planet.Name);
                StarSystem starSystem = GetSystemByNameNoPlanetPics(systemName);
                FileModel fileModel = new FileModel(databaseName, Settings.Configuration["MongoDB:Databases:Collections:pictures"]);
                //if star system doesn't exist create it
                if (starSystem == null)
                {
                    starSystem = new StarSystem();

                    if(planet.Picture != null)
                    {
                        fileModel.UpdateFile(planet.Picture);
                        planet.Picture = null;
                    }
                   
                    starSystem.Planets = new List<Planet> { planet };
                
                    starSystem = new StarSystem(systemName, 0, starSystem.Planets, null, null, "", "", null, null, null);
                    result = UpdateStarSystem(starSystem);
                }
                else
                {
                    Planet planetExist = starSystem.Planets.Find(p => p.Name == planet.Name);

                    //if planet doens't exist add to star system
                    if (planetExist == null)
                    {
                        starSystem.Planets.Add(planet);

                        //if the planet has a picture sent in with it send it to collection
                        if (planet.Picture != null)
                        {
                            fileModel.UpdateFile(planet.Picture);

                            //we're gonna go ahead and null it out now
                            planet.Picture = null;
                        }
                        UpdateStarSystem(starSystem);
                    }
                    else
                    { 
                        //if the planet has a picture sent in with it send it to collection
                        if (planet.Picture != null)
                        {
                            fileModel.UpdateFile(planet.Picture);

                            //we're gonna go ahead and null it out now
                            planet.Picture = null;
                        }

                        Coordinate coords = new Coordinate();
                        //if it has a holding
                        if (planet.Holding != null)
                        {
                            coords.X = planet.Holding.GalaxyX;
                            coords.Y = planet.Holding.GalaxyY;

                            if(starSystem.Coordinates == null)
                            {
                                starSystem.Coordinates = coords;
                            }
                        }

                        //if planet exists get system it belongs to and overwrite planet
                        int index = starSystem.Planets.FindIndex(p => p.Name == planet.Name);
                        starSystem.Planets[index] = planet;
                        result = UpdateStarSystem(starSystem);
                    }
                }
                
            }
            catch (System.Exception e)
            {
                Program.Logs.Add(new LogMessage("UpdatePlanet", MessageType.Error, e.ToString()));
            }

            return result;
        }

        public void StartGalaxyUpdates(object hollerback)
        {
            Program.Logs.Add(new LogMessage("StartGalaxyUpdates", MessageType.Message, "Starting Galaxy Updates"));
            FileModel fileModel = new FileModel(databaseName, Settings.Configuration["MongoDB:Databases:Collections:csv"]);

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
                System.Console.WriteLine($"Attempting holdings_{dateTime.Year}{month}{day}.csv in StartGalaxyUpdates");
                files = fileModel.GetCsv($"holdings_{dateTime.Year}{month}{day}.csv");
            }
            RunUpdateGalaxyColonies(files[0].FileContents);
        }
    }
}