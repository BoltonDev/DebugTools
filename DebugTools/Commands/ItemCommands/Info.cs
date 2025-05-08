namespace DebugTools.Commands.ItemCommands;

using System;
using System.Reflection;
using System.Text;
using CommandSystem;
using LabApi.Features.Wrappers;

[CommandHandler(typeof(ItemParent))]
public class Info : ICommand
{
    public string Command { get; } = "info";
    public string[] Aliases { get; } = { "i" };
    public string Description { get; } = "Displays all information about the item in the current player's hand";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
            return false;

        if (!Player.TryGet(sender, out Player player))
        {
            response = "You must be a player to use this command.";
            return false;
        }

        Item currentItem = player.CurrentItem;
        if (currentItem == null)
        {
            response = "You must have an item in your hand.";
            return false;
        }

        StringBuilder builder = new();
        builder.AppendLine("Item in the current player's hand:");

        foreach (PropertyInfo propertyInfo in currentItem.GetType().GetProperties())
        {
            try
            {
                builder.AppendLine($"{propertyInfo.Name}: {propertyInfo.GetValue(currentItem)}");
            }
            catch (Exception)
            {
                builder.AppendLine($"{propertyInfo.Name}: <Unsupported>");
            }
        }

        response = builder.ToString();
        return true;
    }
}