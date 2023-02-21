//Created by Alexander Fields 

using MongoDB.Bson;
using MongoDB.Driver;
using Optimization.Objects;
using Optimization.Objects.Logging;
using StarportObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace APIStarportGE.Models
{
    public class FileModel
    {
        private readonly IMongoCollection<FileObj> collection;
        private readonly IMongoDatabase database;
        private readonly string databaseName;

        public FileModel(string _databaseName, string collectionName)
        {
            databaseName = _databaseName;
            string file = collectionName;

            database = new Repository.Repository().EstablishConnection().GetDatabase(databaseName);
            if (database != null)
            {
                collection = database.GetCollection<FileObj>(file);
            }
            else
            {
                System.Console.WriteLine("ERROR: database was not found!");
                Program.Logs.Add(new LogMessage("HoldingsModel", MessageType.Critical, "Database Not Found!"));
                Program.SendWebhook(new HttpClient(), Newtonsoft.Json.JsonConvert.SerializeObject(new LogMessage("ColonyModel", MessageType.Critical, "Database Not Found!")));
            }
        }

        public List<FileObj> GetAllPictures()
        {
            return collection.Find(new BsonDocument()).ToList();
        }

        public DeleteResult DeleteCsv(string name)
        {
            DeleteResult result = null;
            try
            {
                collection.DeleteMany(Builders<FileObj>.Filter.Eq(f => f.FileName, name));
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
            }
            return result;
        }

        public List<FileObj> GetCsv(string name)
        {
            List<FileObj> csvs = new List<FileObj>();
            try
            {
                csvs = collection.AsQueryable().Where(d => d.FileName == name).ToList();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
            }
            return csvs;
        }
        public List<FileObj> GetCsv(DateTime date)
        {
            List<FileObj> csvs = new List<FileObj>();
            try
            {
                csvs = collection.AsQueryable().Where(d => d.CreatedDate.Date == date.Date).ToList();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
            }
            return csvs;
        }

        public FileObj GetFile(string name)
        {
            FileObj picture = null;
            try
            {
                picture = collection.Find(d => d.FileName == name).FirstOrDefault();
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
            }
            return picture;
        }

        public bool InsertFile(FileObj file)
        {
            return Repository.Repository.Insert<FileObj>(collection, file);
        }

        public UpdateResult UpdateFile(FileObj file)
        {
            UpdateResult result = null;
            try
            {
                if (GetFile(file.FileName) == null)
                {
                    InsertFile(file);
                }

                UpdateDefinition<FileObj> updateDefinition = Builders<FileObj>.Update
                    .Set(y => y.FileName, file.FileName)
                    .Set(y => y.FileExtension, file.FileExtension)
                    .Set(y => y.FileContents, file.FileContents)
                    .Set(y => y.FileBytes, file.FileBytes)
                    .Set(y => y.CreatedDate, file.CreatedDate)
                    .Set(y => y.LastUpdated, file.LastUpdated)
                    .Set(y => y.FileSize, file.FileSize)
                    .Set(y => y.FolderStructure, file.FolderStructure)
                    .Set(y => y.Directory, file.Directory)
                    ;

                result = collection.UpdateOne(
                      x => x.FileName.Equals(file.FileName),
                      updateDefinition);

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
            }

            return result;
        }

        public List<UpdateResult> UpdatePictures(List<Planet> planetsWPictures)
        {
            List<UpdateResult> results = new List<UpdateResult>();

            foreach (Planet planet in planetsWPictures)
            {
                try
                {
                    planet.Picture.FileName = Path.ChangeExtension(
                            planet.Picture.FileName,
                            Path.GetExtension(planet.Picture.FileName).ToLower()
                            );

                    if (GetFile(planet.Picture.FileName) == null)
                    {                
                        InsertFile(planet.Picture);
                    }

                    UpdateResult result = UpdateFile(planet.Picture);

                    results.Add(result);
                }
                catch (System.Exception e)
                {
                    System.Console.WriteLine($"Failed to register complete with {e}");
                }
            }

            return results;
        }
    }
}
