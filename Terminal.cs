using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace UtilitiesManager
{
    public static class TerminalCommands
    {
        public static int RunSilent(string command, int timeoutMilliseconds = Timeout.Infinite)
        {
            var psi = BuildStartInfo(command);

            // We MUST capture output now
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            using var process = new Process { StartInfo = psi };

            process.Start();

            string output = process.StandardOutput.ReadToEnd().Trim();

            bool exited = process.WaitForExit(timeoutMilliseconds);
            if (!exited)
            {
                try { process.Kill(true); } catch { }
                return -1;
            }

            // Try to parse the output as an int
            if (int.TryParse(output, out int value))
                return value;

            return -1; // failed to parse
        }

        public static Task<int> RunSilentAsync(string command, int timeoutMilliseconds = Timeout.Infinite)
        {
            return Task.Run(() => RunSilent(command, timeoutMilliseconds));
        }

        private static ProcessStartInfo BuildStartInfo(string command)
        {
            if (OperatingSystem.IsWindows())
            {
                return new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c " + command,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
            else
            {
                string escaped = command.Replace("\"", "\\\"");
                return new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escaped}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
            }
        }
    }

    // Class for reading brightness
    public class BrightnessCommand
    {
        public int Run(int timeoutMs = Timeout.Infinite)
        {
            return TerminalCommands.RunSilent("brightnessctl get", timeoutMs);
        }

        public Task<int> RunAsync(int timeoutMs = Timeout.Infinite)
        {
            return TerminalCommands.RunSilentAsync("brightnessctl get", timeoutMs);
        }
    }

    // Class for reading default sink volume via pactl
    public class VolumeCommand
    {
        public int Run(int timeoutMs = Timeout.Infinite)
        {
            return TerminalCommands.RunSilent("pactl get-sink-volume @DEFAULT_SINK@", timeoutMs);
        }

        public Task<int> RunAsync(int timeoutMs = Timeout.Infinite)
        {
            return TerminalCommands.RunSilentAsync("pactl get-sink-volume @DEFAULT_SINK@", timeoutMs);
        }
    }
}
