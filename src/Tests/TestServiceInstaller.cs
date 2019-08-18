// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

namespace System.ServiceProcess.Tests
{
    public class TestServiceInstaller
    {
        public TestServiceInstaller()
        {
        }

        public string DisplayName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string[] ServicesDependedOn { get; set; } = Array.Empty<string>();

        public string ServiceName { get; set; } = string.Empty;

        public ServiceStartMode StartType { get; set; } = ServiceStartMode.Manual;

        public string Username { get; set; }

        public string Password { get; set; }

        public string ServiceCommandLine { get; set; }

        public void Install()
        {
            SampleInstaller.Create(ServiceName, SampleLocation.TestsAssembly);
            using (var svc = new ServiceController(ServiceName))
            {
                if (svc.Status != ServiceControllerStatus.Running)
                {
                    svc.Start();
                    if (!ServiceName.StartsWith("PropagateExceptionFromOnStart"))
                        svc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(120));
                }
            }
        }

        public void RemoveService()
        {
            try
            {
                StopService();
            }
            finally
            {
                // If the service didn't stop promptly, we will get a TimeoutException.
                // This means the test service has gotten "jammed".
                // Meantime we still want this service to get deleted, so we'll go ahead and call
                // DeleteService, which will schedule it to get deleted on reboot.
                // We won't catch the exception: we do want the test to fail.

                DeleteService();

                ServiceName = null;
            }
        }

        private void StopService()
        {
            using (var svc = new ServiceController(ServiceName))
            {
                // The Service exists at this point, but OpenService is failing, possibly because its being invoked concurrently for another service.
                // https://github.com/dotnet/corefx/issues/23388
                if (svc.Status != ServiceControllerStatus.Stopped)
                {
                    try
                    {
                        svc.Stop();
                    }
                    catch (InvalidOperationException)
                    {
                        // Already stopped
                        return;
                    }

                    svc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(120));
                }
            }
        }

        private void DeleteService()
        {
            SampleInstaller.Delete(ServiceName);
        }
    }
}
