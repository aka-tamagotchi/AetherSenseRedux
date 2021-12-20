using AetherSenseRedux.Pattern;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Dalamud.Logging;
using System.Collections.Concurrent;

namespace AetherSenseRedux.Trigger
{
    internal class ChatTrigger : ITrigger
    {
        // ITrigger properties
        public bool Enabled { get; set; }
        public string Type { get; } = "ChatTrigger";
        public string Name { get; init; }
        public List<Device> Devices { get; init; }
        public List<string> EnabledDevices { get; init; }
        public string Pattern { get; init; }
        public PatternConfig PatternSettings { get; init; }

        // ChatTrigger properties
        private ConcurrentQueue<ChatMessage> _messages;
        public Regex Regex { get; init; }
        public long RetriggerDelay { get; init; }
        private DateTime RetriggerTime { get; set; }
        private Guid Guid { get; set; }
        
        /// <summary>
        /// Instantiates a new ChatTrigger.
        /// </summary>
        /// <param name="configuration">The configuration object for this trigger.</param>
        /// <param name="devices">A reference to the list of Buttplug Devices.</param>
        /// <returns>A ChatTrigger object.</returns>
        public ChatTrigger(ChatTriggerConfig configuration, ref List<Device> devices)
        {
            // ITrigger properties
            Enabled = true;
            Name = configuration.Name;
            Devices = devices;
            EnabledDevices = configuration.EnabledDevices;
            Pattern = configuration.Pattern;
            PatternSettings = PatternFactory.GetPatternConfigFromObject(configuration.PatternSettings);
            Regex = new Regex(configuration.Regex);
            RetriggerDelay = configuration.RetriggerDelay;

            // ChatTrigger properties
            _messages = new ConcurrentQueue<ChatMessage>();
            RetriggerTime = DateTime.MinValue;
            Guid = Guid.NewGuid();

        }

        /// <summary>
        /// Adds a chat message to the trigger's processing queue.
        /// </summary>
        /// <param name="message">The chat message.</param>
        public void Queue(ChatMessage message)
        {
            if (Enabled)
            {
                 PluginLog.Verbose("{0} ({1}): Received message to queue",Name, Guid.ToString());
                 _messages.Enqueue(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnTrigger()
        {
            if (RetriggerDelay > 0)
            {
                if (DateTime.UtcNow < RetriggerTime)
                {
                    PluginLog.Debug("Trigger discarded, too soon");
                    return;
                }
                else
                {
                    RetriggerTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(RetriggerDelay);
                }
            }
            lock (Devices)
            {
                foreach (Device device in Devices)
                {
                    if (EnabledDevices.Contains(device.Name))
                    {
                        lock (device.Patterns)
                        {
                            device.Patterns.Add(PatternFactory.GetPatternFromObject(PatternSettings));
                        }
                    }

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {
            Task.Run(MainLoop).ConfigureAwait(false); ;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task MainLoop()
        {
            while (Enabled)
            {
                ChatMessage message;
                if (_messages.TryDequeue(out message))
                {
                    PluginLog.Verbose("{1}: Processing message: {0}", message.ToString(), Guid.ToString());
                    if (Regex.IsMatch(message.ToString()))
                    {
                        OnTrigger();
                        PluginLog.Debug("{1}: Triggered on message: {0}", message.ToString(), Guid.ToString());
                    }
                    await Task.Yield();
                } else
                {
                    await Task.Delay(50);
                }
            }
        }
        static TriggerConfig GetDefaultConfiguration()
        {
            return new ChatTriggerConfig();
        }
    }

    [Serializable]
    public class ChatTriggerConfig : TriggerConfig
    {
        public override string Type { get; } = "Chat";
        public override string Name { get; set; } = "New Chat Trigger";
        public string Regex { get; set; } = "Your Regex Here";
        public long RetriggerDelay { get; set; } = 0;

    }

    struct ChatMessage
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
            ChatType = (uint)chatType & 0x7F;
            Source = (uint)chatType >> 11;
            Destination = ((uint)chatType >> 7) & 0xF;
            FilterString = String.Format("{0:X1}{1:X1}{2:D2}",Source,Destination,ChatType);
            //SenderId = senderId;
            Sender = sender.TextValue;
            Message = message.TextValue;
            IsHandled = isHandled;
        }

        public uint ChatType;
        public uint Source;
        public uint Destination;
        public string FilterString;
        //public uint SenderId;
        public string Sender;
        public string Message;
        public bool IsHandled;

        public override string ToString()
        {
            return string.Format("{2} <{0}> {1}", Sender, Message, FilterString);
        }
    }

    enum MessageType : ushort
    {
        None = 0,
        Debug = 1,
        Urgent = 2,
        Notice = 3,
        
        Say = 10,
        Shout = 11,
        TellOutGoing = 12,
        TellIncoming = 13,
        Party = 14,
        Alliance = 15,
        Ls1 = 16,
        Ls2 = 17,
        Ls3 = 18,
        Ls4 = 19,
        Ls5 = 20,
        Ls6 = 21,
        Ls7 = 22,
        Ls8 = 23,
        FreeCompany = 24,

        NoviceNetwork = 27,
        CustomEmote = 28,
        StandardEmote = 29,
        Yell = 30,
        
        CrossWorldParty = 32,

        PvP = 36,
        Cwls1 = 37,

        AttackHit = 41,
        AttackMiss = 42,
        Action = 43,
        Item = 44,
        Healing = 45,
        Buff = 46,
        Debuff = 47,
        BuffEnd = 48,
        DebuffEnd = 49,

        Alarm = 55,
        Echo = 56,
        SystemMessage = 57,
        BattleSystemMessage = 58,
        GatheringSystemMessage = 59,
        Error = 60,
        NPCDialogue = 61,
        LootNotice = 62,

        ProgressionMessage = 64,
        LootMessage = 65,
        SynthesisMessage = 66,
        GatheringMessage = 67,
        NPCAnnouncement = 68,
        FreeCompanyAnnouncement = 69,
        FreeCompanyLogin = 70,
        SaleNotification = 71,
        PartyFinderNotice = 72,
        SignMessage = 73,
        RandomNumberMessage = 74,
        NoviceNetworkNotification = 75,
        OrchestrionNotification = 76,
        PvPTeamAnnouncement = 77,
        PvPLogin = 78,
        MessageBookAlert = 79,
        
        Cwls2 = 101,
        Cwls3 = 102,
        Cwls4 = 103,
        Cwls5 = 104,
        Cwls6 = 105,
        Cwls7 = 106,
        Cwls8 = 107
        

    }

    enum MessageTarget   : ushort
    {
        System = 0x0,
        You = 0x1,
        Party = 0x2,
        Alliance = 0x3,
        Other = 0x4,
        EngagedHostile = 0x5,
        UnengagedHostile = 0x6,
        FriendlyNPC = 0x7,
        Pet = 0x8,
        PartyPet = 0x9,
        AlliancePet = 0xA,
        OtherPet = 0xB
    }
}
