namespace DebugTools;

using System.ComponentModel;

public class Config
{
    [Description("(Object dumping) Debug level (max level).")]
    public int DebugLevel { get; set; } = 1;
}