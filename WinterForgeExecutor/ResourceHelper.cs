using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace WinterForgeRuntime.Launcher;

public static class ResourceHelper
{
    public static Stream GetResourceStream(string resourceName)
    {
        var asm = Assembly.GetExecutingAssembly();
        return asm.GetManifestResourceStream(resourceName);
    }
    public static string[] GetResourceNames()
    {
        var asm = Assembly.GetExecutingAssembly();
        return asm.GetManifestResourceNames();
    }

    public static string GetResourceString(string resourceName)
    {
        using var stream = GetResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
