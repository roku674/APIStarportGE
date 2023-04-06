
//Created by Alexander Fields 

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Libmongocrypt;
using Optimization.Objects;
using Optimization.Objects.Logging;
using Optimization.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace APIStarportGE
{
    internal class Program
    {
        public static bool isCancelled { get;set; }
        public static string configJson = Directory.GetCurrentDirectory() + "/config.json";
        public static List<LogMessage> Logs { get; set; }
        public static DateTime deployTime { get; set; }

        public static void Main(string[] args)
        {
            deployTime = DateTime.Now;
            LogMessage.MessageSourceSetter = "APIStarportGE";
            Logs = new List<LogMessage>();
            string configContents = System.IO.File.ReadAllText(configJson);
            Settings.BuildAndSetConfig(configJson);
            isCancelled = true;

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
            
            ThreadPool.QueueUserWorkItem(Updater);

            try
            {
                app.Run();
            }
            catch(System.Exception ex)
            {
                Logs.Add(new LogMessage("Main", MessageType.Critical, $"{LogMessage.MessageSourceSetter} has failed! {ex}"));

                SendWebhook(new HttpClient(), Newtonsoft.Json.JsonConvert.SerializeObject(new LogMessage("Main", MessageType.Critical, $"{LogMessage.MessageSourceSetter} has failed! {ex}")));

                EmailLogsAndClear();
            }
        }

        internal static void EmailLogsAndClear()
        {
            Mailing messaging = new Mailing();

            string emailBody = "<h2>Nothing to report!</h2>";

            if (Logs.Count > 0)
            {
                for (int i = 0;i < Logs.Count;i++)
                {
                    Logs[i].Id = i;
                }

                emailBody = Utility.ConvertDataTableToHTML(Utility.ConvertListToDataTable(Logs), 3, 2, 1, null);
            }

            SmtpClient client = messaging.CreateSmtpClient(
                Settings.Configuration["Smtp:server"],
                Settings.Configuration["Smtp:email"],
                Utility.DecodeString(Settings.Configuration["Smtp:password"]),
                int.Parse(Settings.Configuration["Smtp:port"]));

            MailMessage email = messaging.CreateEmail(
                Settings.Configuration["Smtp:to"],
                Settings.Configuration["Smtp:email"],
                "Logging Updates",
                emailBody,
                true,
                new List<string>()
                );

            client.Send(email);

            Logs = new List<LogMessage>();
        }

        internal static string SendWebhook(HttpClient client, string message)
        {
            message = message.Replace("\"", "").Replace("'", "").Replace("\\", "");
            // Create the message payload
            string json = "{ \"content\": \"" + message + "\" }";

            return APICaller.PostJson(
                       client,
                       Logs,
                       json,
                       $"{Settings.Configuration["Webhooks:api"]}"
                       ).Result;
        }

        private async static void Updater(object state)
        {
            isCancelled = false;
            System.Console.WriteLine("Updater is running...");

            while(System.DateTime.Now.Minute != 0)
            {
                await Task.Delay(60000);
            }

            while (true)
            {
                if (isCancelled)
                {
                    break;
                }
                _ = Task.Run(() => EmailLogsAndClear());
                await Task.Delay(System.TimeSpan.FromHours(1));
            }
        }

    }    
}