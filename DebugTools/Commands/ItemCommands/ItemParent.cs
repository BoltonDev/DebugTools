namespace DebugTools.Commands.ItemCommands;

using CommandSystem;

[CommandHandler(typeof(DebugToolsParent))]
public class ItemParent : ParentBaseCommand
{
    public override string Command { get; } = "item";
    public override string[] Aliases { get; } = { "i" };
    public override string Description { get; } = "Parent command for items debugging";
}