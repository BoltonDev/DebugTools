namespace DebugTools.Commands;

using CommandSystem;
using EventCommands;
using ItemCommands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class DebugToolsParent : ParentBaseCommand
{
    public override string Command { get; } = "debugtools";
    public override string[] Aliases { get; } = { "dt", "debug" };
    public override string Description { get; } = "Debug tools parent command.";

    public override void LoadGeneratedCommands()
    {
        RegisterCommand(new EventParent());
        RegisterCommand(new ItemParent());
    }
}