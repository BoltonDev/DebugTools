namespace DebugTools;

using System.Collections.Generic;
using System.ComponentModel;

public class Config
{
    [Description("(Object dumping) Debug level (max level).")]
    public int DebugLevel { get; set; } = 1;

    public List<string> PreventEventLogging { get; set; } = new()
    {
        "PlayerSendingVoiceMessage",
        "PlayerReceivingVoiceMessage",
        "PlayerUsingRadio",
        "PlayerUsedRadio",
        "PlayerValidatedVisibility"
    };
}