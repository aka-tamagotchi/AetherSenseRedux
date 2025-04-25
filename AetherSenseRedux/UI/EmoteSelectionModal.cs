using System;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using AetherSenseRedux.Util;
using Dalamud.Interface.Colors;
using ImGuiNET;
using Lumina.Excel.Sheets;

namespace AetherSenseRedux.UI;

public class EmoteSelectionModal
{
    /// <summary>
    /// The name of the modal in ImGui.
    /// </summary>
    private string Name { get; init; }

    /// <summary>
    /// The current value of search input.
    /// This should only be set using the `setSearchString` method.
    /// </summary>
    private string _emoteSearch = "";

    /// <summary>
    /// The current list of emotes as filtered by the _emoteSearch input value.
    /// </summary>
    private ImmutableList<Emote> _filteredEmotes;

    /// <summary>
    /// The string that was last used to generate the _filteredEmotes value.
    /// </summary>
    private string _currentFilter = "";

    /// <summary>
    /// The list of *all* emotes, sorted according to the table's current sort settings.
    /// </summary>
    private ImmutableList<Emote> _sortedEmotes;

    /// <summary>
    /// Whether or not the _sortedEmotes have been modified,
    /// and the _filteredEmotes needs to be re-filtered.
    /// </summary>
    private bool _filterSortDirty;

    public EmoteSelectionModal(string name)
    {
        this.Name = name;
        this._sortedEmotes = EmoteDataUtil.GetEmotes();
        this._filteredEmotes = this._sortedEmotes;
    }

    public void OpenModalPopup()
    {
        ImGui.OpenPopup(Name);
    }

    public bool DrawEmoteSelectionPopup(string popupTitle, Emote? selectedEmote, out uint emoteId)
    {
        var pOpen = true;
        if (ImGui.BeginPopupModal(Name, ref pOpen))
        {
            var emoteSearchString = _emoteSearch;
            if (ImGui.InputTextWithHint("Search", "Name, /command, or ID", ref emoteSearchString, 50))
            {
                SetSearchString(emoteSearchString);
            }

            const ImGuiTableFlags flags = ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingStretchProp | ImGuiTableFlags.Sortable | ImGuiTableFlags.NoSavedSettings;
            var outerSize = new Vector2(0, ImGui.GetTextLineHeightWithSpacing() * 10);
            if (ImGui.BeginTable("EmoteSelectionTable", 3, flags, outerSize))
            {

                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableSetupColumn("ID", ImGuiTableColumnFlags.None, 0, (uint)EmoteTableColumn.Id);
                ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.DefaultSort, 0, (uint)EmoteTableColumn.Name);
                ImGui.TableSetupColumn("Command", ImGuiTableColumnFlags.None, 0, (uint)EmoteTableColumn.Command);
                ImGui.TableHeadersRow();

                if (ImGui.TableGetSortSpecs() is var sortSpecs)
                    ApplyEmoteTableSorting(sortSpecs);

                unsafe
                {
                    var clipper = new ImGuiListClipperPtr(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
                    clipper.Begin(FilteredEmotes.Count);
                    while (clipper.Step())
                    {
                        for (var row = clipper.DisplayStart; row < clipper.DisplayEnd; row++)
                        {
                            var availableEmote = FilteredEmotes[row];
                            var isSelected = availableEmote.RowId == selectedEmote?.RowId;
                            ImGui.TableNextRow();

                            ImGui.TableSetColumnIndex(0);
                            if (ImGui.Selectable($"{availableEmote.RowId}", isSelected,
                                    ImGuiSelectableFlags.SpanAllColumns))
                            {
                                emoteId = availableEmote.RowId;
                                ClosePopup();
                                return true;
                            }

                            if (isSelected)
                                ImGui.SetItemDefaultFocus();

                            ImGui.TableSetColumnIndex(1);
                            if (!availableEmote.Name.IsEmpty)
                                ImGui.Text($"{availableEmote.Name.ExtractText()}");
                            else
                                ImGui.TextColored(ImGuiColors.DalamudGrey, "unknown");

                            ImGui.TableSetColumnIndex(2);
                            var command = availableEmote.TextCommand.ValueNullable?.Command;
                            if (command is { IsEmpty: false })
                                ImGui.Text($"{command.Value.ExtractText()}");
                            else
                                ImGui.TextColored(ImGuiColors.DalamudGrey, "unknown");
                        }
                    }
                    clipper.Destroy();
                }

                ImGui.EndTable();
            }

            if (ImGui.Button("Cancel"))
            {
                ClosePopup();
            }

            ImGui.EndPopup();
        }

        emoteId = selectedEmote?.RowId ?? 0;
        return false;
    }

    /// <summary>
    /// Save the search string to local property, and refresh the filtered emote list.
    /// </summary>
    /// <param name="searchString">A string to search emotes for</param>
    private void SetSearchString(string searchString)
    {
        _emoteSearch = searchString.Trim();
    }

    /// <summary>
    /// Gets the 
    /// </summary>
    private ImmutableList<Emote> FilteredEmotes
    {
        get
        {
            // If the search value is unchanged, and the sorting hasn't changed,
            // then we can re-use the existing filtered list.
            if (this._emoteSearch == this._currentFilter && this._filterSortDirty == false)
                return this._filteredEmotes;

            // Otherwise, we need to regenerate the filter list.
            this._filteredEmotes = _emoteSearch == ""
                ? _sortedEmotes
                : _sortedEmotes.Where((emote => DoesSearchStringMatchEmote(_emoteSearch, emote))).ToImmutableList();

            // Update internal tracking so we know the filtering was completed.
            this._currentFilter = this._emoteSearch;
            this._filterSortDirty = false;
            return this._filteredEmotes;
        }
    }

    /// <summary>
    /// Close the popup and reset state
    /// </summary>
    private void ClosePopup()
    {
        ImGui.CloseCurrentPopup();
        SetSearchString("");
    }

    private static bool DoesSearchStringMatchEmote(string searchString, Emote e)
    {
        return e.Name.ExtractText().Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
            (e.TextCommand.ValueNullable?.Command.ExtractText()
                .Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (e.TextCommand.ValueNullable?.ShortCommand.ExtractText()
                .Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (e.TextCommand.ValueNullable?.ShortAlias.ExtractText()
                .Contains(searchString, StringComparison.OrdinalIgnoreCase) ?? false) || (e.RowId
                .ToString().Contains(searchString, StringComparison.OrdinalIgnoreCase));
    }

    private void ApplyEmoteTableSorting(ImGuiTableSortSpecsPtr sortSpecs)
    {
        // The pointer provided by ImGui keeps track of whether other not the sorting has changed,
        // so we only need to re-sort the emotes if the pointer has changed.
        if (!sortSpecs.SpecsDirty) return;
        _sortedEmotes = _sortedEmotes.Sort(((a, b) => TableSortEmoteComparison(a, b, sortSpecs)));

        // Mark the sorting as having been updated.
        sortSpecs.SpecsDirty = false;
        // Note that we need to regenerate the _filteredEmotes list based on the new sorting. 
        this._filterSortDirty = true;
    }

    private static int TableSortEmoteComparison(Emote a, Emote b, ImGuiTableSortSpecsPtr sortSpecs)
    {
        for (var n = 0; n < sortSpecs.SpecsCount; n++)
        {
            unsafe
            {
                var sortSpec = sortSpecs.Specs.NativePtr[n];

                var delta = 0;
                switch (sortSpec.ColumnUserID)
                {
                    case (uint)EmoteTableColumn.Id: delta = ((int)a.RowId - (int)b.RowId); break;
                    case (uint)EmoteTableColumn.Name:
                        {
                            if (a.Name.IsEmpty && b.Name.IsEmpty)
                                delta = 0;
                            else if (a.Name.IsEmpty && !b.Name.IsEmpty)
                                delta = 1;
                            else if (!a.Name.IsEmpty && b.Name.IsEmpty)
                                delta = -1;
                            else
                                delta = string.Compare(a.Name.ExtractText(), b.Name.ExtractText(),
                                    StringComparison.OrdinalIgnoreCase);
                            break;
                        }
                    case (uint)EmoteTableColumn.Command:
                        {
                            if (a.TextCommand.ValueNullable == null &&
                                b.TextCommand.ValueNullable == null)
                                delta = 0;
                            else if (a.TextCommand.ValueNullable == null &&
                                     b.TextCommand.ValueNullable != null)
                                delta = 1;
                            else if (a.TextCommand.ValueNullable != null &&
                                     b.TextCommand.ValueNullable == null)
                                delta = -1;
                            else
                                delta = string.Compare(a.TextCommand.Value.Command.ExtractText(),
                                    b.TextCommand.Value.Command.ExtractText(), StringComparison.OrdinalIgnoreCase);
                            break;
                        }
                }

                if (delta > 0)
                    return sortSpec.SortDirection == ImGuiSortDirection.Ascending ? 1 : -1;
                else if (delta < 0)
                    return sortSpec.SortDirection == ImGuiSortDirection.Ascending ? -1 : 1;
            }
        }

        return (int)a.RowId - (int)b.RowId;
    }
}

internal enum EmoteTableColumn : uint
{
    Id = 1,
    Name = 2,
    Command = 3
}