using FriendsGamesTools.UI;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public class NewVersionWindow : Window
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI whatsNew;
#if ECS_SAVE_MIGRATION
        static MigrationSettings settings => MigrationSettings.instance;
        static NewVersionWindow instance;
        public static void Show(int versionFrom, int versionTo)
        {
            var inst = instance ?? (instance = Windows.Get(settings.newVersionWindow.prefab));
            var complete = versionFrom >= versionTo;
            if (complete) {
                inst.shown = false;
                return;
            }
            var currMigrationHasWhatsNew = !string.IsNullOrEmpty(settings.Get(versionFrom + 1).whatsNew);
            if (!currMigrationHasWhatsNew)
                Show(versionFrom + 1, versionTo);
            else
                inst.ShowFromTo(versionFrom, versionTo);
        }
        int versionFrom, versionTo;
        void ShowFromTo(int versionFrom, int versionTo)
        {
            shown = true;
            this.versionFrom = versionFrom;
            this.versionTo = versionTo;
            var currVersionConfig = settings.Get(versionFrom + 1);
            if (title != null)
                title.text = $"NEW VERSION {currVersionConfig.shownVersion}";
            if (whatsNew != null)
                whatsNew.text = currVersionConfig.whatsNew;
        }
        public override void OnClosePressed() => Show(versionFrom + 1, versionTo);
#endif
    }
}
