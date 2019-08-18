using System;
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
                await service.OnStartAsync(Environment.GetCommandLineArgs(), service.TokenSource.Token);
                ServiceHelper.BlockUntilControlC();
                await service.OnStopAsync();
            }
        }
    }
}
    public enum PowerBroadcastStatus
    {
        BatteryLow          = PowerBroadcastStatusInner.PBT_APMBATTERYLOW,
         OemEvent            = PowerBroadcastStatusInner.PBT_APMOEMEVENT,
         PowerStatusChange   = PowerBroadcastStatusInner.PBT_APMPOWERSTATUSCHANGE,
         QuerySuspend        = PowerBroadcastStatusInner.PBT_APMQUERYSUSPEND,
         QuerySuspendFailed  = PowerBroadcastStatusInner.PBT_APMQUERYSUSPENDFAILED,
         ResumeAutomatic     = PowerBroadcastStatusInner.PBT_APMRESUMEAUTOMATIC,
         ResumeCritical      = PowerBroadcastStatusInner.PBT_APMRESUMECRITICAL,
         ResumeSuspend       = PowerBroadcastStatusInner.PBT_APMRESUMESUSPEND,
         Suspend             = PowerBroadcastStatusInner.PBT_APMSUSPEND,
     }