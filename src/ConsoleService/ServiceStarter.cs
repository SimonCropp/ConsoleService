using System;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ConsoleService
{
    public static class ServiceStarter
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
                Console.WriteLine($"{service.ServiceName} - Press Ctrl-C to Exit");
                await service.StartAsync(Environment.GetCommandLineArgs());
                ServiceHelper.BlockUntilControlC();
                await service.StopAsync();
            }
        }
    }
}