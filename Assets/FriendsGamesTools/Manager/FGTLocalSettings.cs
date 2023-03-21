using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools
{
    public class FGTLocalSettings : SettingsScriptable<FGTLocalSettings>
    {
        protected override bool inRepository => false;

        public bool showAllModules;
        public bool alphabetOrder;
        public string shownParentModuleDefine;
        public bool FGTEditable;

        public bool updatesShown = true;
        public bool updatesShowOnlyNotCompleted = true;
        public bool updatesShowOnlyRelevant = true;

        public bool wizardShown = true;
        public int wizardQuestionInd;
        public List<bool> wizardAnswers = new List<bool>();

        public GameObject openedPrefab;
    }
}