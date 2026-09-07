using Spectre.Console;

#region Title

var title = new FigletText("Brew Kit")
{
    Justification = Justify.Center,
    Color = Color.White,
};

var titleSection = new Panel(title)
{
    Border = BoxBorder.Heavy,
    BorderStyle = new Style(Color.White),
    Padding = new Padding(0, 0, 0, 0)
};

AnsiConsole.Write(titleSection);

#endregion

#region SD Detection

AnsiConsole.MarkupLine("Searching for SD Card...");

List<DriveInfo> drives = [];

await AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots)
    .StartAsync("Waiting for SD card...", async ctx =>
    {
        while (drives.Count == 0)
        {
            drives = DriveHelper.GetDrives();
            if (drives.Count == 0)
            {
                ctx.Status("Waiting for SD card... (insert one now)");
                await Task.Delay(1000);
            }
        }
    });

AnsiConsole.WriteSuccess($"Found {drives.Count} drive(s)");

var driveChoices = new SelectionPrompt<string>()
    .Title("Select SD Drive:")
    .AddChoices(drives.Select(drive => drive.Name));

var selectedDriveName = AnsiConsole.Prompt(driveChoices);
var chosenDrive = drives.Single(drive => drive.Name == selectedDriveName);
var selectedDriveFormat = chosenDrive.DriveFormat.ToUpper();

AnsiConsole.WriteLine($"Detected SD Format: {selectedDriveFormat}");

if (string.Compare(selectedDriveFormat, "FAT32") != 0
    && string.Compare(selectedDriveFormat, "MSDOS") != 0)
{
    AnsiConsole.WriteWarning("Chosen drive must be formatted to FAT32. Doing so will result in deletion of ALL data currently on the drive.", newline: false);
    AnsiConsole.MarkupLine("[red]Please backup any important data before proceeding.[/]");
    // Call convert here
}
else
{
    AnsiConsole.WriteSuccess("Drive formatted correctly.");
}

#endregion