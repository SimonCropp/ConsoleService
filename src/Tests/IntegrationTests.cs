using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

public class IntegrationTests :
    XunitLoggingBase
{
    [Fact]
    public void RunAsConsole()
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{SampleLocation.SampleAssembly}\"",
        };
        using (var process = Process.Start(startInfo))
        {
            var thread = new Thread(() =>
            {
                Thread.Sleep(500);
                process.Kill();
            });
            thread.Start();
            process.WaitForExit();
        }
    }

    //[Fact]
    //public void Install()
    //{
    //    SampleInstaller.Delete();
    //    SampleInstaller.Create();
    //}
    [Fact]
    public void RunAsService()
    {
        SampleInstaller.Delete();
        SampleInstaller.Create();
        using (var controller = GetController())
        {
            Assert.Equal(ServiceControllerStatus.Stopped, controller.Status);

            controller.Start();
            controller.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
            controller.Refresh();
            Assert.Equal(ServiceControllerStatus.Running, controller.Status);

            controller.Stop();
            controller.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15));
            controller.Refresh();
            Assert.Equal(ServiceControllerStatus.Stopped, controller.Status);

            SampleInstaller.Delete();
        }
    }

    private static ServiceController GetController()
    {
        return ServiceController.GetServices().SingleOrDefault(x => x.ServiceName == "WindowsServiceSample");
    }

    public IntegrationTests(ITestOutputHelper output) :
        base(output)
    {
    }
}