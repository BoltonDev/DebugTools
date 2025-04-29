namespace DebugTools;

using System;
using LabApi.Features;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using Utils.NonAllocLINQ;

public class DebugTools : Plugin<Config>
{
    public override string Name { get; } = "DebugTools";
    public override string Description { get; } = "";
    public override string Author { get; } = "Bolton";
    public override Version Version { get; } = new(1, 0, 1);
    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);

    public static DebugTools Instance { get; private set; }
    public static bool ObjectDumpingEnabled { get; private set; }

    public override void Enable()
    {
        Instance = this;
        ObjectDumpingEnabled = PluginLoader.Dependencies.Any(a => a.FullName.Contains("ObjectDumping"));

        EventLogger.RegisterEvents();
    }

    public override void Disable()
    {
        EventLogger.UnregisterEvents();

        Instance = null;
    }
}