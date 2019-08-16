using System.Threading;
using System.Threading.Tasks;
using ConsoleService;

class MyService :
    ProgramService
{
    protected override Task OnStartAsync(string[] args, CancellationToken cancellation)
    {
        return Task.Delay(1000, cancellation);
    }

    protected override Task OnStopAsync()
    {
        return Task.Delay(2000);
    }
}