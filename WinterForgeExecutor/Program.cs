using WinterForgeRuntime.Launcher;
using WinterRose.Recordium;
using WinterRose.WinterForgeSerializing;

const string CONFIG_FILE = "RuntimeConfig.wf";
const string COMPILED_EXTENSION = ".wfbin";

WinterForge.NoAccessFilter = true;
LogDestinations.AddDestination(new ConsoleLogDestination());

if (!File.Exists(CONFIG_FILE))
{
    using Stream configTemplate = ResourceHelper.GetResourceStream("WinterForgeRuntime.Launcher.Default.RuntimeConfig.wf");
    using FileStream file = File.OpenWrite(CONFIG_FILE);
    configTemplate.CopyTo(file);
}

RuntimeConfig config = WinterForge.DeserializeFromFile<RuntimeConfig>(CONFIG_FILE)
   ?? throw new InvalidOperationException("The runtime config was invalid!");
if (config.WorkingDir != null)
    Directory.SetCurrentDirectory(config.WorkingDir);
else
    config.WorkingDir = Directory.GetCurrentDirectory();

DirectoryInfo scriptsDir = new DirectoryInfo(Path.Combine(config.WorkingDir, config.ScriptsDir ?? "Scripts"));
DirectoryInfo compiledDir = new DirectoryInfo(Path.Combine(config.WorkingDir, config.CompiledDir ?? "Compiled"));

if (config.GracefulErrors)
{
    try
    {
        StartRuntime();
    }
    catch (Exception e)
    {
        Popup.Show($"Exception of type '{e.GetType().Name}': {e.Message}");
    }
}
else
{
    StartRuntime();
}

void StartRuntime()
{
    WinterForge.ImportDir = compiledDir.FullName;

#if RELEASE
    if(!scriptsDir.Exists && compiledDir.Exists)
    {
        Run();
        return;
    }
#endif
    if (!scriptsDir.Exists)
        scriptsDir.Create();

    FileInfo[] scripts = scriptsDir.GetFiles();
    if (scripts.Length == 0)
    {
        using Stream template = ResourceHelper.GetResourceStream("WinterForgeRuntime.Launcher.Default.Main.wf");
        using FileStream file = File.OpenWrite(Path.Combine(scriptsDir.FullName, "Main.wf"));
        template.CopyTo(file);
        scripts = scriptsDir.GetFiles();
    }


    foreach (var script in scripts)
    {
        string compiledPath = Path.Combine(compiledDir.FullName, Path.GetFileNameWithoutExtension(script.Name)) + COMPILED_EXTENSION;
        WinterForge.ConvertFromFileToFile(script.FullName, compiledPath);
    }

    Run();
}

void Run()
{
    FileInfo? main = compiledDir.GetFiles($"{config.MainScript}.*").FirstOrDefault();
    if (main is null)
    {
        Popup.Show("No main script found. did you mistype the configuration?");
        return;
    }
    object result = WinterForge.DeserializeFromFile(main.FullName);
    if (result is int i)
        Environment.ExitCode = i;
}