// The original implementation of this class
// was from the _Pat Me_ and _Emote Log_ plugins.
//
// It has been modified here to be more generic, and support
// notifications of both incoming and outgoing emotes.
//
// See:
//  - https://github.com/MgAl2O4/PatMeDalamud
//  - https://github.com/RokasKil/EmoteLog


using System;
using System.Linq;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Hooking;

namespace AetherSenseRedux.Hooks;

public class EmoteReaderHooks : IDisposable
{
    public delegate void EmoteDelegate(EmoteEvent e);

    /// <summary>
    /// Event triggered when the player either performs an emote
    /// or is the target of someone else performing an emote.
    /// </summary>
    public event EmoteDelegate? OnEmote;

    private readonly Hook<OnEmoteFuncDelegate> _hookEmote;

    public EmoteReaderHooks()
    {
        _hookEmote = Service.GameInteropProvider.HookFromSignature<OnEmoteFuncDelegate>(
            "E8 ?? ?? ?? ?? 48 8D 8B ?? ?? ?? ?? 4C 89 74 24", OnEmoteDetour);
        _hookEmote.Enable();
    }

    public void Dispose()
    {
        _hookEmote.Dispose();
        GC.SuppressFinalize(this);
    }

    ~EmoteReaderHooks()
    {
        _hookEmote.Dispose();
    }

    private void OnEmoteDetour(ulong unk, ulong instigatorAddr, ushort emoteId, ulong targetId, ulong unk2)
    {
        if (Service.ClientState.LocalPlayer != null)
        {
            var instigatorOb = Service.ObjectTable.FirstOrDefault(x => (ulong)x.Address == instigatorAddr);
            if (instigatorOb is IPlayerCharacter playerCharacter)
            {
                // If a remote player performed the emote while targeting the local player
                if (targetId == Service.ClientState.LocalPlayer.GameObjectId)
                {
                    Service.PluginLog.Verbose(
                        $"Player {instigatorOb.Name} used emote {emoteId} on target {Service.ClientState.LocalPlayer.Name} ({targetId:X})");
                    OnEmote?.Invoke(new EmoteEvent
                    {
                        EmoteId = emoteId,
                        Instigator = playerCharacter,
                        Target = Service.ClientState.LocalPlayer
                    });
                }
                // If the local player performed the emote
                else if (instigatorOb.GameObjectId == Service.ClientState.LocalPlayer.GameObjectId)
                {
                    var targetOb = targetId != 0xE0000000
                        ? Service.ObjectTable.FirstOrDefault(x => x.GameObjectId == targetId)
                        : null;

                    Service.PluginLog.Verbose(
                        $"Local player {instigatorOb.Name} used emote {emoteId}" + (targetOb != null
                            ? $" on target {targetOb.Name} ({targetId:X})"
                            : string.Empty));

                    OnEmote?.Invoke(new EmoteEvent
                    {
                        EmoteId = emoteId,
                        Instigator = Service.ClientState.LocalPlayer,
                        Target = targetOb is IPlayerCharacter targetPc ? targetPc : null
                    });
                }
            }
        }

        _hookEmote.Original(unk, instigatorAddr, emoteId, targetId, unk2);
    }

    /// <summary>
    /// Delegate signature for the function being hooked in the game.
    /// </summary>
    private delegate void OnEmoteFuncDelegate(ulong unk, ulong instigatorAddr, ushort emoteId, ulong targetId, ulong unk2);
}

public class EmoteEvent
{
    /// <summary>
    ///     The character which performed the emote.
    /// </summary>
    public required IPlayerCharacter Instigator { get; init; }

    /// <summary>
    ///     The character who was the target of the emote.
    /// </summary>
    public IPlayerCharacter? Target { get; init; }

    /// <summary>
    ///     The emote ID.
    /// </summary>
    public ushort EmoteId { get; init; }
}