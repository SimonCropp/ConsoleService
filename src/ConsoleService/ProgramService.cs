using System;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleService
{
    [DesignerCategory("Code")]
    public abstract class ProgramService :
        ServiceBase
    {
        Task startAsync;
        Task stopAsync;
        internal CancellationTokenSource tokenSource = new CancellationTokenSource();
        ManualResetEvent startResetEvent = new ManualResetEvent(false);
        ManualResetEvent stopResetEvent = new ManualResetEvent(false);
        ExceptionDispatchInfo startException;
        ExceptionDispatchInfo stopException;

        protected ProgramService()
        {
            OverrideValidator.Validate(this);
            ServiceName = GetType().Assembly.GetName().Name;
        }

        protected override void OnStart(string[] args)
        {
            startAsync = StartAsync(args);
            startResetEvent.WaitOne();
            startException?.Throw();
        }

        async Task StartAsync(string[] args)
        {
            try
            {
                await OnStartAsync(args, tokenSource.Token);
            }
            catch (Exception exception)
            {
                startException = ExceptionDispatchInfo.Capture(exception);
            }

            startResetEvent.Set();
        }

        protected internal abstract Task OnStartAsync(string[] args, CancellationToken cancellation);

        protected override void OnStop()
        {
            tokenSource.Cancel();
            stopAsync = StopAsync();
            stopResetEvent.WaitOne();
            stopException?.Throw();
        }

        protected internal abstract Task OnStopAsync();

        async Task StopAsync()
        {
            try
            {
                await OnStopAsync();
            }
            catch (Exception exception)
            {
                stopException = ExceptionDispatchInfo.Capture(exception);
            }

            stopResetEvent.Set();
        }

        protected override void Dispose(bool disposing)
        {
            startResetEvent.Dispose();
            stopResetEvent.Dispose();
            startAsync?.Dispose();
            stopAsync?.Dispose();
            base.Dispose(disposing);
        }
    }
}