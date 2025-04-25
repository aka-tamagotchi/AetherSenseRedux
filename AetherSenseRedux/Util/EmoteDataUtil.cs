using System.Collections.Immutable;
using Lumina.Excel.Sheets;

namespace AetherSenseRedux.Util;

public static class EmoteDataUtil
{
    /// <summary>
    /// Cached list of emotes so we don't need to repeatedly request it from the DataManager,
    /// since we'll be fetching the list on every UI refresh.
    /// </summary>
    private static ImmutableList<Emote>? _emotes;

    /// <summary>
    /// Get a list of all emotes.
    /// </summary>
    /// <returns></returns>
    public static ImmutableList<Emote> GetEmotes()
    {
        return _emotes ??= Service.DataManager.GetExcelSheet<Emote>().ToImmutableList();
    }

    /// <summary>
    /// Attempt to get a single Emote by ID
    /// </summary>
    /// <param name="emoteId">The emote ID</param>
    /// <returns></returns>
    public static Emote? GetEmote(uint emoteId)
    {
        return Service.DataManager.GetExcelSheet<Emote>().GetRowOrDefault(emoteId);
    }
}