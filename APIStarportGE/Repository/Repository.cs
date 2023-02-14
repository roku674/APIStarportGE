//Copyright © 2022, Perilous Games, Ltd. All rights reserved.

using MongoDB.Driver;
using Optimization.Encryption;
using Optimization.Objects;
using System.Collections.Generic;

namespace APIStarportGE.Repository
{
    public class Repository
    { 
        public MongoClient EstablishConnection()
        {
            string pw = AES.DecryptString(Settings.Configuration["MongoDB:key"],Settings.Configuration["MongoDB:password"]);
            MongoClientSettings settings = null;
            try
            {
                settings = MongoClientSettings.FromConnectionString("mongodb+srv://" + Settings.Configuration["MongoDB:username"] + ":" + pw + "@"+Settings.Configuration["MongoDB:Cluster"]+ ".1jcquzs.mongodb.net/?retryWrites=true&w=majority");
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to set MongoDB with {e}");
            }

            if (settings == null)
            {
                return null;
            }

            return new MongoClient(settings);
        }

        public static bool Insert<T>(IMongoCollection<T> collection, T obj)
        {
            try
            {
                collection.InsertOne(obj);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
                return false;
            }
            return true;
        }


        public static bool Insert<T>(IMongoCollection<T> collection, List<T> obj)
        {
            try
            {
                collection.InsertMany(obj);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine($"Failed to register complete with {e}");
                return false;
            }
            return true;
        }
    }
}