namespace DebugTools;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using LabApi.Features;
using LabApi.Features.Console;

public static class EventLogger
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

                MethodInfo handlerMethod;
                if (handlerType.IsGenericType)
                {
                    if (DebugTools.Instance.Config.PreventEventLogging.Contains(handlerType.GetGenericArguments()[0].Name.Replace("EventArgs", string.Empty)))
                        continue;

                    handlerMethod = DebugTools.ObjectDumpingEnabled
                        ? typeof(EventLogger).GetMethod(nameof(EventLogger.HandleEventTEventArgsDump))
                            .MakeGenericMethod(handlerType.GetGenericArguments())
                        : typeof(EventLogger).GetMethod(nameof(EventLogger.HandleEventTEventArgs))
                            .MakeGenericMethod(handlerType.GetGenericArguments());
                }
                else
                {
                    handlerMethod = typeof(EventLogger).GetMethod(nameof(EventLogger.HandleEvent));   
                }

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

    public static void HandleEventTEventArgsDump<T>(T ev)
        where T : EventArgs
    {
        Logger.Raw(ObjectDumper.Dump(ev, new DumpOptions
        {
            IgnoreIndexers = false,
            MaxLevel = DebugTools.Instance.Config.DebugLevel,
            UseTypeFullName = true
        }), ConsoleColor.Cyan);
    }

    public static void HandleEventTEventArgs<T>(T ev)
        where T : EventArgs
    {
        StringBuilder builder = new();
        builder.AppendLine($"[DebugTools] [EventLogger] Event: {ev.GetType().Name.Replace("EventArgs", "")}");
        builder.AppendLine("Args:");

        foreach (PropertyInfo propertyInfo in ev.GetType().GetProperties())
        {
            try
            {
                builder.AppendLine($"- {propertyInfo.Name}: {propertyInfo.GetValue(ev)}");
            }
            catch (Exception)
            {
                builder.AppendLine($"- {propertyInfo.Name}: <Unsupported>");
            }
        }

        Logger.Raw(builder.ToString(), ConsoleColor.Cyan);
    }

    public static void HandleEvent()
    {
        string eventName = new StackTrace().GetFrame(2).GetMethod().Name.Remove(0, 2);
        Logger.Raw($"[DebugTools] [EventLogger] Event: {eventName}", ConsoleColor.Cyan);
    }
}