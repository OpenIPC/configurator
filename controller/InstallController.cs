using System;
using System.Diagnostics;

namespace Configurator.Controller
{
    public class InstallController
    {
        public void InstallDependencies()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c dotnet restore",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
                string result = process.StandardOutput.ReadToEnd();
                Console.WriteLine(result);
            }
        }
    }
}
