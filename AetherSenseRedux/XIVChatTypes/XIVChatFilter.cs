using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIVChatTypes
{
    internal class XIVChatFilter
    {
        //public bool[][] FilterTable = new bool[14][]
        //        {
        //            new bool[27],   //General
        //            new bool[16],   // Battle: You
        //            new bool[16],   // Battle: Party
        //            new bool[16],   // Battle: Alliance
        //            new bool[16],   // Battle: Other PC
        //            new bool[16],   // Battle: Engaged
        //            new bool[16],   // Battle: Unengaged
        //            new bool[16],   // Battle: Friendly
        //            new bool[16],   // Battle: Pet
        //            new bool[16],   // Battle: Party Pet
        //            new bool[16],   // Battle: Alliance Pet
        //            new bool[16],   // Battle: Other's Pet
        //            new bool[33],   // Misc
        //            new bool[15]   // GM Messages
        //        };
        //    }
        //}

        public static string[] FilterCategoryNames = new string[14]
        {
            "General",
            "Battle: You",
            "Battle: Party",
            "Battle: Alliance",
            "Battle: Other PC",
            "Battle: Engaged",
            "Battle: Unengaged",
            "Battle: Friendly",
            "Battle: Pet",
            "Battle: Party Pet",
            "Battle: Alliance Pet",
            "Battle: Other's Pet",
            "Misc",
            "GM Messages"
        };

        public static string[][] FilterNames = new string[14][]
        {
            new string[27]      // General
            {
                "Say",
                "Yell",
                "Shout",
                "Tell",
                "Party",
                "Alliance",
                "Free Company",
                "PVP Team",
                "Cross-world Linkshell 1",
                "Cross-world Linkshell 2",
                "Cross-world Linkshell 3",
                "Cross-world Linkshell 4",
                "Cross-world Linkshell 5",
                "Cross-world Linkshell 6",
                "Cross-world Linkshell 7",
                "Cross-world Linkshell 8",
                "Linkshell 1",
                "Linkshell 2",
                "Linkshell 3",
                "Linkshell 4",
                "Linkshell 5",
                "Linkshell 6",
                "Linkshell 7",
                "Linkshell 8",
                "Novice Network",
                "Emotes (Standard)",
                "Emotes (Custom)"
            },
            new string[16]      // Battle: You
            {
                "Your AttackHit",
                "Your AttackMiss",
                "Your Action",
                "Your Item",
                "Your Healing",
                "Your Buff",
                "Your Debuff",
                "AttackHit You",
                "AttackMiss You",
                "Action You",
                "Item You",
                "Healing You",
                "Buff You",
                "Debuff You",
                "Your BuffEnd",
                "Your DebuffEnd",
            },   
            new string[16]      // Battle: Party
            {
                "Party's AttackHit",
                "Party's AttackMiss",
                "Party's Action",
                "Party's Item",
                "Party's Healing",
                "Party's Buff",
                "Party's Debuff",
                "AttackHit Party",
                "AttackMiss Party",
                "Action Party",
                "Item Party",
                "Healing Party",
                "Buff Party",
                "Debuff Party",
                "Party's BuffEnd",
                "Party's DebuffEnd",
            },
            new string[16]      // Battle: Alliance
            {
                "Alliance's AttackHit",
                "Alliance's AttackMiss",
                "Alliance's Action",
                "Alliance's Item",
                "Alliance's Healing",
                "Alliance's Buff",
                "Alliance's Debuff",
                "AttackHit Alliance",
                "AttackMiss Alliance",
                "Action Alliance",
                "Item Alliance",
                "Healing Alliance",
                "Buff Alliance",
                "Debuff Alliance",
                "Alliance's BuffEnd",
                "Alliance's DebuffEnd",
            },
            new string[16]      // Battle: Other PC
            {
                "Other PC's AttackHit",
                "Other PC's AttackMiss",
                "Other PC's Action",
                "Other PC's Item",
                "Other PC's Healing",
                "Other PC's Buff",
                "Other PC's Debuff",
                "AttackHit Other PC",
                "AttackMiss Other PC",
                "Action Other PC",
                "Item Other PC",
                "Healing Other PC",
                "Buff Other PC",
                "Debuff Other PC",
                "Other PC's BuffEnd",
                "Other PC's DebuffEnd",
            },
            new string[16]      // Battle: Engaged
            {
                "Engaged Enemy's AttackHit",
                "Engaged Enemy's AttackMiss",
                "Engaged Enemy's Action",
                "Engaged Enemy's Item",
                "Engaged Enemy's Healing",
                "Engaged Enemy's Buff",
                "Engaged Enemy's Debuff",
                "AttackHit Engaged Enemy",
                "AttackMiss Engaged Enemy",
                "Action Engaged Enemy",
                "Item Engaged Enemy",
                "Healing Engaged Enemy",
                "Buff Engaged Enemy",
                "Debuff Engaged Enemy",
                "Engaged Enemy's BuffEnd",
                "Engaged Enemy's DebuffEnd",
            },
            new string[16]      // Battle: Unengaged
            {
                "Unengaged Enemy's AttackHit",
                "Unengaged Enemy's AttackMiss",
                "Unengaged Enemy's Action",
                "Unengaged Enemy's Item",
                "Unengaged Enemy's Healing",
                "Unengaged Enemy's Buff",
                "Unengaged Enemy's Debuff",
                "AttackHit Unengaged Enemy",
                "AttackMiss Unengaged Enemy",
                "Action Unengaged Enemy",
                "Item Unengaged Enemy",
                "Healing Unengaged Enemy",
                "Buff Unengaged Enemy",
                "Debuff Unengaged Enemy",
                "Unengaged Enemy's BuffEnd",
                "Unengaged Enemy's DebuffEnd",
            },
            new string[16]      // Battle: Friendly
            {
                "Friendly NPC's AttackHit",
                "Friendly NPC's AttackMiss",
                "Friendly NPC's Action",
                "Friendly NPC's Item",
                "Friendly NPC's Healing",
                "Friendly NPC's Buff",
                "Friendly NPC's Debuff",
                "AttackHit Friendly NPC",
                "AttackMiss Friendly NPC",
                "Action Friendly NPC",
                "Item Friendly NPC",
                "Healing Friendly NPC",
                "Buff Friendly NPC",
                "Debuff Friendly NPC",
                "Friendly NPC's BuffEnd",
                "Friendly NPC's DebuffEnd",
            },
            new string[16]      // Battle: Pet
            {
                "Pet's AttackHit",
                "Pet's AttackMiss",
                "Pet's Action",
                "Pet's Item",
                "Pet's Healing",
                "Pet's Buff",
                "Pet's Debuff",
                "AttackHit Pet",
                "AttackMiss Pet",
                "Action Pet",
                "Item Pet",
                "Healing Pet",
                "Buff Pet",
                "Debuff Pet",
                "Pet's BuffEnd",
                "Pet's DebuffEnd",
            },
            new string[16]      // Battle: Party Pet
            {
                "Party Pet's AttackHit",
                "Party Pet's AttackMiss",
                "Party Pet's Action",
                "Party Pet's Item",
                "Party Pet's Healing",
                "Party Pet's Buff",
                "Party Pet's Debuff",
                "AttackHit Party Pet",
                "AttackMiss Party Pet",
                "Action Party Pet",
                "Item Party Pet",
                "Healing Party Pet",
                "Buff Party Pet",
                "Debuff Party Pet",
                "Party Pet's BuffEnd",
                "Party Pet's DebuffEnd",
            },
            new string[16]      // Battle: Alliance Pet
            {
                "Alliance Pet's AttackHit",
                "Alliance Pet's AttackMiss",
                "Alliance Pet's Action",
                "Alliance Pet's Item",
                "Alliance Pet's Healing",
                "Alliance Pet's Buff",
                "Alliance Pet's Debuff",
                "AttackHit Alliance Pet",
                "AttackMiss Alliance Pet",
                "Action Alliance Pet",
                "Item Alliance Pet",
                "Healing Alliance Pet",
                "Buff Alliance Pet",
                "Debuff Alliance Pet",
                "Alliance Pet's BuffEnd",
                "Alliance Pet's DebuffEnd",
            },
            new string[16]      // Battle: Other's Pet
            {
                "Other Pet's AttackHit",
                "Other Pet's AttackMiss",
                "Other Pet's Action",
                "Other Pet's Item",
                "Other Pet's Healing",
                "Other Pet's Buff",
                "Other Pet's Debuff",
                "AttackHit Other Pet",
                "AttackMiss Other Pet",
                "Action Other Pet",
                "Item Other Pet",
                "Healing Other Pet",
                "Buff Other Pet",
                "Debuff Other Pet",
                "Other Pet's BuffEnd",
                "Other Pet's DebuffEnd",
            },
            new string[33]      // Misc
            {
                "Debug",
                "Notice",
                "Urgent",
                "Alarm",
                "System Messages",
                "Your Battle System Messages",
                "Other's Battle System Messages",
                "Gathering System Messages",
                "Echo",
                "Error",
                "Novice Network Notifications",
                "Free Company Announcements",
                "PvP Team Announcements",
                "Free Company Login Notifications",
                "PvP Team Login Notifications",
                "Retainer Sale Notifications",
                "NPC Dialogue",
                "NPC Dialogue (Announcements)",
                "Loot Notices",
                "Your Progression Messages",
                "Party Member's Progression Messages",
                "Other's Progression Messages",
                "Your Loot Messages",
                "Other's Loot Messages",
                "Your Synthesis Messages",
                "Other's Synthesis Messages",
                "Your Gathering Messages",
                "Other's Gathering Messages",
                "Party Search",
                "Raid Markers on PCs",
                "Random Numbers",
                "Orchestrion Notifications",
                "Estate Message Book Alerts"
            },
            new string[15]      // GM Messages
            {
                "[GM] Say",
                "[GM] Shout",
                "[GM] Tell",
                "[GM] Party",
                "[GM] Free Company",
                "[GM] Novice Network",
                "[GM] Yell",
                "[GM] Linkshell 1",
                "[GM] Linkshell 2",
                "[GM] Linkshell 3",
                "[GM] Linkshell 4",
                "[GM] Linkshell 5",
                "[GM] Linkshell 6",
                "[GM] Linkshell 7",
                "[GM] Linkshell 8"
            }   
        };

        private static readonly (Group?, Group?, Channel)[][][] FilterMasks = new (Group?, Group?, Channel)[14][][]
        {
            new (Group?, Group?, Channel)[27][] 
            {
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Say) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Yell) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Shout) },
                new (Group?, Group?, Channel)[2] { (null, null, Channel.TellOutgoing), (null, null, Channel.TellIncoming) },
                new (Group?, Group?, Channel)[2] { (null, null, Channel.Party), (null, null, Channel.CrossWorldParty) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Alliance) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.FreeCompany) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.PvPTeam) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Cwls1) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Cwls2) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Cwls3) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Cwls4) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Cwls5) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Cwls6) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Cwls7) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Cwls8) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Ls1) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Ls2) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Ls3) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Ls4) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Ls5) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Ls6) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Ls7) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Ls8) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.NoviceNetwork) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.StandardEmote) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.CustomEmote) },
            },
            new (Group?, Group?, Channel)[16][] 
            {
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (null, Group.You, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (null, Group.You, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (null, Group.You, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (null, Group.You, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (null, Group.You, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (null, Group.You, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (null, Group.You, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.BuffEnd) },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.DebuffEnd) }
            },
            new (Group?, Group?, Channel)[16][]
            {
                new (Group?, Group?, Channel)[1] { (Group.Party, null, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (Group.Party, null, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (Group.Party, null, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (Group.Party, null, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (Group.Party, null, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (Group.Party, null, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (Group.Party, null, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (null, Group.Party, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (null, Group.Party, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (null, Group.Party, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (null, Group.Party, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (null, Group.Party, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (null, Group.Party, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (null, Group.Party, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (Group.Party, null, Channel.BuffEnd) },
                new (Group?, Group?, Channel)[1] { (Group.Party, null, Channel.DebuffEnd) }
            },
            new (Group?, Group?, Channel)[16][]
            {
                new (Group?, Group?, Channel)[1] { (Group.Alliance, null, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (Group.Alliance, null, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (Group.Alliance, null, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (Group.Alliance, null, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (Group.Alliance, null, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (Group.Alliance, null, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (Group.Alliance, null, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (null, Group.Alliance, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (null, Group.Alliance, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (null, Group.Alliance, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (null, Group.Alliance, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (null, Group.Alliance, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (null, Group.Alliance, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (null, Group.Alliance, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (Group.Alliance, null, Channel.BuffEnd) },
                new (Group?, Group?, Channel)[1] { (Group.Alliance, null, Channel.DebuffEnd) }
            },
            new (Group?, Group?, Channel)[16][]
            {
                new (Group?, Group?, Channel)[1] { (Group.Other, null, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (Group.Other, null, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (Group.Other, null, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (Group.Other, null, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (Group.Other, null, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (Group.Other, null, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (Group.Other, null, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (null, Group.Other, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (null, Group.Other, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (null, Group.Other, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (null, Group.Other, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (null, Group.Other, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (null, Group.Other, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (null, Group.Other, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (Group.Other, null, Channel.BuffEnd) },
                new (Group?, Group?, Channel)[1] { (Group.Other, null, Channel.DebuffEnd) }
            },
            new (Group?, Group?, Channel)[16][]
            {
                new (Group?, Group?, Channel)[1] { (Group.EngagedHostile, null, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (Group.EngagedHostile, null, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (Group.EngagedHostile, null, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (Group.EngagedHostile, null, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (Group.EngagedHostile, null, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (Group.EngagedHostile, null, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (Group.EngagedHostile, null, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (null, Group.EngagedHostile, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (null, Group.EngagedHostile, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (null, Group.EngagedHostile, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (null, Group.EngagedHostile, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (null, Group.EngagedHostile, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (null, Group.EngagedHostile, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (null, Group.EngagedHostile, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (Group.EngagedHostile, null, Channel.BuffEnd) },
                new (Group?, Group?, Channel)[1] { (Group.EngagedHostile, null, Channel.DebuffEnd) }
            },
            new (Group?, Group?, Channel)[16][]
            {
                new (Group?, Group?, Channel)[1] { (Group.UnengagedHostile, null, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (Group.UnengagedHostile, null, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (Group.UnengagedHostile, null, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (Group.UnengagedHostile, null, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (Group.UnengagedHostile, null, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (Group.UnengagedHostile, null, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (Group.UnengagedHostile, null, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (null, Group.UnengagedHostile, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (null, Group.UnengagedHostile, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (null, Group.UnengagedHostile, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (null, Group.UnengagedHostile, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (null, Group.UnengagedHostile, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (null, Group.UnengagedHostile, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (null, Group.UnengagedHostile, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (Group.UnengagedHostile, null, Channel.BuffEnd) },
                new (Group?, Group?, Channel)[1] { (Group.UnengagedHostile, null, Channel.DebuffEnd) }
            },
            new (Group?, Group?, Channel)[16][]
            {
                new (Group?, Group?, Channel)[1] { (Group.FriendlyNPC, null, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (Group.FriendlyNPC, null, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (Group.FriendlyNPC, null, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (Group.FriendlyNPC, null, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (Group.FriendlyNPC, null, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (Group.FriendlyNPC, null, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (Group.FriendlyNPC, null, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (null, Group.FriendlyNPC, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (null, Group.FriendlyNPC, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (null, Group.FriendlyNPC, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (null, Group.FriendlyNPC, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (null, Group.FriendlyNPC, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (null, Group.FriendlyNPC, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (null, Group.FriendlyNPC, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (Group.FriendlyNPC, null, Channel.BuffEnd) },
                new (Group?, Group?, Channel)[1] { (Group.FriendlyNPC, null, Channel.DebuffEnd) }
            },
            new (Group?, Group?, Channel)[16][]
            {
                new (Group?, Group?, Channel)[1] { (Group.Pet, null, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (Group.Pet, null, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (Group.Pet, null, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (Group.Pet, null, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (Group.Pet, null, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (Group.Pet, null, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (Group.Pet, null, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (null, Group.Pet, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (null, Group.Pet, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (null, Group.Pet, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (null, Group.Pet, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (null, Group.Pet, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (null, Group.Pet, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (null, Group.Pet, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (Group.Pet, null, Channel.BuffEnd) },
                new (Group?, Group?, Channel)[1] { (Group.Pet, null, Channel.DebuffEnd) }
            },
            new (Group?, Group?, Channel)[16][]
            {
                new (Group?, Group?, Channel)[1] { (Group.PartyPet, null, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (Group.PartyPet, null, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (Group.PartyPet, null, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (Group.PartyPet, null, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (Group.PartyPet, null, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (Group.PartyPet, null, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (Group.PartyPet, null, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (null, Group.PartyPet, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (null, Group.PartyPet, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (null, Group.PartyPet, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (null, Group.PartyPet, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (null, Group.PartyPet, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (null, Group.PartyPet, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (null, Group.PartyPet, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (Group.PartyPet, null, Channel.BuffEnd) },
                new (Group?, Group?, Channel)[1] { (Group.PartyPet, null, Channel.DebuffEnd) }
            },
            new (Group?, Group?, Channel)[16][]
            {
                new (Group?, Group?, Channel)[1] { (Group.AlliancePet, null, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (Group.AlliancePet, null, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (Group.AlliancePet, null, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (Group.AlliancePet, null, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (Group.AlliancePet, null, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (Group.AlliancePet, null, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (Group.AlliancePet, null, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (null, Group.AlliancePet, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (null, Group.AlliancePet, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (null, Group.AlliancePet, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (null, Group.AlliancePet, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (null, Group.AlliancePet, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (null, Group.AlliancePet, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (null, Group.AlliancePet, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (Group.AlliancePet, null, Channel.BuffEnd) },
                new (Group?, Group?, Channel)[1] { (Group.AlliancePet, null, Channel.DebuffEnd) }
            },
            new (Group?, Group?, Channel)[16][]
            {
                new (Group?, Group?, Channel)[1] { (Group.OtherPet, null, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (Group.OtherPet, null, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (Group.OtherPet, null, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (Group.OtherPet, null, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (Group.OtherPet, null, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (Group.OtherPet, null, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (Group.OtherPet, null, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (null, Group.OtherPet, Channel.AttackHit) },
                new (Group?, Group?, Channel)[1] { (null, Group.OtherPet, Channel.AttackMiss) },
                new (Group?, Group?, Channel)[1] { (null, Group.OtherPet, Channel.Action) },
                new (Group?, Group?, Channel)[1] { (null, Group.OtherPet, Channel.Item) },
                new (Group?, Group?, Channel)[1] { (null, Group.OtherPet, Channel.Healing) },
                new (Group?, Group?, Channel)[1] { (null, Group.OtherPet, Channel.Buff) },
                new (Group?, Group?, Channel)[1] { (null, Group.OtherPet, Channel.Debuff) },
                new (Group?, Group?, Channel)[1] { (Group.OtherPet, null, Channel.BuffEnd) },
                new (Group?, Group?, Channel)[1] { (Group.OtherPet, null, Channel.DebuffEnd) }
            },
            new (Group?, Group?, Channel)[33][]
            {
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Debug) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Urgent) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Notice) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Alarm) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.SystemMessage) },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.BattleSystemMessage) },
                new (Group?, Group?, Channel)[10] 
                {
                    (Group.Party, null, Channel.BattleSystemMessage), 
                    (Group.Alliance, null, Channel.BattleSystemMessage), 
                    (Group.Other, null, Channel.BattleSystemMessage),
                    (Group.EngagedHostile, null, Channel.BattleSystemMessage), 
                    (Group.UnengagedHostile, null, Channel.BattleSystemMessage), 
                    (Group.FriendlyNPC, null, Channel.BattleSystemMessage),
                    (Group.Pet, null, Channel.BattleSystemMessage),
                    (Group.PartyPet, null, Channel.BattleSystemMessage), 
                    (Group.AlliancePet, null, Channel.BattleSystemMessage), 
                    (Group.OtherPet, null, Channel.BattleSystemMessage) 
                },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GatheringSystemMessage) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Error) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.Echo) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.NoviceNetworkNotification) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.FreeCompanyAnnouncement) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.PvPTeamAnnouncement) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.FreeCompanyLogin) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.PvPLogin) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.RetainerSale) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.NPCDialogue) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.NPCAnnouncement) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.LootNotice) },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.ProgressionMessage) },
                new (Group?, Group?, Channel)[1] { (Group.Party, null, Channel.ProgressionMessage) },
                new (Group?, Group?, Channel)[8] 
                {
                    (Group.Alliance, null, Channel.ProgressionMessage),
                    (Group.Other, null, Channel.ProgressionMessage),
                    (Group.EngagedHostile, null, Channel.ProgressionMessage),   //possibly pvp?
                    (Group.UnengagedHostile, null, Channel.ProgressionMessage), //possibly pvp?
                    (Group.Pet, null, Channel.ProgressionMessage),
                    (Group.PartyPet, null, Channel.ProgressionMessage),
                    (Group.AlliancePet, null, Channel.ProgressionMessage),
                    (Group.OtherPet, null, Channel.ProgressionMessage)
                },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.LootMessage) },
                new (Group?, Group?, Channel)[3] 
                { 
                    (Group.Party, null, Channel.LootMessage), 
                    (Group.Alliance, null, Channel.LootMessage), 
                    (Group.Other, null, Channel.LootMessage) 
                },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.SynthesisMessage) },
                new (Group?, Group?, Channel)[3] 
                { 
                    (Group.Party, null, Channel.SynthesisMessage), 
                    (Group.Alliance, null, Channel.SynthesisMessage), 
                    (Group.Other, null, Channel.SynthesisMessage) 
                },
                new (Group?, Group?, Channel)[1] { (Group.You, null, Channel.GatheringMessage) },
                new (Group?, Group?, Channel)[3] 
                { 
                    (Group.Party, null, Channel.GatheringMessage), 
                    (Group.Alliance, null, Channel.GatheringMessage), 
                    (Group.Other, null, Channel.GatheringMessage) 
                },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.PartySearch) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.RaidMarkers) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.RandomNumberMessage) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.OrchestrionNotification) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.MessageBookAlert) }
            },
            new (Group?, Group?, Channel)[15][] 
            {
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMSay) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMShout) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMTell) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMParty) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMFreeCompany) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMNoviceNetwork) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMYell) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMLs1) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMLs2) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMLs3) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMLs4) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMLs5) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMLs6) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMLs7) },
                new (Group?, Group?, Channel)[1] { (null, null, Channel.GMLs8) }
            }
        };

        private List<(Group, Group, Channel)> filterList;

        public XIVChatFilter()
        {
            filterList = new();
        }

        public XIVChatFilter(bool[][] filtertable)
        {
            filterList = new();
            ParseFilterTable(filtertable);
        }

        public void Add(int category, int index)
        {
            var filters = FilterMasks[category][index];
            foreach (var filter in filters)
            {
                Add(filter);
            }
        }

        public void Add((Group?, Group?, Channel) filter)
        {            
            foreach (var item in ExpandFilter(filter))
            {
                if (!filterList.Contains(item))
                {
                    filterList.Add(item);
                }
            }
        }

        public void Add(uint magic)
        {
            var chatType = XIVChatTypeEx.Decode(magic);
            Add(chatType);
        }


        public void Remove(int category, int index)
        {
            var filters = FilterMasks[category][index];
            foreach (var filter in filters)
            {
                Remove(filter);
            }
        }

        public void Remove((Group?, Group?, Channel) filter)
        {
            foreach (var item in ExpandFilter(filter))
            {
                filterList.Remove(item);
            }
        }

        public void Remove(uint magic)
        {
            var chatType = XIVChatTypeEx.Decode(magic);
            Remove(chatType);
        }

        public bool Match((Group, Group, Channel) chatType)
        {
            return filterList.Contains(chatType);
        }

        public bool Match(uint magic)
        {
            var chatType = XIVChatTypeEx.Decode(magic);
            return Match(chatType);
        }

        private static List<(Group, Group, Channel)> ExpandFilter((Group?, Group?, Channel) filter)
        {
            (var source, var target, var channel) = filter;
            List<Group> intermediate = new();
            List<(Group, Group, Channel)> result = new();
            if (source == null)
            {
                foreach (var a in Enum.GetValues(typeof(Group)))
                {
                    intermediate.Add((Group)a);
                }
            }
            else
            {
                intermediate.Add((Group)source);
            }

            if (target == null)
            {
                foreach (var source1 in intermediate)
                {
                    foreach (var a in Enum.GetValues(typeof(Group)))
                    {
                        result.Add((source1, (Group)a, channel));
                    }
                }
            }
            else
            {
                foreach (var source1 in intermediate)
                {
                    result.Add((source1, (Group)target, channel));
                }
            }

            return result;
        }

        private void ParseFilterTable(bool[][] filtertable)
        {
            for (int i1 = 0; i1 < filtertable.Length; i1++)
            {
                for (int i2 = 0; i2 < filtertable[i1].Length; i2++)
                {
                    if (filtertable[i1][i2])
                    {
                        
                        Add(i1,i2);
                        
                    }
                }
            }
        }

    }
}
