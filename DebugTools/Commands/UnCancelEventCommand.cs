namespace DebugTools.Commands;

using System;
using System.Collections.Generic;
using System.Reflection;
using CommandSystem;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class UnCancelEventCommand : ICommand
{
    public string Command { get; } = "uncancelevent";
    public string[] Aliases { get; } = { "uce", "ucevent" };
    public string Description { get; } = "Allows you to restore the execution of an event.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands))
        {
            response = "You do not have permission to use this command.";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = "You must provide the event name.";
            return false;
        }

        string eventNameArg = arguments.At(0);

        foreach (KeyValuePair<EventInfo, Delegate> eventHandler in EventLogger.CancelledEvents)
        {
            Type eventArgs = eventHandler.Key.EventHandlerType.GetGenericArguments()[0];
            string eventName = eventArgs.Name.Replace("EventArgs", "");

            if (eventNameArg != eventName) continue;

            eventHandler.Key.RemoveEventHandler(DebugTools.Instance, eventHandler.Value);

            EventLogger.CancelledEvents.Remove(eventHandler.Key);

            response = $"{eventName} has been un-cancelled.";
            return true;
        }

        response = "Can't find the event, or it isn't cancelled.";
        return false;
    }
}