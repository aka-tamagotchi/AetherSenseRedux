using AetherSenseRedux.Pattern;
using AetherSenseRedux.Trigger;
using Dalamud.Logging;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AetherSenseRedux
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    class PluginUI : IDisposable
    {
        private Configuration configuration;
        private Plugin plugin;

        // this extra bool exists for ImGui, since you can't ref a property but you can ref a field

        private bool settingsVisible = false;
        public bool SettingsVisible
        {
            get { return settingsVisible; }
            set { settingsVisible = value; }
        }

        // C# doesn't have static variables so we fake it by making values that need to be static into fields
        private int SelectedTrigger = 0;

        // In order to keep the UI from trampling all over the configuration as changes are being made, we keep a working copy here when needed.
        private Configuration? WorkingCopy;

        public PluginUI(Configuration configuration, Plugin plugin)
        {
            this.configuration = configuration;
            this.plugin = plugin;
        }

        /// <summary>
        /// Would dispose of any unmanaged resources if we used any here. Since we don't, we probably don't need this.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Draw handler for plugin UI
        /// </summary>
        public void Draw()
        {
            // This is our only draw handler attached to UIBuilder, so it needs to be
            // able to draw any windows we might have open.
            // Each method checks its own visibility/state to ensure it only draws when
            // it actually makes sense.
            // There are other ways to do this, but it is generally best to keep the number of
            // draw delegates as low as possible.

            DrawSettingsWindow();
        }

        /// <summary>
        /// Draws the settings window and does a little housekeeping with the working copy of the config since imgui encourages mixing UI and logic. 
        /// </summary>
        private void DrawSettingsWindow()
        {
            if (!SettingsVisible)
            {

                // if we aren't drawing the window we don't need a working copy of the configuration
                if (WorkingCopy != null)
                {
                    PluginLog.Debug("Making WorkingCopy null.");
                    WorkingCopy = null;
                }

                return;
            }

            // we can only get here if we know we're going to draw the settings window, so let's get our working copy back

            if (WorkingCopy == null)
            {
                PluginLog.Debug("WorkingCopy was null, importing current config.");
                WorkingCopy = new Configuration();
                WorkingCopy.Import(configuration);
            }

            ////
            ////    SETTINGS WINDOW
            ////
            ImGui.SetNextWindowSize(new Vector2(640, 400), ImGuiCond.Appearing);
            if (ImGui.Begin("AetherSense Redux", ref settingsVisible, ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.MenuBar))
            {

                ////
                ////    MENU BAR
                ////
                if (ImGui.BeginMenuBar())
                {
                    if (ImGui.BeginMenu("File"))
                    {
                        ImGui.Selectable("Import...", false, ImGuiSelectableFlags.Disabled);
                        ImGui.Selectable("Export...", false, ImGuiSelectableFlags.Disabled);
                        ImGui.EndMenu();
                    }
                    ImGui.EndMenuBar();
                }

                ////
                ////    BODY
                ////
                ImGui.BeginChild("body", new Vector2(0, -ImGui.GetFrameHeightWithSpacing()), false);
                
                ImGui.Indent(1); //for some reason the UI likes to cut off a pixel on the left side if we don't do this

                if (ImGui.BeginTabBar("MyTabBar", ImGuiTabBarFlags.None))
                {
                    if (ImGui.BeginTabItem("Intiface"))
                    {
                        var address = WorkingCopy.Address;
                        if (ImGui.InputText("Intiface Address", ref address, 64))
                        {
                            WorkingCopy.Address = address;
                        }
                        ImGui.SameLine();
                        if (plugin.Running)
                        {
                            if (ImGui.Button("Disconnect"))
                            {
                                plugin.Stop();
                            }
                        }
                        else
                        {
                            if (ImGui.Button("Connect"))
                            {
                                configuration.Address = WorkingCopy.Address;
                                plugin.Start();
                            }
                        }
                        ImGui.Spacing();
                        ImGui.BeginChild("status", new Vector2(0, 0), true);

                        ImGui.EndChild();
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Triggers"))
                    {
                        var listToRemove = new List<dynamic>();
                        if (SelectedTrigger >= WorkingCopy.Triggers.Count)
                        {
                            SelectedTrigger = (SelectedTrigger > 0) ? WorkingCopy.Triggers.Count - 1 : 0;
                        }
                        ImGui.BeginChild("left", new Vector2(150, -ImGui.GetFrameHeightWithSpacing()), true);
                            
                        foreach (var (t, i) in WorkingCopy.Triggers.Select((value, i) => (value, i)))
                        {
                            ImGui.PushID(i); // We push the iterator to the ID stack so multiple triggers of the same type and name are still distinct
                            if (ImGui.Selectable(String.Format("{0} ({1})", t.Name, t.Type), SelectedTrigger == i))
                            {
                                SelectedTrigger = i;
                            }
                            ImGui.PopID();
                        }

                        ImGui.EndChild();
                            
                        ImGui.SameLine();
                            
                        ImGui.BeginChild("right", new Vector2(0, -(ImGui.GetFrameHeightWithSpacing() * 2)), false);

                        if (WorkingCopy!.Triggers.Count == 0)
                        {
                            ImGui.Text("Use the Add New button to add a trigger.");

                        } 
                        else
                        {
                            DrawChatTriggerConfig(WorkingCopy.Triggers[SelectedTrigger]);
                        }
                            
                        ImGui.EndChild();
                            



                        if (ImGui.Button("Add New"))
                        {
                            List<dynamic> triggers = WorkingCopy.Triggers;
                            triggers.Add(new ChatTriggerConfig()
                            {
                                PatternSettings = new ConstantPatternConfig()
                            });
                        }
                        ImGui.SameLine();
                        if (ImGui.Button("Remove"))
                        {
                            WorkingCopy.Triggers.RemoveAt(SelectedTrigger);
                        }
                        ImGui.EndTabItem();
                    }
                    if (ImGui.BeginTabItem("Advanced"))
                    {
                        var configValue = WorkingCopy.LogChat;
                        if (ImGui.Checkbox("Log Chat to Debug", ref configValue))
                        {
                            WorkingCopy.LogChat = configValue;

                        }
                        if (ImGui.Button("Restore Default Triggers"))
                        {
                            WorkingCopy.LoadDefaults();
                        }
                        ImGui.EndTabItem();
                    }
                    ImGui.EndTabBar();
                }

                ImGui.Unindent(1); //for some reason the UI likes to cut off a pixel on the left side if we don't do this

                ImGui.EndChild();
                
                ////
                ////    FOOTER
                ////
                // save button
                if (ImGui.Button("Save"))
                {
                    configuration.Import(WorkingCopy);
                    configuration.Save();
                    plugin.Restart();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Save configuration changes to disk.");
                }
                // end save button
                ImGui.SameLine();
                // apply button
                if (ImGui.Button("Apply"))
                {
                    configuration.Import(WorkingCopy);
                    plugin.Restart();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Apply configuration changes without saving.");
                }
                // end apply button
                ImGui.SameLine();
                // revert button
                if (ImGui.Button("Revert"))
                {
                    try
                    {
                        var cloneconfig = configuration.CloneConfigurationFromDisk();
                        configuration.Import(cloneconfig);
                        WorkingCopy.Import(configuration);
                    }
                    catch (Exception ex)
                    {
                        PluginLog.Error(ex, "Could not restore configuration.");
                    }

                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("Discard all changes and reload the configuration from disk.");
                }
                // end revert button
            }

            ImGui.End();
        }


        /// <summary>
        /// Draws the configuration interface for chat triggers
        /// </summary>
        /// <param name="t">A ChatTriggerConfig object containing the current configuration for the trigger.</param>
        private void DrawChatTriggerConfig(dynamic t)
        {
            if (ImGui.BeginTabBar("TriggerConfig", ImGuiTabBarFlags.None))
            {
                
                
                if (ImGui.BeginTabItem("Basic"))
                {

                    //begin name field
                    var name = (string)t.Name;
                    if (ImGui.InputText("Name", ref name, 64))
                    {
                        t.Name = name;
                    }
                    //end name field

                    //begin regex field
                    var regex = (string)t.Regex;
                    if (ImGui.InputText("Regex", ref regex, 255))
                    {
                        t.Regex = regex;
                    }
                    //end regex field

                    //begin retrigger delay field
                    var retriggerdelay = (int)t.RetriggerDelay;
                    if (ImGui.InputInt("Retrigger Delay (ms)", ref retriggerdelay))
                    {
                        t.RetriggerDelay = (long)retriggerdelay;
                    }
                    //end retrigger delay field

                    ImGui.EndTabItem();
                }

                ////
                ////    DEVICES TAB
                ////
                if (ImGui.BeginTabItem("Devices"))
                {
                    
                    //Begin enabled devices selection
                    WorkingCopy!.SeenDevices = new List<string>(configuration.SeenDevices);
                    if (WorkingCopy.SeenDevices.Count > 0)
                    {
                        bool[] selected = new bool[WorkingCopy.SeenDevices.Count];
                        bool modified = false;
                        foreach (var (device, j) in WorkingCopy.SeenDevices.Select((value, i) => (value, i)))
                        {
                            if (t.EnabledDevices.Contains(device))
                            {
                                selected[j] = true;
                            }
                            else
                            {
                                selected[j] = false;
                            }
                        }
                        if (ImGui.BeginListBox("Enabled Devices"))
                        {
                            foreach (var (device, j) in WorkingCopy.SeenDevices.Select((value, i) => (value, i)))
                            {
                                if (ImGui.Selectable(device, selected[j]))
                                {
                                    selected[j] = !selected[j];
                                    modified = true;
                                }
                            }
                            ImGui.EndListBox();
                        }
                        if (modified)
                        {
                            var toEnable = new List<string>();
                            foreach (var (device, j) in WorkingCopy.SeenDevices.Select((value, i) => (value, i)))
                            {
                                if (selected[j])
                                {
                                    toEnable.Add(device);
                                }
                            }
                            t.EnabledDevices = toEnable;
                        }
                    } else
                    {
                        ImGui.Text("Connect to Intiface and connect devices to populate the list.");
                    }
                    //end enabled devices selection
                    
                    ImGui.EndTabItem();
                }
                
                ////
                ////    FILTERS TAB
                ////
                if (ImGui.BeginTabItem("Filters"))
                {
                    ImGui.EndTabItem();
                }
                
                ////
                ////    PATTERN TAB
                ////
                if (ImGui.BeginTabItem("Pattern"))
                {
                    string[] patterns = { "Constant", "Ramp", "Random", "Square", "Saw"};

                    //begin pattern selection
                    if (ImGui.BeginCombo("##combo",t.PatternSettings.Type))
                    {
                        foreach (string pattern in patterns)
                        {
                            bool is_selected = t.PatternSettings.Type == pattern;
                            if(ImGui.Selectable(pattern, is_selected))
                            {
                                if (t.PatternSettings.Type != pattern)
                                {
                                    t.PatternSettings = PatternFactory.GetDefaultsFromString(pattern);
                                }
                            }
                            if (is_selected)
                            {
                                ImGui.SetItemDefaultFocus();
                            }
                        }
                        ImGui.EndCombo();
                    }
                    //end pattern selection

                    ImGui.SameLine();
                    
                    //begin test button
                    if (ImGui.ArrowButton("test", ImGuiDir.Right))
                    {
                        plugin.DoPatternTest(t.PatternSettings);
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("Preview pattern on all devices.");
                    }
                    //end test button

                    ImGui.Indent();
                    
                    //begin pattern settings
                    switch ((string)t.PatternSettings.Type)
                    {
                        case "Constant":
                            DrawConstantPatternSettings(t.PatternSettings);
                            break;
                        case "Ramp":
                            DrawRampPatternSettings(t.PatternSettings);
                            break;
                        case "Saw":
                            DrawSawPatternSettings(t.PatternSettings);
                            break;
                        case "Random":
                            DrawRandomPatternSettings(t.PatternSettings);
                            break;
                        case "Square":
                            DrawSquarePatternSettings(t.PatternSettings);
                            break;
                        default:
                            //we should never get here but just in case
                            ImGui.Text("Select a valid pattern.");
                            break;
                    }
                    //end pattern settings
                    
                    ImGui.Unindent();

                    ImGui.EndTabItem();
                }
                
                
                ImGui.EndTabBar();
            }
        }

        /// <summary>
        /// Draws the configuration interface for constant patterns
        /// </summary>
        /// <param name="pattern">A ConstantPatternConfig object containing the current configuration for the pattern.</param>
        private static void DrawConstantPatternSettings(dynamic pattern)
        {
            int duration = (int)pattern.Duration;
            if (ImGui.InputInt("Duration (ms)", ref duration))
            {
                pattern.Duration = (long)duration;
            }
            double level = (double)pattern.Level;
            if (ImGui.InputDouble("Level", ref level))
            {
                pattern.Level = level;
            }
        }

        /// <summary>
        /// Draws the configuration interface for ramp patterns
        /// </summary>
        /// <param name="pattern">A RampPatternConfig object containing the current configuration for the pattern.</param>
        private static void DrawRampPatternSettings(dynamic pattern)
        {
            int duration = (int)pattern.Duration;
            if (ImGui.InputInt("Duration (ms)", ref duration))
            {
                pattern.Duration = (long)duration;
            }
            double start = (double)pattern.Start;
            if (ImGui.InputDouble("Start", ref start))
            {
                pattern.Start = start;
            }
            double end = (double)pattern.End;
            if (ImGui.InputDouble("End", ref end))
            {
                pattern.End = end;
            }
        }

        /// <summary>
        /// Draws the configuration interface for saw patterns
        /// </summary>
        /// <param name="pattern">A SawPatternConfig object containing the current configuration for the pattern.</param>
        private static void DrawSawPatternSettings(dynamic pattern)
        {
            int duration = (int)pattern.Duration;
            if (ImGui.InputInt("Duration (ms)", ref duration))
            {
                pattern.Duration = (long)duration;
            }
            double start = (double)pattern.Start;
            if (ImGui.InputDouble("Start", ref start))
            {
                pattern.Start = start;
            }
            double end = (double)pattern.End;
            if (ImGui.InputDouble("End", ref end))
            {
                pattern.End = end;
            }
            int duration1 = (int)pattern.Duration1;
            if (ImGui.InputInt("Saw Duration (ms)", ref duration1))
            {
                pattern.Duration1 = (long)duration1;
            }
        }

        /// <summary>
        /// Draws the configuration interface for random patterns
        /// </summary>
        /// <param name="pattern">A RandomPatternConfig object containing the current configuration for the pattern.</param>
        private static void DrawRandomPatternSettings(dynamic pattern)
        {
            int duration = (int)pattern.Duration;
            if (ImGui.InputInt("Duration (ms)", ref duration))
            {
                pattern.Duration = (long)duration;
            }
            double min = (double)pattern.Minimum;
            if (ImGui.InputDouble("Minimum", ref min))
            {
                pattern.Minimum = min;
            }
            double max = (double)pattern.Maximum;
            if (ImGui.InputDouble("Maximum", ref max))
            {
                pattern.Maximum = max;
            }
        }

        /// <summary>
        /// Draws the configuration interface for square patterns
        /// </summary>
        /// <param name="pattern">A SquarePatternConfig object containing the current configuration for the pattern.</param>
        private static void DrawSquarePatternSettings(dynamic pattern)
        {
            int duration = (int)pattern.Duration;
            if (ImGui.InputInt("Duration (ms)", ref duration))
            {
                pattern.Duration = (long)duration;
            }
            double level1 = (double)pattern.Level1;
            if (ImGui.InputDouble("Level 1", ref level1))
            {
                pattern.Level1 = level1;
            }
            int duration1 = (int)pattern.Duration1;
            if (ImGui.InputInt("Level 1 Duration (ms)", ref duration1))
            {
                pattern.Duration1 = (long)duration1;
            }
            double level2 = (double)pattern.Level2;
            if (ImGui.InputDouble("Level 2", ref level2))
            {
                pattern.Level2 = level2;
            }
            int duration2 = (int)pattern.Duration2;
            if (ImGui.InputInt("Level 2 Duration (ms)", ref duration2))
            {
                pattern.Duration2 = (long)duration2;
            }
            int offset = (int)pattern.Offset;
            if (ImGui.InputInt("Offset (ms)", ref offset))
            {
                pattern.Offset = (long)offset;
            }
        }
    }
}
