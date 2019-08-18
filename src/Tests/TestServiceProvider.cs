// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading.Tasks;

namespace System.ServiceProcess.Tests
{
    internal sealed class TestServiceProvider
    {
        private const int readTimeout = 60000;
        public const string LocalServiceName = "NT AUTHORITY\\LocalService";

        private static readonly Lazy<bool> s_runningWithElevatedPrivileges = new Lazy<bool>(
            () => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator));

        private NamedPipeClientStream _client;

        public static bool RunningWithElevatedPrivileges
        {
            get { return s_runningWithElevatedPrivileges.Value; }
        }

        public NamedPipeClientStream Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new NamedPipeClientStream(".", TestServiceName, PipeDirection.In);
                }
                return _client;
            }
            set
            {
                if (value == null)
                {
                    _client.Dispose();
                    _client = null;
                }
            }
        }

        public readonly string TestServiceAssembly = typeof(TestService).Assembly.Location;
        public readonly string TestMachineName;
        public readonly TimeSpan ControlTimeout;
        public readonly string TestServiceName;
        public readonly string Username;
        public readonly string TestServiceDisplayName;

        private readonly TestServiceProvider _dependentServices;
        public TestServiceProvider()
        {
            TestMachineName = ".";
            ControlTimeout = TimeSpan.FromSeconds(120);
            TestServiceName = Guid.NewGuid().ToString();
            TestServiceDisplayName = "Test Service " + TestServiceName;

            _dependentServices = new TestServiceProvider(TestServiceName + ".Dependent");

            // Create the service
            CreateTestServices();
        }

        public TestServiceProvider(string serviceName, string userName = LocalServiceName)
        {
            TestMachineName = ".";
            ControlTimeout = TimeSpan.FromSeconds(120);
            TestServiceName = serviceName;
            TestServiceDisplayName = "Test Service " + TestServiceName;
            Username = userName;

            // Create the service
            CreateTestServices();
        }

        public async Task<byte> ReadPipeAsync()
        {
            var received = new byte[] { 0 };
            Task readTask = Client.ReadAsync(received, 0, 1);
            await readTask.TimeoutAfter(readTimeout).ConfigureAwait(false);
            return received[0];
        }

        public byte GetByte() => ReadPipeAsync().Result;

        private void CreateTestServices()
        {
            var testServiceInstaller = new TestServiceInstaller
            {
                ServiceName = TestServiceName,
                DisplayName = TestServiceDisplayName,
                Description = "__Dummy Test Service__",
                Username = Username
            };

            if (_dependentServices != null)
            {
                testServiceInstaller.ServicesDependedOn = new[] { _dependentServices.TestServiceName };
            }

            var processName = Process.GetCurrentProcess().MainModule.FileName;
            var entryPointName = typeof(TestService).Assembly.Location;
            var arguments = TestServiceName;
            arguments = $"\"{entryPointName}\" {arguments}";

            testServiceInstaller.ServiceCommandLine = $"\"{processName}\" {arguments}";

            testServiceInstaller.Install();
        }

        public void DeleteTestServices()
        {
            try
            {
                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }

                var testServiceInstaller = new TestServiceInstaller
                {
                    ServiceName = TestServiceName
                };
                testServiceInstaller.RemoveService();
            }
            finally
            {
                // Lets be sure to try and clean up dependenct services even if something goes
                // wrong with the full removal of the other service.
                _dependentServices?.DeleteTestServices();
            }
        }
    }
}
