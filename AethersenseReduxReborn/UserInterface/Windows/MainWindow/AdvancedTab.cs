using Dalamud.Interface.Raii;
using ImGuiNET;

namespace AethersenseReduxReborn.UserInterface.Windows.MainWindow;

public class AdvancedTab : ITab
{
    private bool _logChat;
    
    private readonly Configuration _configuration;
    
    public string Name => "Advanced";

    public AdvancedTab(Configuration configuration)
    {
        _configuration = configuration;

        _logChat = _configuration.LogChat;
    }
    
    public void Draw()
    {
        using var advancedTab = ImRaii.TabItem("Advanced");
        if (!advancedTab)
            return;

        var configValue = _logChat;
        if (ImGui.Checkbox("Log Chat to Debug", ref configValue)) {
            _logChat = configValue;
        }

        if (ImGui.Button("Restore Default Triggers")) {
            _configuration.LoadDefaults();
        }
    }
}
