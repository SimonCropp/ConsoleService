using System;
using System.ComponentModel;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleService
{
    [DesignerCategory("Code")]
    public abstract class ProgramService :
        ServiceBase
    {
        static TimeSpan DefaultServiceTimeout = TimeSpan.FromSeconds(30);
        CancellationTokenSource? startTokenSource;
        CancellationTokenSource? stopTokenSource;

        protected ProgramService()
        {
            ServiceName = GetType().Assembly.GetName().Name;
        }

        protected sealed override void OnStart(string[] args)
        {
            StartAsync(args).GetAwaiter().GetResult();
        }

        internal Task StartAsync(string[] args)
        {
            startTokenSource = new CancellationTokenSource(DefaultServiceTimeout);
            return OnStartAsync(args, startTokenSource.Token);
        }

        internal Task StopAsync()
        {
            stopTokenSource = new CancellationTokenSource(DefaultServiceTimeout);
            return OnStopAsync(stopTokenSource.Token);
        }

        protected abstract Task OnStartAsync(string[] args, CancellationToken cancellation);

        protected sealed override void OnStop()
        {
            StopAsync().GetAwaiter().GetResult();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            startTokenSource?.Dispose();
            stopTokenSource?.Dispose();
        }

        protected abstract Task OnStopAsync(CancellationToken cancellation);
    }
}