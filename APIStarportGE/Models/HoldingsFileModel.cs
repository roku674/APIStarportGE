//Created by Alexander Fields 

using MongoDB.Driver;
using Optimization.Objects;
using StarportObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace APIStarportGE.Models
{
    public class HoldingsFileModel
    {
        private readonly IMongoCollection<FileObj> collection;
        private readonly IMongoDatabase database;
        private readonly string databaseName;

        public HoldingsFileModel(string _databaseName)
        {
            databaseName = _databaseName;
            string csv = Settings.Configuration["MongoDB:Databases:Collections:csv"];

            database = new Repository.Repository().EstablishConnection().GetDatabase(databaseName);
            if (database != null)
            {
                collection = database.GetCollection<FileObj>(csv);
            }
            else
            {
                System.Console.WriteLine("ERROR: database was not found!");
            }
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

        public bool InsertFile(FileObj file)
        {
            return Repository.Repository.Insert<FileObj>(collection, file);
        }

        public UpdateResult UpdateCsv(FileObj csv)
        {
            UpdateResult result = null;
            try
            {
                if (GetCsv(csv.FileName).Count == 0)
                {
                    InsertFile(csv);
                }

                UpdateDefinition<FileObj> updateDefinition = Builders<FileObj>.Update
                    .Set(y => y.FileName, csv.FileName)
                    .Set(y => y.FileExtension, csv.FileExtension)
                    .Set(y => y.FileContents, csv.FileContents)
                    .Set(y => y.FileBytes, csv.FileBytes)
                    .Set(y => y.CreatedDate, csv.CreatedDate)
                    .Set(y => y.LastUpdated, csv.LastUpdated)
                    .Set(y => y.FileSize, csv.FileSize)
                    .Set(y => y.FolderStructure, csv.FolderStructure)
                    .Set(y => y.Directory, csv.Directory)
                    ;

                result = collection.UpdateOne(
                      x => x.FileName.Equals(csv.FileName),
                      updateDefinition);

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
            }

            return result;
        }

    }
}
