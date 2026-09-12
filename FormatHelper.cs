using System.Diagnostics;

public class FormatHelper
{
    public static async Task<bool> FormatToFat32Async(DriveInfo drive, string label = "WII")
    {
        if (OperatingSystem.IsMacOS())
        {
            return await RunAsync("diskutil", ["eraseVolume", "FAT32", label, drive.RootDirectory.FullName]);
        }
        else if (OperatingSystem.IsWindows())
        {
            // format asks for the current volume label before erasing
            return await RunAsync("format", [drive.Name.TrimEnd('\\'), "/FS:FAT32", "/Q", $"/V:{label}", "/Y"], drive.VolumeLabel);
        }
        else
        {
            return false;
        }
    }

    static async Task<bool> RunAsync(string fileName, string[] args, string? input = null)
    {
        var startInfo = new ProcessStartInfo(fileName, args)
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };

        using var process = Process.Start(startInfo)!;
        await process.StandardInput.WriteLineAsync(input);
        process.StandardInput.Close();

        await Task.WhenAll(process.StandardOutput.ReadToEndAsync(), process.StandardError.ReadToEndAsync(), process.WaitForExitAsync());
        return process.ExitCode == 0;
    }
}
