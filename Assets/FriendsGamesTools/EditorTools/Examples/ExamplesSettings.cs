#if EXAMPLES
using System;
using System.Collections.Generic;

namespace FriendsGamesTools.Examples
{
    public class ExamplesSettings : SettingsScriptable<ExamplesSettings>
    {
        [Serializable]
        public class Example
        {
            public SceneField scene;
            public List<string> forModules;
        }
        public List<Example> examples;
        protected override bool inFGTGeneratedFolder => false;
        protected override string SubFolder => "Examples";
        protected override bool inResources => false;
    }
}
#endif