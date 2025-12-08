using System;
using System.Collections.Generic;
using System.Text;

namespace WinterForgeRuntime.Launcher;

public class RuntimeConfig
{
    public RuntimeConfig() {}

    public string MainScript { get; set; }
    public string? WorkingDir { get; set; }
    public string? ScriptsDir { get; set; }
    public string? CompiledDir { get; set; }
    public bool GracefulErrors { get; set; } = true;
}
