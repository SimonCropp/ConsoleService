﻿using System;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ConsoleService
{
    public abstract class ServiceStarter
    {
        public static async Task Start<T>()
            where T : ProgramService, new()
        {
            using (var service = new T())
            {
                if (ServiceHelper.IsService())
                {
                    ServiceBase.Run(service);
                    return;
                }

                Console.Title = service.ServiceName;
                Console.WriteLine("Press Ctrl-C to Exit");
                await service.OnStartAsync(Environment.GetCommandLineArgs(), service.tokenSource.Token);
                ServiceHelper.BlockUntilControlC();
                await service.OnStopAsync();
            }
        }
    }
}