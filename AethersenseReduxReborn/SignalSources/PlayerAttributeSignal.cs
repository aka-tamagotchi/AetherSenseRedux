using System;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Utility;
using ImGuiNET;

namespace AethersenseReduxReborn.SignalSources;

public sealed class PlayerAttributeSignal: SignalBase
{
    private readonly PlayerAttributeSignalConfig _config;

    public PlayerAttributeSignal(PlayerAttributeSignalConfig config) { _config = config; }

    public override void Update(double elapsedMilliseconds)
    {
        if (_config.PlayerName.IsNullOrWhitespace()){
            Value = 0;
            return;
        }
        
        var player = (PlayerCharacter)Service.ObjectTable.Single(o => o.Name.TextValue == _config.PlayerName);
        var val = _config.AttributeToTrack switch {
            AttributeToTrack.Hp => (double)player.CurrentHp / player.MaxHp,
            AttributeToTrack.Mp => (double)player.CurrentMp / player.MaxMp,
            _                   => throw new ArgumentOutOfRangeException(),
        };
        if (_config.Correlation == Correlation.Inverse)
            val = 1 - val;
        Value = val;
    }

    public override void DrawConfig() { _config.DrawConfigOptions(); }
}

public class PlayerAttributeSignalConfig
{
    public required string           PlayerName       { get; set; }
    public required AttributeToTrack AttributeToTrack { get; set; }
    public required Correlation      Correlation      { get; set; }

    public static PlayerAttributeSignalConfig DefaultConfig() =>
        new() {
                  PlayerName       = "",
                  AttributeToTrack = AttributeToTrack.Hp,
                  Correlation      = Correlation.Positive,
              };

    internal void DrawConfigOptions()
    {
        var name = PlayerName;
        if (ImGui.InputText("Player Name", ref name, 20))
            if (name is not null){
                if (name.IsValidCharacterName())
                    PlayerName = name;
                else
                    ImGui.Text("Please enter a valid character name.");
            }

        using (var attributeCombo = ImRaii.Combo("Attribute to Track", AttributeToTrack.ToString())){
            if (attributeCombo){
                var selected = AttributeToTrack;
                foreach (var attributeToTrack in Enum.GetValues<AttributeToTrack>()){
                    if (ImGui.Selectable(attributeToTrack.ToString(), attributeToTrack == selected))
                        selected = attributeToTrack;
                }

                AttributeToTrack = selected;
            }
        }

        using (var correlationCombo = ImRaii.Combo("Correlation", Correlation.ToString())){
            if (correlationCombo){
                var selected = Correlation;
                foreach (var correlation in Enum.GetValues<Correlation>()){
                    if (ImGui.Selectable(correlation.ToString(), correlation == selected))
                        selected = correlation;
                }

                Correlation = selected;
            }
        }
    }
}

public enum AttributeToTrack
{
    Hp,
    Mp,
}

public enum Correlation
{
    Positive,
    Inverse,
}
