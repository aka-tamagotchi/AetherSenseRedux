using System;
using System.Collections.Generic;

namespace AetherSenseRedux.Trigger.Emote;

[Serializable]
public class EmoteTriggerConfig : TriggerConfig
{
    public override string Type { get; } = "Emote";
    public override string Name { get; set; } = "New Emote Trigger";
    public List<ushort> EmoteIds { get; set; } = [];
    public bool TriggerOnPerform = true;
    public bool TriggerOnTarget = false;
    public long RetriggerDelay { get; set; } = 0;
}