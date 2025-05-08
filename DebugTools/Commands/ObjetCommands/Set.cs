namespace DebugTools.Commands.ObjetCommands;

using System;
using System.Linq;
using System.Reflection;
using System.Text;
using CommandSystem;
using Extensions;
using LabApi.Features.Wrappers;
using UnityEngine;

[CommandHandler(typeof(ObjetParent))]
public class Set : ICommand, IUsage
{
    public string Command { get; } = "set";
    public string[] Aliases { get; } = { "s" };
    public string Description { get; } = "Sets a value to a component property of the object you're currently looking at.";
    public string Usage { get; } = "<component> <property_name> <value>";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
            return false;

        if (!Player.TryGet(sender, out Player player))
        {
            response = "You must be a player to use this command.";
            return false;
        }

        if (arguments.Count < 3)
        {
            response = "You must provide the name of the component, the name of the property and its new value.";
            return false;
        }

        string componentName = arguments.At(0);
        Type componentType = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == componentName);

        if (componentType == null)
        {
            response = "The component you provided was not found.";
            return false;
        }

        if (!Physics.Raycast(player.Camera.position, player.Camera.forward, out RaycastHit hit, float.PositiveInfinity, ~LayerMask.GetMask("Hitbox")))
        {
            response = "Unable to retrieve the object you're looking at.";
            return false;
        }

        if (!hit.collider.TryGetComponent(componentType, out Component component) && !hit.collider.TryGetComponentInParent(componentType, out component))
        {
            response = "Unable to get the component from the object.";
            return false;
        }

        PropertyInfo propertyInfo = component.GetType().GetProperty(arguments.At(1));
        if (propertyInfo == null)
        {
            response = "The property doesn't exist.";
            return false;
        }

        string value = arguments.At(2);
        Type propertyType = propertyInfo.PropertyType;
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            propertyType = Nullable.GetUnderlyingType(propertyType);
        }

        if (propertyType == null)
        {
            response = "Property type is null.";
            return false;
        }

        try
        {
            object convertedValue = value.ConvertTo(propertyType);
            propertyInfo.SetValue(component, convertedValue);
        }
        catch (Exception)
        {
            response = "Unable to set the value.";
            return false;
        }

        response = "Property value has been changed.";
        return true;
    }
}