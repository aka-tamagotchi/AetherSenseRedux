using System;
using Dalamud.Game.ClientState.Objects.SubKinds;

namespace AetherSenseRedux.Trigger.Emote;

public class EmoteLogItem
{
    public required IPlayerCharacter Instigator { get; init; }
    public IPlayerCharacter? Target { get; init; }
    public ushort EmoteId { get; init; }
    public bool PlayerIsPerformer { get; init; }
    public bool PlayerIsTarget { get; init; }
    public DateTime Timestamp { get; init; }
}