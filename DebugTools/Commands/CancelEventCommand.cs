namespace DebugTools.Commands;

using System;
using System.Collections.Generic;
using System.Reflection;
using CommandSystem;
using LabApi.Events.Arguments.Interfaces;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CancelEventCommand : ICommand
{
    public string Command { get; } = "cancelevent";
    public string[] Aliases { get; } = { "ce", "cevent" };
    public string Description { get; } = "Allows you to prevent the execution of an event (must implement ICancellableEvent).";

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

        foreach (KeyValuePair<EventInfo, Delegate> eventHandler in EventLogger.EventHandlers)
        {
            Type eventHandlerType = eventHandler.Key.EventHandlerType;

            if (!eventHandlerType.IsGenericType)
                continue;

            Type eventArgs = eventHandlerType.GetGenericArguments()[0];
            string eventName = eventArgs.Name.Replace("EventArgs", "");

            if (eventNameArg != eventName || !typeof(ICancellableEvent).IsAssignableFrom(eventArgs)) continue;

            if (EventLogger.CancelledEvents.ContainsKey(eventHandler.Key))
            {
                response = $"{eventName} is already cancelled.";
                return false;
            }

            Delegate handler = Delegate.CreateDelegate(eventHandlerType, typeof(CancelEventCommand).GetMethod(nameof(CancelEventCommand.HandleICancellableEvent)).MakeGenericMethod(eventHandlerType.GetGenericArguments()));
            eventHandler.Key.AddEventHandler(DebugTools.Instance, handler);

            EventLogger.CancelledEvents.Add(eventHandler.Key, handler);

            response = $"{eventName} has been cancelled.";
            return true;
        }

        response = "Can't find the event, or it doesn't implement ICancellableEvent interface.";
        return false;
    }

    public static void HandleICancellableEvent<T>(T ev) where T : ICancellableEvent
    {
        ev.IsAllowed = false;
    }
}