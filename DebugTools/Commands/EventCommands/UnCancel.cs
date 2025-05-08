namespace DebugTools.Commands.EventCommands;

using System;
using System.Collections.Generic;
using System.Reflection;
using CommandSystem;

[CommandHandler(typeof(EventParent))]
public class UnCancel : ICommand, IUsage
{
    public string Command { get; } = "uncancel";
    public string[] Aliases { get; } = { "uc", "u" };
    public string Description { get; } = "Restores the execution of an event";
    public string Usage { get; } = "<event_name>";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
            return false;

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