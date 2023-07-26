using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace AetherSenseRedux.Trigger;

internal struct ChatMessage
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatType"></param>
    /// <param name="senderId"></param>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    /// <param name="isHandled"></param>
    public ChatMessage(XivChatType chatType, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        ChatType = (uint)chatType;
        //SenderId = senderId;
        Sender    = sender.TextValue;
        Message   = message.TextValue;
        IsHandled = isHandled;
    }

    public uint ChatType;
    //public uint SenderId;
    public string Sender;
    public string Message;
    public bool   IsHandled;

    public override string ToString()
    {
        return $"<{Sender}> {Message}";
    }
}
