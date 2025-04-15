namespace DebugTools.Commands;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CommandSystem;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class ListCancelEventCommand : ICommand
{
    public string Command { get; } = "listcancelevent";
    public string[] Aliases { get; } = { "lce", "lcevent" };
    public string Description { get; } = "Shows you the list of all cancelled event.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands))
        {
            response = "You do not have permission to use this command.";
            return false;
        }

        StringBuilder builder = new();
        builder.Append("All cancelled events:\n");
        foreach (KeyValuePair<EventInfo, Delegate> eventHandler in EventLogger.CancelledEvents)
        {
            Type eventArgs = eventHandler.Key.EventHandlerType.GetGenericArguments()[0];
            string eventName = eventArgs.Name.Replace("EventArgs", "");
            builder.Append($"- {eventName}\n");
        }

        response = builder.ToString();
        return true;
    }
}