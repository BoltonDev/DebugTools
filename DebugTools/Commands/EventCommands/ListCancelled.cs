namespace DebugTools.Commands.EventCommands;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CommandSystem;

[CommandHandler(typeof(EventParent))]
public class ListCancelled : ICommand
{
    public string Command { get; } = "listcancelled";
    public string[] Aliases { get; } = { "lc", "list" };
    public string Description { get; } = "Displays the list of all cancelled events";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
            return false;

        StringBuilder builder = new();

        builder.AppendLine("All cancelled events:\n");
        foreach (KeyValuePair<EventInfo, Delegate> eventHandler in EventLogger.CancelledEvents)
        {
            Type eventArgs = eventHandler.Key.EventHandlerType.GetGenericArguments()[0];
            string eventName = eventArgs.Name.Replace("EventArgs", "");
            builder.AppendLine($"- {eventName}");
        }

        response = builder.ToString();
        return true;
    }
}