using UnityEditor;
using System.Collections.Generic;

namespace FriendsGamesTools
{
    public abstract class ModulesFolder : ModuleManager
    {
        public override string parentModule => FGTRootModule.define;
        public override bool canBeEnabled => false;
        protected List<ModuleManager> compiledItems;
        void InitItemsIfNeeded() 
        {
            if (compiledItems != null) return;
            compiledItems = FriendsGamesToolsWindow.allModules.Filter(m => m.parentModule == Define)
                .Filter(i => i.compiled);
        }
        protected override string collapsedDescription
        {
            get
            {
                string str = string.Empty;
                if (compiledItems?.Count > 0)
                    str = $"{compiledItems.ConvertAll(i => $"<b>{i.Define}</b>").PrintCollection(", ")} ";
                str += base.collapsedDescription;
                return str;
            }
        }
        protected override void OnCollapsedGUI()
        {
            base.OnCollapsedGUI();
            InitItemsIfNeeded();
        }
    }  
}


