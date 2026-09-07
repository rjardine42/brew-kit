public class DriveHelper
{
    public static List<DriveInfo> GetDrives()
    {
        if (OperatingSystem.IsWindows())
        {
            return DriveInfo.GetDrives()
                .Where(d => d.IsReady && d.DriveType == DriveType.Removable)
                .ToList();
        }
        else if (OperatingSystem.IsMacOS())
        {
            return DriveInfo.GetDrives()
                .Where(d => d.IsReady && d.RootDirectory.FullName.StartsWith("/Volumes"))
                .ToList();
        }
        else if (OperatingSystem.IsLinux())
        {
            return [];
        }
        else
        {
            return [];
        }
    }
}
