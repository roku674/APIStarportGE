
//Created by Alexander Fields 

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.IO;
using Optimization.Encryption;
using Optimization.Objects;
using Optimization.Objects.Logging;
using Optimization.Utility;
using StarportObjects;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace APIStarportGE
{
    internal class Program
    {
        private static int retries = 0;

        public static string configJson = Directory.GetCurrentDirectory() + "/config.json";
        public static List<LogMessage> Logs { get; set; }

        public static void Main(string[] args)
        {
            LogMessage.MessageSourceSetter = "APIStarportGE";
            Logs = new List<LogMessage>();
            string configContents = File.ReadAllText(configJson);
            Settings.BuildAndSetConfig(configJson);

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            WebApplication app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            System.Console.WriteLine(LogMessage.MessageSourceSetter + " is now running!");
            Logs.Add(new LogMessage("Main", MessageType.Success, LogMessage.MessageSourceSetter + " Successfully booted up!"));

            try
            {
                app.Run();
            }
            catch(System.Exception ex)
            {
                Logs.Add(new LogMessage("Main", MessageType.Critical, $"{LogMessage.MessageSourceSetter} has failed on attempt {retries} attempting to restart! {ex}"));
                retries++;
                Main(args);
            }

        }
    }
}