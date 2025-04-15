namespace DebugTools;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using LabApi.Features;
using LabApi.Features.Console;

internal static class EventLogger
{
    public static readonly Dictionary<EventInfo, Delegate> EventHandlers = new();
    public static readonly Dictionary<EventInfo, Delegate> CancelledEvents = new();

    public static void RegisterEvents()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(LabApiProperties));
        List<Type> eventHandlers = assembly.GetTypes().Where(t => t.Namespace == "LabApi.Events.Handlers").ToList();

        foreach (Type eventHandler in eventHandlers)
        {
            foreach (EventInfo eventInfo in eventHandler.GetEvents())
            {
                Type handlerType = eventInfo.EventHandlerType;

                MethodInfo handlerMethod = handlerType.IsGenericType ? 
                    typeof(EventLogger).GetMethod(nameof(EventLogger.HandleEventTEventArgs)).MakeGenericMethod(handlerType.GetGenericArguments()) : 
                    typeof(EventLogger).GetMethod(nameof(EventLogger.HandleEvent));

                if (handlerMethod == null)
                    continue;

                Delegate handler = Delegate.CreateDelegate(handlerType, handlerMethod);

                eventInfo.AddEventHandler(DebugTools.Instance, handler);

                EventHandlers.Add(eventInfo, handler);
            }
        }
    }

    public static void UnregisterEvents()
    {
        foreach (KeyValuePair<EventInfo, Delegate> eventHandler in EventHandlers)
        {
            eventHandler.Key.RemoveEventHandler(DebugTools.Instance, eventHandler.Value);
        }

        EventHandlers.Clear();
    }

    public static void HandleEventTEventArgs<T>(T ev)
        where T : EventArgs
    {
        StringBuilder builder = new();
        builder.Append($"[DebugTools] [EventLogger] Event: {ev.GetType().Name.Replace("EventArgs", "")}\n");
        builder.Append("Args:\n");
    
        foreach (PropertyInfo propertyInfo in ev.GetType().GetProperties())
        {
            builder.Append($"- {propertyInfo.Name}: {propertyInfo.GetValue(ev)}\n");
        }

        Logger.Raw(builder.ToString(), ConsoleColor.Cyan);
    }

    public static void HandleEvent()
    {
        string eventName = new StackTrace().GetFrame(2).GetMethod().Name.Remove(0, 2);
        Logger.Raw($"[DebugTools] [EventLogger] Event: {eventName}", ConsoleColor.Cyan);
    }
}