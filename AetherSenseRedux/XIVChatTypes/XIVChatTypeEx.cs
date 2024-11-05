using System.Collections.Generic;

namespace XIVChatTypes
{
    public partial class XIVChatTypeEx
    {

        /// <summary>
        /// Decodes Dalamud's XivChatType magic number
        /// </summary>
        /// <param name="magic">The magic number from XivChatType as a uint</param>
        /// <returns>A tuple containing the source as a Group enum, target as a Group enum, and channel as a Channel enum in that order.</returns>
        public static (Group, Group, Channel) Decode(uint magic)
        {
            var chatBaseType = (Channel)(magic & 0x007F);
            var chatSource = (Group)(magic >> 11);
            var chatTarget = (Group)((magic >> 7) & 0xF);

            return (chatSource, chatTarget, chatBaseType);
        }

        /// <summary>
        /// Encodes a chat type into the magic number expected by Dalamud's XivChatType
        /// </summary>
        /// <param name="chatType">A tuple containing the source as a Group enum, target as a Group enum, and channel as a Channel enum in that order.</param>
        /// <returns>The magic number for XivChatType as a uint</returns>
        public static uint Encode((Group, Group, Channel) chatType)
        {
            (var source, var target, var channel) = chatType;
            return (uint)channel | ((uint)target << 7) | ((uint)source << 11);
        }

        public static Dictionary<Channel, string> ChannelFriendlyName { get; } = new Dictionary<Channel, string>()
        {
            { Channel.None, "None" },
            { Channel.Debug, "Debug"},
            { Channel.Urgent, "Urgent"},
            { Channel.Notice, "Notice"},
            { Channel.Say, "Say"},
            { Channel.Shout, "Shout" },
            { Channel.TellOutgoing, "Tell (Outgoing)"},
            { Channel.TellIncoming, "Tell (Incoming)"},
            { Channel.Party, "Party" },
            { Channel.Alliance, "Alliance" },
            { Channel.Ls1, "Linkshell 1" },
            { Channel.Ls2, "Linkshell 2" },
            { Channel.Ls3, "Linkshell 3" },
            { Channel.Ls4, "Linkshell 4" },
            { Channel.Ls5, "Linkshell 5" },
            { Channel.Ls6, "Linkshell 6" },
            { Channel.Ls7, "Linkshell 7" },
            { Channel.Ls8, "Linkshell 8" },
            { Channel.FreeCompany, "Free Company" },
            { Channel.NoviceNetwork, "Novice Network" },
            { Channel.CustomEmote, "Emote (Custom)" },
            { Channel.StandardEmote, "Emote" },
            { Channel.Yell, "Yell"},
            { Channel.CrossWorldParty, "Cross-world Party" },
            { Channel.PvPTeam, "PvP Team" },
            { Channel.Cwls1, "Cross-world Linkshell 1" },
            { Channel.AttackHit, "Damage Notification" },
            { Channel.AttackMiss, "Miss Notification" },
            { Channel.Action, "Action Use" },
            { Channel.Item, "Item Use" },
            { Channel.Healing, "Heal" },
            { Channel.Buff, "Buff" },
            { Channel.Debuff, "Debuff" },
            { Channel.BuffEnd, "Buff Expires" },
            { Channel.DebuffEnd, "Debuff Expires" },
            { Channel.Alarm, "Alarm" },
            { Channel.Echo, "Echo" },
            { Channel.SystemMessage, "System Message" },
            { Channel.BattleSystemMessage, "Battle System Message" },
            { Channel.GatheringSystemMessage, "Gathering System Message" },
            { Channel.Error, "Error Message" },
            { Channel.NPCDialogue, "NPC Dialogue" },
            { Channel.LootNotice, "Loot Notice" },
            { Channel.ProgressionMessage, "Player Progression" },
            { Channel.LootMessage, "Loot Message" },
            { Channel.SynthesisMessage, "Synthesis Message" },
            { Channel.GatheringMessage, "Gathering Message"},
            { Channel.NPCAnnouncement, "NPC Announcement" },
            { Channel.FreeCompanyAnnouncement, "FC Announcement" },
            { Channel.FreeCompanyLogin, "FC Login Notification" },
            { Channel.RetainerSale, "Retainer Sale Notification" },
            { Channel.PartySearch, "Matchmaking Notification" },
            { Channel.RaidMarkers, "Raid Marker" },
            { Channel.RandomNumberMessage, "Random Number" },
            { Channel.NoviceNetworkNotification, "Novice Network Notification" },
            { Channel.OrchestrionNotification, "Orchestrion Now Playing"},
            { Channel.PvPTeamAnnouncement, "PVP Team Announcement" },
            { Channel.PvPLogin, "PVP Team Login Notification"},
            { Channel.MessageBookAlert, "Message Book Alert" },
            { Channel.GMTell, "GM Tell" },
            { Channel.GMSay, "GM Say" },
            { Channel.GMShout, "GM Shout" },
            { Channel.GMYell, "GM Yell" },
            { Channel.GMParty, "GM Party" },
            { Channel.GMFreeCompany, "GM Free Company" },
            { Channel.GMLs1, "GM Linkshell 1" },
            { Channel.GMLs2, "GM Linkshell 2" },
            { Channel.GMLs3, "GM Linkshell 3" },
            { Channel.GMLs4, "GM Linkshell 4" },
            { Channel.GMLs5, "GM Linkshell 5" },
            { Channel.GMLs6, "GM Linkshell 6" },
            { Channel.GMLs7, "GM Linkshell 7" },
            { Channel.GMLs8, "GM Linkshell 8" },
            { Channel.GMNoviceNetwork, "GM Novice Network" },
            { Channel.Cwls2, "Cross-world Linkshell 2" },
            { Channel.Cwls3, "Cross-world Linkshell 3" },
            { Channel.Cwls4, "Cross-world Linkshell 4" },
            { Channel.Cwls5, "Cross-world Linkshell 5" },
            { Channel.Cwls6, "Cross-world Linkshell 6" },
            { Channel.Cwls7, "Cross-world Linkshell 7" },
            { Channel.Cwls8, "Cross-world Linkshell 8" }
        };

        public static Dictionary<Group, string> GroupFriendlyName { get; } = new Dictionary<Group, string>()
        {
            { Group.System, "System" },
            { Group.You, "You" },
            { Group.Party, "Party Member" },
            { Group.Alliance, "Alliance Member" },
            { Group.Other, "Other PC" },
            { Group.EngagedHostile, "Engaged Hostile" },
            { Group.UnengagedHostile, "Unengaged Hostile" },
            { Group.FriendlyNPC, "Friendly NPC" },
            { Group.Pet, "Pet/Companion" },
            { Group.PartyPet, "Party Member's Pet/Companion" },
            { Group.AlliancePet, "Alliance Member's Pet/Companion" },
            { Group.OtherPet, "Other PC's Pet/Companion" }
        };
    }
    public enum Channel : ushort
    {
        None = 0,   // Displays in game as [LogKind未設定] which translates to "Log Kind Not Set"
        Debug = 1,
        Urgent = 2,
        Notice = 3,

        Say = 10,
        Shout = 11,
        TellOutgoing = 12,
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

        CrossWorldParty = 32, // exists but doesn't seem to be used anymore

        PvPTeam = 36,
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
        RetainerSale = 71,
        PartySearch = 72,
        RaidMarkers = 73,
        RandomNumberMessage = 74,
        NoviceNetworkNotification = 75,
        OrchestrionNotification = 76,
        PvPTeamAnnouncement = 77,
        PvPLogin = 78,
        MessageBookAlert = 79,

        GMTell = 80,
        GMSay = 81,
        GMShout = 82,
        GMYell = 83,
        GMParty = 84,
        GMFreeCompany = 85,
        GMLs1 = 86,
        GMLs2 = 87,
        GMLs3 = 88,
        GMLs4 = 89,
        GMLs5 = 90,
        GMLs6 = 91,
        GMLs7 = 92,
        GMLs8 = 93,
        GMNoviceNetwork = 94,

        Cwls2 = 101,
        Cwls3 = 102,
        Cwls4 = 103,
        Cwls5 = 104,
        Cwls6 = 105,
        Cwls7 = 106,
        Cwls8 = 107


    }

    public enum Group : ushort
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
