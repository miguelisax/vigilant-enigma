﻿﻿// This is an automatically generated file, based on settings.json and PackageSettingsGen.tt
/* settings.json content:
{
  "settings": [
    {
      "name": "CollectMetrics",
      "type": "bool",
      "default": 'true'
    },
    {
      "name": "UIState",
      "type": "object",
      "typename": "UIState",
      "default": "null"
    },
    {
      "name": "HideTeamExplorerWelcomeMessage",
      "type": "bool",
      "default": "false"
    },
    {
      "name": "EnableTraceLogging",
      "type": "bool",
      "default": "false"
    }
  ]
}
*/

using System.ComponentModel;

namespace GitHub.Settings
{
    public interface IPackageSettings : INotifyPropertyChanged
    {
        void Save();
        bool CollectMetrics { get; set; }
        UIState UIState { get; set; }
        bool HideTeamExplorerWelcomeMessage { get; set; }
        bool EnableTraceLogging { get; set; }
    }
}