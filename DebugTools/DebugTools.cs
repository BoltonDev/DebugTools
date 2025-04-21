namespace DebugTools;

using System;
using System.Reflection;
using HarmonyLib;
using LabApi.Features;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using Utils.NonAllocLINQ;

public class DebugTools : Plugin<Config>
{
    private Harmony _harmony;

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

        _harmony = new Harmony("fr.bolton.debug-tools");
        _harmony.PatchAll();

        EventLogger.RegisterEvents();
    }

    public override void Disable()
    {
        EventLogger.UnregisterEvents();

        Instance = null;
    }
}