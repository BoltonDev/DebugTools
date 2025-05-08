namespace DebugTools.Commands.ObjetCommands;

using CommandSystem;

[CommandHandler(typeof(DebugToolsParent))]
public class ObjetParent : ParentBaseCommand
{
    public override string Command { get; } = "object";
    public override string[] Aliases { get; } = { "o" };
    public override string Description { get; } = "Parent command for objects debugging";
}