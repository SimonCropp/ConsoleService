using System;
using System.ComponentModel;
using System.Reflection;
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

        public ProgramService()
        {
            var type = GetType();
            var bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            var onStart = type.GetMethod("OnStart", bindingFlags, null, new[] {typeof(string[])}, null);
            EnsureNotOverriden(onStart);
            var onStop = type.GetMethod("OnStop", bindingFlags);
            EnsureNotOverriden(onStop);
        }

        static void EnsureNotOverriden(MethodInfo method)
        {
            if (method.DeclaringType != typeof(ProgramService))
            {
                throw new Exception("Do not override OnStart or OnStop. Instead place that code in OnStartAsync and OnStopAsync");
            }
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