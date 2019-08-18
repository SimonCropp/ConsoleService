// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceProcess;
using System.ServiceProcess.Tests;
using Xunit;

// NOTE: All tests checking the output file should always call Stop before checking because Stop will flush the file to disk.
public class ServiceBaseTests : IDisposable
{
    private const int connectionTimeout = 30000;
    private readonly TestServiceProvider _testService;

    public ServiceBaseTests()
    {
        _testService = new TestServiceProvider();
        SampleInstaller.Delete(_testService.TestServiceName);
    }

    private void AssertExpectedProperties(ServiceController testServiceController)
    {
        Assert.Equal(_testService.TestServiceName, testServiceController.ServiceName);
        Assert.Equal(_testService.TestServiceDisplayName, testServiceController.DisplayName);
        Assert.Equal(_testService.TestMachineName, testServiceController.MachineName);
        Assert.Equal(ServiceType.Win32OwnProcess, testServiceController.ServiceType);
        Assert.True(testServiceController.CanPauseAndContinue);
        Assert.True(testServiceController.CanStop);
        Assert.True(testServiceController.CanShutdown);
    }

    // [Fact]
    // To cleanup lingering Test Services uncomment the Fact attribute, make it public and run the following command
    //   msbuild /t:rebuildandtest /p:XunitMethodName=System.ServiceProcess.Tests.ServiceBaseTests.Cleanup /p:OuterLoop=true
    // Remember to comment out the Fact again before running tests otherwise it will cleanup tests running in parallel
    // and cause them to fail.
    private void Cleanup()
    {
        foreach (var controller in ServiceController.GetServices())
        {
            try
            {
                var currentService = controller.DisplayName;
                if (controller.DisplayName.StartsWith("Test Service"))
                {
                    Console.WriteLine($"Trying to clean-up {currentService}");
                    var deleteService = new TestServiceInstaller
                    {
                        ServiceName = controller.ServiceName
                    };
                    deleteService.RemoveService();
                    Console.WriteLine("Cleaned up " + currentService);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed " + ex.Message);
            }
        }
    }

    [Fact]
    public void TestOnStartThenStop()
    {
        var controller = ConnectToServer();

        controller.Stop();
        Assert.Equal((int) PipeMessageByteCode.Stop, _testService.GetByte());
        controller.WaitForStatus(ServiceControllerStatus.Stopped);
    }

    [Fact]
    public void TestOnStartWithArgsThenStop()
    {
        var controller = ConnectToServer();

        controller.Stop();
        Assert.Equal((int) PipeMessageByteCode.Stop, _testService.GetByte());
        controller.WaitForStatus(ServiceControllerStatus.Stopped);

        controller.Start(new string[] {"StartWithArguments", "a", "b", "c"});
        _testService.Client = null;
        _testService.Client.Connect();

        // There is no definite order between start and connected when tests are running on multiple threads.
        // In this case we dont care much about the order, so we are just checking whether the appropiate bytes have been sent.
        Assert.Equal((int) (PipeMessageByteCode.Connected | PipeMessageByteCode.Start), _testService.GetByte() | _testService.GetByte());
        controller.WaitForStatus(ServiceControllerStatus.Running);

        controller.Stop();
        Assert.Equal((int) PipeMessageByteCode.Stop, _testService.GetByte());
        controller.WaitForStatus(ServiceControllerStatus.Stopped);
    }

    [Fact]
    public void TestOnPauseThenStop()
    {
        var controller = ConnectToServer();

        controller.Pause();
        Assert.Equal((int) PipeMessageByteCode.Pause, _testService.GetByte());
        controller.WaitForStatus(ServiceControllerStatus.Paused);

        controller.Stop();
        Assert.Equal((int) PipeMessageByteCode.Stop, _testService.GetByte());
        controller.WaitForStatus(ServiceControllerStatus.Stopped);
    }

    [Fact]
    public void TestOnPauseAndContinueThenStop()
    {
        var controller = ConnectToServer();

        controller.Pause();
        Assert.Equal((int) PipeMessageByteCode.Pause, _testService.GetByte());
        controller.WaitForStatus(ServiceControllerStatus.Paused);

        controller.Continue();
        Assert.Equal((int) PipeMessageByteCode.Continue, _testService.GetByte());
        controller.WaitForStatus(ServiceControllerStatus.Running);

        controller.Stop();
        Assert.Equal((int) PipeMessageByteCode.Stop, _testService.GetByte());
        controller.WaitForStatus(ServiceControllerStatus.Stopped);
    }

    [Fact]
    public void TestOnExecuteCustomCommand()
    {
        var controller = ConnectToServer();

        controller.ExecuteCommand(128);
        Assert.Equal(128, _testService.GetByte());

        controller.Stop();
        Assert.Equal((int) PipeMessageByteCode.Stop, _testService.GetByte());
        controller.WaitForStatus(ServiceControllerStatus.Stopped);
    }

    [Fact]
    public void TestOnContinueBeforePause()
    {
        var controller = ConnectToServer();

        controller.Continue();
        controller.WaitForStatus(ServiceControllerStatus.Running);

        controller.Stop();
        Assert.Equal((int) PipeMessageByteCode.Stop, _testService.GetByte());
        controller.WaitForStatus(ServiceControllerStatus.Stopped);
    }

    [Fact]
    public void PropagateExceptionFromOnStart()
    {
        var serviceName = nameof(PropagateExceptionFromOnStart) + Guid.NewGuid().ToString();
        var testService = new TestServiceProvider(serviceName);
        testService.Client.Connect(connectionTimeout);
        Assert.Equal((int) PipeMessageByteCode.Connected, testService.GetByte());
        Assert.Equal((int) PipeMessageByteCode.ExceptionThrown, testService.GetByte());
        testService.DeleteTestServices();
    }

    ServiceController ConnectToServer()
    {
        _testService.Client.Connect(connectionTimeout);
        Assert.Equal((int) PipeMessageByteCode.Connected, _testService.GetByte());

        var controller = new ServiceController(_testService.TestServiceName);
        AssertExpectedProperties(controller);
        return controller;
    }

    public void Dispose()
    {
        _testService.DeleteTestServices();
    }
}

public enum PipeMessageByteCode
{
    Start,
    Continue,
    Pause,
    Stop,
    OnCustomCommand,
    ExceptionThrown,
    Connected

}