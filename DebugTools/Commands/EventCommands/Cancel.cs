namespace DebugTools.Commands.EventCommands;

using System;
using System.Collections.Generic;
using System.Reflection;
using CommandSystem;
using LabApi.Events.Arguments.Interfaces;

[CommandHandler(typeof(EventParent))]
public class Cancel : ICommand, IUsage
{
    public string Command { get; } = "cancel";
    public string[] Aliases { get; } = { "c" };
    public string Description { get; } = "Prevents the execution of an event. EventArgs must implement ICancellableEvent";
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

            Delegate handler = Delegate.CreateDelegate(eventHandlerType, typeof(Cancel).GetMethod(nameof(Cancel.HandleICancellableEvent)).MakeGenericMethod(eventHandlerType.GetGenericArguments()));
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