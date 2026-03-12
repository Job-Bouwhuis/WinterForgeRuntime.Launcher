using WinterRose.Recordium;


namespace WinterForgeCLICompiler;

public class ProgramArguments
{
    private readonly Dictionary<string, string> parameters = new(StringComparer.OrdinalIgnoreCase);
    private readonly string[] expectedPositionalKeys;
    private static Log log = new("ProgramArguments");

    public ProgramArguments(string[] args) : this(args, []) { }

    public ProgramArguments(string[] args, params string[] expectedPositionalKeys)
    {
        this.expectedPositionalKeys = expectedPositionalKeys;
        Parse(args);
    }

    public static implicit operator ProgramArguments(string[] args) => new(args);

    private void Parse(string[] args)
    {
        var positionalValues = new List<string>();

        foreach (var arg in args)
        {
            if (arg.StartsWith("--"))
            {
                string cleanArg = arg[2..];

                int equalsIndex = cleanArg.IndexOf('=');

                if (equalsIndex >= 0)
                {
                    string key = cleanArg[..equalsIndex].Trim();
                    string value = cleanArg[(equalsIndex + 1)..].Trim();
                    parameters[key] = value;
                }
                else
                {
                    parameters[cleanArg] = "true";
                }
            }
            else
            {
                positionalValues.Add(arg);
            }
        }

        for (int i = 0; i < positionalValues.Count && i < expectedPositionalKeys.Length; i++)
        {
            var key = expectedPositionalKeys[i];
            parameters[key] = positionalValues[i];
        }

        if (positionalValues.Count > expectedPositionalKeys.Length)
        {
            for (int i = expectedPositionalKeys.Length; i < positionalValues.Count; i++)
                WarnUnknown(positionalValues[i]);
        }
    }

    private void WarnUnknown(string arg) => 
        log.Warning($"Unrecognized argument format: '{arg}'. Arguments should start with '--' " +
            $"and optionally contain '=' for values. " +
            $"Example: --key=value or --flag");

    public string Get(string key, string defaultValue = null)
    {
        if (!parameters.TryGetValue(key, out var val) || string.IsNullOrWhiteSpace(val))
            return defaultValue;
        return val;
    }

    public int GetInt(string key, int defaultValue = 0)
        => int.TryParse(Get(key), out var val) ? val : defaultValue;

    public bool GetBool(string key, bool defaultValue = false)
        => bool.TryParse(Get(key), out var val) ? val : defaultValue;

    public float GetFloat(string key, float defaultValue = 0f)
        => float.TryParse(Get(key), out var val) ? val : defaultValue;
}