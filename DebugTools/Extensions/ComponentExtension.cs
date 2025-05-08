namespace DebugTools.Extensions;

using System;
using UnityEngine;

public static class ComponentExtension
{
    public static bool TryGetComponentInParent(this Component component, Type type, out Component result)
    {
        result = component.GetComponentInParent(type);
        return result != null;
    }
}