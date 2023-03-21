using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class LogsDebugView : FGTModuleDebugPanel
    {
        public override string tab => "Logs";
        public override string module => "DEBUG_LOGS";
        public override bool wholeTab => true;

        LogsCapturer logs => LogsCapturer.instance;
        [SerializeField] LogEntryView prefab;
        [SerializeField] Transform itemsParent;
        [SerializeField] LogEntryView selectedView;
        [SerializeField] TextMeshProUGUI buildInfo;
        [SerializeField] TextMeshProUGUI systemInfo;
        [SerializeField] TextMeshProUGUI emailingStatus;

#if DEBUG_LOGS
        public override void OnDebugPanelAwake() => LogsCapturer.EnsureExists();
        List<LogEntryView> views = new List<LogEntryView>();

        private void Start()
        {
            if (selectedView != null)
                selectedView.Show(new LogEntry { logString = "", stackTrace = "", type = LogType.Log });
            prefab.gameObject.SetActive(false);
            if (buildInfo != null)
                buildInfo.text = BuildInfoManager.buildInfo;
            if (systemInfo != null)
                systemInfo.text = $"{SystemInfo.deviceModel}, {Application.installMode}, {Application.installerName}";
        }

        protected override void Update()
        {
            base.Update();
            if (!Application.isPlaying) return;
            Utils.UpdatePrefabsList(views, logs.capturedLogs, prefab, itemsParent, 
                (log, view) => view.Show(log, OnLogSelected));
        }
        private void OnLogSelected(LogEntry log)
        {
            for (int i = 0; i < views.Count && views[i].gameObject.activeSelf; i++)
                views[i].SetSelected(views[i].log == log);
            if (selectedView != null)
                selectedView.Show(log);
        }
        public async void EmailLogs()
        {
            if (emailingStatus != null)
                emailingStatus.text = "sending email...";
            var error = await logs.EmailLogs();
            if (emailingStatus != null)
                emailingStatus.text = string.IsNullOrEmpty(error) ? "email sent successfully" : error;
        }
        public void CopyAllLogs() => logs.CopyAllLogs();
        public void ClearLogs() => logs.ClearLogs();
#endif
    }
}
