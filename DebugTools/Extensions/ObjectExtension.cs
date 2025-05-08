namespace DebugTools.Extensions;

using System;

public static class ObjectExtension
{
    public static object ConvertTo(this object value, Type targetType)
    {
        if (value == null || targetType == null)
            throw new ArgumentNullException();

        Type underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType.IsEnum)
        {
            if (value is string str)
                return Enum.Parse(underlyingType, str, ignoreCase: true);
            return Enum.ToObject(underlyingType, value);
        }

        return underlyingType.IsInstanceOfType(value) ? value : Convert.ChangeType(value, underlyingType);
    }
}