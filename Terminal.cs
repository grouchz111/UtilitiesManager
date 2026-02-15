using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace UtilitiesManager
{
    public static class TerminalCommands
    {
        // Terminal helper
        public static async Task<string> RunCommandAsync(string command, int timeoutMs = Timeout.Infinite)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = new Process { StartInfo = psi };
            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            return output.Trim();  // ← note: we ignore error stream here – if command fails, you'll get empty string
        }
    }

    public class ChangeValueCommand
    {
        public async Task SetBrightnessAsync(int value)
        {
            // Added % suffix – brightnessctl requires it for percentage mode
            await TerminalCommands.RunCommandAsync($"brightnessctl set {value}%");
        }

        public async Task SetVolumeAsync(int percentage)
        {
            await TerminalCommands.RunCommandAsync(
                $"pactl set-sink-volume @DEFAULT_SINK@ {percentage}%"
            );
        }
    }

    public class CheckDependencyCommand
    {
        public int OriginalValueLight { get; private set; }
        public int OriginalValueSound { get; private set; }

        // Load both values at startup
        public async Task LoadOriginalValuesAsync()
        {
            OriginalValueLight = await GetBrightnessAsync();
            OriginalValueSound = await GetVolumeAsync();
        }

        // BRIGHTNESS 
        public async Task<int> GetBrightnessAsync()
        {
  
     
            string output = await TerminalCommands.RunCommandAsync("brightnessctl get");
            if (int.TryParse(output, out int value))
                return value;
            return -1;  
        }

        // VOLUME 
        public async Task<int> GetVolumeAsync()
        {
            string output = await TerminalCommands.RunCommandAsync(
                "pactl get-sink-volume @DEFAULT_SINK@"
            );
            // Extract the first percentage value (e.g., "100%")
            var match = Regex.Match(output, @"(\d+)%");
            if (match.Success)
                return int.Parse(match.Groups[1].Value);
            return -1;
        }
    }
}