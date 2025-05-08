namespace DebugTools.Commands.EventCommands;

using CommandSystem;

[CommandHandler(typeof(DebugToolsParent))]
public class EventParent : ParentBaseCommand
{
    public override string Command { get; } = "event";
    public override string[] Aliases { get; } = { "e" };
    public override string Description { get; } = "Parent command for events debugging";
}