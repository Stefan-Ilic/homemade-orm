using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;

namespace IntegrationTests
{
    public abstract class Docker
    {
        private static List<string> _runningContainers = new List<string>();

        public static void Start(string containerName)
        {
            if (_runningContainers.Contains(containerName))
            {
                return;
            }
            var processInfo = new ProcessStartInfo("docker", $"start {containerName}");

            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            int exitCode;
            using (var process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit(1200000);
                if (!process.HasExited)
                {
                    process.Kill();
                }

                exitCode = process.ExitCode;
                process.Close();
            }
            Thread.Sleep(1000);
            _runningContainers.Remove(containerName);
        }

        public static void Stop(string containerName)
        {
            if (!_runningContainers.Contains(containerName))
            {
                return;
            }
            var processInfo = new ProcessStartInfo("docker", $"stop {containerName}");

            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;

            int exitCode;
            using (var process = new Process())
            {
                process.StartInfo = processInfo;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit(1200000);
                if (!process.HasExited)
                {
                    process.Kill();
                }

                exitCode = process.ExitCode;
                process.Close();
            }
            Thread.Sleep(1000);
        }
    }
}
