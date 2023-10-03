namespace AethersenseReduxReborn.SignalSources;

public enum SignalSourceType
{
    ChatTrigger,
    PlayerAttribute,
}

public static class SignalSourceTypeExtensions
{
    public static string DisplayString(this SignalSourceType sourceType) =>
        sourceType switch {
            SignalSourceType.ChatTrigger     => "Chat Trigger",
            SignalSourceType.PlayerAttribute => "Player Attribute",
            _                                => "Unknown Type",
        };
}
