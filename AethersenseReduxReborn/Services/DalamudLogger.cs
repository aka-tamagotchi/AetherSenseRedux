using System;
using Dalamud.Logging;

namespace AethersenseReduxReborn.Services;

public class DalamudLogger
{
    public void Verbose(string         messageTemplate, params object[] values)                                  { PluginLog.Verbose(messageTemplate, values); }
    public void Verbose(Exception?     exception,       string          messageTemplate, params object[] values) { PluginLog.Verbose(exception,       messageTemplate, values); }
    public void Debug(string           messageTemplate, params object[] values)                                  { PluginLog.Debug(messageTemplate, values); }
    public void Debug(Exception?       exception,       string          messageTemplate, params object[] values) { PluginLog.Debug(exception,       messageTemplate, values); }
    public void Information(string     messageTemplate, params object[] values)                                  { PluginLog.Information(messageTemplate, values); }
    public void Information(Exception? exception,       string          messageTemplate, params object[] values) { PluginLog.Information(exception,       messageTemplate, values); }
    public void Warning(string         messageTemplate, params object[] values)                                  { PluginLog.Warning(messageTemplate, values); }
    public void Warning(Exception?     exception,       string          messageTemplate, params object[] values) { PluginLog.Warning(exception,       messageTemplate, values); }
    public void Error(string           messageTemplate, params object[] values)                                  { PluginLog.Error(messageTemplate, values); }
    public void Error(Exception?       exception,       string          messageTemplate, params object[] values) { PluginLog.Error(exception,       messageTemplate, values); }
}
