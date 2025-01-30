using Microsoft.Win32;

#pragma warning disable CA1416

if (args.Length == 0)
{
    Console.WriteLine("No arguments provided.");

    Environment.Exit(1);
}

switch (args[0])
{
    case "-a":
        RunOpenEditWithTool();
        break;
    case "-b":
        RunRestoreOldContextMenuTool();
        break;
    case "-c":
        RunRevertRestoreOldContextMenuTool();
        break;
    default:
        Console.WriteLine($"Unknown argument: {args[0]}. Possible values are: -a (OpenEditWithTool)");
        Environment.Exit(1);
        break;
}

return 0;

// e.g. '-a Sublime "Edit in Sublime" "C:\Program Files\Sublime Text 3\sublime_text.exe"'
void RunOpenEditWithTool()
{
    if (args.Length < 4)
    {
        Console.WriteLine("Incorrect number of arguments. 1: application 2: display 3: executable 4 (optional): icon");
        Environment.Exit(1);
    }

    var applicationName = args[1];
    var displayText     = args[2];
    var executablePath  = args[3];
    var iconPath        = args.Length > 4 ? args[4] : args[3];

    var commandValue = $"{executablePath} %1";

    using var key = Registry.CurrentUser.CreateSubKey($@"Software\Classes\*\shell\{applicationName}");
    key.SetValue(null, displayText, RegistryValueKind.ExpandString);
    key.SetValue("Icon", iconPath, RegistryValueKind.ExpandString);

    var commandKey = key.CreateSubKey("command");
    commandKey.SetValue(null, commandValue, RegistryValueKind.ExpandString);

    Console.WriteLine("Entry added");
}

void RunRestoreOldContextMenuTool()
{
    using var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\CLSID\{{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}}\InProcServer32");
    key.SetValue(null, "", RegistryValueKind.String);

    Console.WriteLine("Entry added");
}

void RunRevertRestoreOldContextMenuTool()
{
    Registry.CurrentUser.DeleteSubKey(@"Software\Classes\CLSID\{{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}}");

    Console.WriteLine("Entry removed");
}

#pragma warning restore CA1416
