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
using XIVChatTypes;

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
        public PatternConfig PatternSettings { get; init; }
        private XIVChatFilter Filter { get; init; }
        public bool UseFilter { get; init; }

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
            PatternSettings = PatternFactory.GetPatternConfigFromObject(configuration.PatternSettings);

            Regex = new Regex(configuration.Regex);
            RetriggerDelay = configuration.RetriggerDelay;
            UseFilter = configuration.UseFilter;
            Filter = new XIVChatFilter(configuration.FilterTable);

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
                    if (UseFilter && !Filter.Match(message.ChatType))
                    {
                        continue;
                    }
                    if (!Regex.IsMatch(message.ToString()))
                    {
                        continue;
                    }
                    
                    OnTrigger();
                    PluginLog.Debug("{1}: Triggered on message: {0}", message.ToString(), Guid.ToString());
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
        public bool UseFilter { get; set; } = false;
        public bool[][] FilterTable { get; set; } = new bool[14][]
                {
                    new bool[27],   //General
                    new bool[16],   // Battle: You
                    new bool[16],   // Battle: Party
                    new bool[16],   // Battle: Alliance
                    new bool[16],   // Battle: Other PC
                    new bool[16],   // Battle: Engaged
                    new bool[16],   // Battle: Unengaged
                    new bool[16],   // Battle: Friendly
                    new bool[16],   // Battle: Pet
                    new bool[16],   // Battle: Party Pet
                    new bool[16],   // Battle: Alliance Pet
                    new bool[16],   // Battle: Other's Pet
                    new bool[33],   // Misc
                    new bool[15]    // GM Messages
                };

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
            ChatType = (uint)chatType;
            //SenderId = senderId;
            Sender = sender.TextValue;
            Message = message.TextValue;
            IsHandled = isHandled;
        }

        public uint ChatType;
        //public uint SenderId;
        public string Sender;
        public string Message;
        public bool IsHandled;

        public override string ToString()
        {
            return string.Format("<{0}> {1}", Sender, Message);
        }
    }
}
