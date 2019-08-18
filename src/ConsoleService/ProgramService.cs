using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleService
{
    [DesignerCategory("Code")]
    public abstract class ProgramService :
        ServiceBase
    {
        internal CancellationTokenSource TokenSource = new CancellationTokenSource();

        protected ProgramService()
        {
            OverrideValidator.Validate(this);
            ServiceName = GetType().Assembly.GetName().Name;
        }

        protected override void OnStart(string[] args)
        {
            OnStartAsync(args, TokenSource.Token).GetAwaiter().GetResult();
        }

        protected internal abstract Task OnStartAsync(string[] args, CancellationToken cancellation);

        protected override void OnStop()
        {
            TokenSource.Cancel();
            OnStopAsync().GetAwaiter().GetResult();
        }

        protected internal abstract Task OnStopAsync();
    }
}