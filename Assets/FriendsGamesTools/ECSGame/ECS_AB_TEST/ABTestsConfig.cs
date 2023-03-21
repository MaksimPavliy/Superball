#if ECS_PLAYER
using System;
using System.Collections.Generic;

namespace FriendsGamesTools.ABTests
{
    public class ABTestsConfig : SettingsScriptable<ABTestsConfig>
    {
        public List<TestConfig> tests = new List<TestConfig>();
        public bool showOnMainWindow = false;
    }
    [Serializable]
    public class TestConfig
    {
        public string name = "";
        public bool enabled => disabledAlwaysEventInd == -1;
        public int disabledAlwaysEventInd = -1;
        public List<TestOptionConfig> options = new List<TestOptionConfig>();
        public string GetOptionName(int i) => GetOptionName(options[i]);
        public string GetOptionName(TestOptionConfig option) => $"{name}_{option.optionSuffix}";
    }
    [Serializable]
    public class TestOptionConfig {
        public string optionSuffix = "";
        public int optionMass = 1;
    }
}
#endif
