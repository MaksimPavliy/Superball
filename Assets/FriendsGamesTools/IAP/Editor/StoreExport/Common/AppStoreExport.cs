#if IAP
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools.IAP
{
    public abstract class AppStoreExport
    {
        protected IAPSettings config => SettingsInEditor<IAPSettings>.instance;
        protected AppStoresCredentials credentials => SettingsInEditor<AppStoresCredentials>.instance;
        public void OnGUI(ref bool changed)
        {
            OnCustomGUI(ref changed);
            if (state == State.Idle)
            {
                if (GUILayout.Button($"Export to {storeName}"))
                    Export();
            }
            else
            {
                switch (state)
                {
                    case State.InProgress: EditorGUIUtils.ColoredLabel($"exporting to {storeName}...", EditorGUIUtils.warningColor); break;
                    case State.Success: EditorGUIUtils.ColoredLabel($"export to {storeName} success", EditorGUIUtils.green); break;
                    case State.Failed: EditorGUIUtils.ColoredLabel($"export to {storeName} failed\n", EditorGUIUtils.red); break;
                }
            }
        }
        protected virtual void OnCustomGUI(ref bool changed) { }
        public abstract string storeName { get; }
        protected abstract Task<bool> Exporting();
        enum State { Idle, InProgress, Failed, Success }
        State state;
        async void Export()
        {
            state = State.InProgress;
            config.OnBeforeExport();
            var success = await Exporting();
            state = success ? State.Success : State.Failed;
        }
        public abstract bool ExportValid(StringBuilder sb = null);
    }
}
#endif