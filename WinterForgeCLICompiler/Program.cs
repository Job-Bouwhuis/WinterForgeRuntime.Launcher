using WinterForgeCLICompiler;
using WinterRose.WinterForgeSerializing;

ProgramArguments arguments = new(args, "input-file");

if (arguments.Get("help") != null || args.Length == 0)
{
    Console.WriteLine("Usage: wf-compile <file.wf> [--output-path=DIR] [--output-file=NAME] [--compile] [--run]");
    return 0;
}

string inputFile = arguments.Get("input-file");
if (!File.Exists(inputFile))
{
    Console.WriteLine($"File not found: {inputFile}");
    return 1;
}

// Determine operation
bool doRun = arguments.GetBool("run");
bool doCompile = arguments.GetBool("compile") || !doRun; // default to compile if neither

if (doCompile)
{
    // Compute output directory
    string outputDir = arguments.Get("output-path", Path.GetDirectoryName(Path.GetFullPath(inputFile)));
    if (!Directory.Exists(outputDir))
        Directory.CreateDirectory(outputDir);

    // Compute output file name
    string outputFileName = arguments.Get("output-file", Path.GetFileNameWithoutExtension(inputFile) + ".wfc");
    string outputFile = Path.Combine(outputDir, outputFileName);

    try
    {
        WinterForge.ConvertFromFileToFile(inputFile, outputFile);
        Console.WriteLine($"Compiled {inputFile} → {outputFileName}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Compilation failed: {ex.Message}");
        return 2;
    }
}

if (doRun)
{
    try
    {
        object res = WinterForge.DeserializeFromFile(inputFile);
        if (res is Nothing)
            return 0;

        if (!arguments.GetBool("hide-output"))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Script result: {res}");
            Console.ResetColor();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Run failed: {ex.Message}");
        return 3;
    }
}

return 0;