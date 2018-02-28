﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog.Web;
using System;
using System.IO;

namespace GenesisVision.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config")
                                    .GetCurrentClassLogger();

            try
            {
                logger.Debug("Init GenesisVision.Core");
                BuildWebHost(args).Run();
            }
            catch (Exception e)
            {
                logger.Fatal($"Application stopped: {e.Message} {Environment.NewLine}{e.StackTrace}");
                throw;
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseKestrel()
                   .UseIISIntegration()
                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseStartup<Startup>()
                   .UseNLog()
                   .Build();
    }
}
