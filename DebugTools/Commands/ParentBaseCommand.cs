namespace DebugTools.Commands;

using System;
using System.Collections.Generic;
using System.Text;
using CommandSystem;

public class ParentBaseCommand : ParentCommand
{
    public override string Command { get; }
    public override string[] Aliases { get; }
    public override string Description { get; }

    public override void LoadGeneratedCommands()
    {
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
            return false;

        StringBuilder helper = new();

        helper.AppendLine("You need to specify a valid subcommand! Available ones:");
        foreach (KeyValuePair<string, ICommand> command in Commands)
        {
            ICommand cmd = command.Value;
            IUsage cmdUsage = cmd as IUsage;

            helper.AppendLine($"\n- <b>{command.Key} {cmdUsage?.Usage} (Aliases: {string.Join(", ", cmd.Aliases)})</b>");
            helper.AppendLine($"  <i>{cmd.Description}</i>");
        }

        response = helper.ToString();
        return false;
    }
}