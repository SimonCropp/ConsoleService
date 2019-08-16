using System;
using System.Threading;
using System.Threading.Tasks;
using ApprovalTests;
using ConsoleService;
using Xunit;
using Xunit.Abstractions;

public class OverridesTests :
    XunitLoggingBase
{
    [Fact]
    public void Throw_for_onStart()
    {
        var exception = Assert.Throws<Exception>(() => new BadStart());
        Approvals.Verify(exception.Message);
    }

    public class BadStart : ProgramService
    {
        protected override void OnStart(string[] args)
        {
        }

        protected override Task OnStartAsync(string[] args, CancellationToken cancellation)
        {
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync()
        {
            return Task.CompletedTask;
        }
    }

    [Fact]
    public void Throw_for_onStop()
    {
        var exception = Assert.Throws<Exception>(() => new BadStop());
        Approvals.Verify(exception.Message);
    }

    public class BadStop : ProgramService
    {
        protected override void OnStop()
        {
        }

        protected override Task OnStartAsync(string[] args, CancellationToken cancellation)
        {
            return Task.CompletedTask;
        }

        protected override Task OnStopAsync()
        {
            return Task.CompletedTask;
        }
    }

    public OverridesTests(ITestOutputHelper output) :
        base(output)
    {
    }
}