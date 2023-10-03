using System;
using System.Text.RegularExpressions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.SignalSources;

public class ChatTriggerSignal: SignalBase
{
    private          SimplePattern?          _currentPattern;
    private readonly ChatTriggerSignalConfig _config;

    public ChatTriggerSignal(ChatTriggerSignalConfig config)
    {
        _config                     =  config;
        Service.ChatGui.ChatMessage += OnChatMessageReceived;
    }

    private void OnChatMessageReceived(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (type != _config.ChatType)
            return;
        try{
            var match = _config.Regex.Match(message.TextValue);
            if (!match.Success)
                return;
            TriggerPattern();
        } catch (Exception e){
            Service.PluginLog.Error(e, "Error while matching regex");
        }
    }

    protected override void Dispose(bool disposing)
    {
        Service.ChatGui.ChatMessage -= OnChatMessageReceived;
        base.Dispose(disposing);
    }

    public override void Update(double elapsedMilliseconds)
    {
        var output = 0.0d;
        if (_currentPattern is not null)
            output = _currentPattern.Update(elapsedMilliseconds);
        if (_currentPattern is {
                IsCompleted: true,
            })
            _currentPattern = null;
        Value = output;
    }

    public override void DrawConfig()
    {
        using var id = ImRaii.PushId(Name);
        ImGui.Text(Name);
        ImGui.Text($"Intensity: {Value}");
        _config.DrawConfigOptions();
        if (ImGui.Button("Test Pattern"))
            TriggerPattern();
    }

    private void TriggerPattern() => _currentPattern = SimplePattern.CreatePatternFromConfig(_config.PatternConfig);
}

public class ChatTriggerSignalConfig
{
    public required SimplePatternConfig PatternConfig { get; set; }
    public required Regex               Regex         { get; set; }
    public required XivChatType         ChatType      { get; set; }

    public static ChatTriggerSignalConfig DefaultConfig() =>
        new() {
                  PatternConfig = SimplePatternConfig.DefaultConstantPattern(),
                  Regex         = new Regex(""),
                  ChatType      = XivChatType.None,
              };

    public void DrawConfigOptions()
    {
        using (var chatTypeCombo = ImRaii.Combo("Chat Type", ChatType.ToString())){
            if (chatTypeCombo){
                foreach (var chatType in Enum.GetValues<XivChatType>()){
                    if (ImGui.Selectable(chatType.ToString(), chatType == ChatType))
                        ChatType = chatType;
                }
            }
        }
        var pattern = Regex.ToString();
        if (ImGui.InputText("Regex", ref pattern, 2048))
            Regex = new Regex(pattern);

        PatternConfig.DrawConfigOptions();
    }
}
