using System;
using System.Diagnostics;
using System.Text;

static class SampleInstaller
{
    public static void Create(string name, string assembly)
    {
        Run($"create {name} binpath= \"dotnet {assembly}\"");
    }

    public static void Start()
    {
        Run("start WindowsServiceSample");
    }
    public static void Stop()
    {
        Run("stop WindowsServiceSample");
    }

    public static void Delete(string name)
    {
        Run($"delete {name}");
    }

    static void Run(string arguments)
    {
        var errorBuilder = new StringBuilder();
        var outputBuilder = new StringBuilder();
        using (var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "sc",
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false,
            }
        })
        {
            process.Start();
            process.OutputDataReceived += (sender, args) => { outputBuilder.AppendLine(args.Data); };
            process.BeginOutputReadLine();
            process.ErrorDataReceived += (sender, args) => { errorBuilder.AppendLine(args.Data); };
            process.BeginErrorReadLine();
            if (!process.WaitForExit(500))
            {
                var timeoutError = $@"Process timed out. Command line: sc {arguments}.
Output: {outputBuilder}
Error: {errorBuilder}";
                throw new Exception(timeoutError);
            }
            if (process.ExitCode == 0)
            {
                return;
            }

            var error = $@"Could not execute process. Command line: sc {arguments}.
Output: {outputBuilder}
Error: {errorBuilder}";
            throw new Exception(error);
        }
    }
}