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
public class Get : ICommand, IUsage
{
    public string Command { get; } = "get";
    public string[] Aliases { get; } = { "g" };
    public string Description { get; } = "Gets the component properties and fields from the object you're currently looking at.";
    public string Usage { get; } = "[component]";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
            return false;

        if (!Player.TryGet(sender, out Player player))
        {
            response = "You must be a player to use this command.";
            return false;
        }

        if (!Physics.Raycast(player.Camera.position, player.Camera.forward, out RaycastHit hit, float.PositiveInfinity, ~LayerMask.GetMask("Hitbox")))
        {
            response = "Unable to retrieve the object you're looking at.";
            return false;
        }

        if (arguments.Count < 1)
        {
            StringBuilder componentsList = new();
            componentsList.AppendLine("List of components (parent & current):");
            foreach (Component c in hit.collider.GetComponents<Component>().Concat(hit.collider.GetComponentsInParent<Component>()))
            {
                componentsList.AppendLine(c.GetType().Name);
            }

            response = componentsList.ToString();
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

        if (!hit.collider.TryGetComponent(componentType, out Component component) && !hit.collider.TryGetComponentInParent(componentType, out component))
        {
            response = "Unable to get the component from the object.";
            return false;
        }

        StringBuilder builder = new();
        builder.AppendLine("\n<b>Fields</b>");
        foreach (FieldInfo fieldInfo in component.GetType().GetFields())
        {
            try
            {
                builder.AppendLine($"  {fieldInfo.Name}: {fieldInfo.GetValue(component)}");
            }
            catch (Exception)
            {
                builder.AppendLine($"  {fieldInfo.Name}: <Unsupported>");
            }
        }

        builder.AppendLine("\n<b>Properties</b>");
        foreach (PropertyInfo propertyInfo in component.GetType().GetProperties())
        {
            try
            {
                builder.AppendLine($"  <color={(propertyInfo.CanWrite ? "green" : "red")}>{propertyInfo.Name}: {propertyInfo.GetValue(component)}</color>");
            }
            catch (Exception)
            {
                builder.AppendLine($"  {propertyInfo.Name}: <Unsupported>");
            }
        }

        response = builder.ToString();
        return true;
    }
}