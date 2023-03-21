using System;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    public class LogEntryView : MonoBehaviour
    {
        [SerializeField] Text message;
        [SerializeField] Text stacktrace;
        [SerializeField] GameObject generalErrorParent;
        [SerializeField] GameObject selectedParent;
#if DEBUG_LOGS
        public LogEntry log { get; private set; }
        Action<LogEntry> onSelected;
        private void Awake()
        {
            SetSelected(false);
        }
        public void Show(LogEntry log, Action<LogEntry> onSelected = null)
        {
            this.log = log;
            this.onSelected = onSelected;
            if (message != null)
                message.text = log.logString;
            if (stacktrace != null)
                stacktrace.text = log.stackTrace;
            if (generalErrorParent != null)
            {
                var isGeneralError = log.type == LogType.Error
                    || log.type == LogType.Exception || log.type == LogType.Assert;
                generalErrorParent.SetActive(isGeneralError);
            }
        }
        public void OnCopyPressed()
        {
            log.ToString().CopyToClipboard();
        }
        public void OnSelected()
        {
            onSelected.Invoke(log);
        }
        public void SetSelected(bool selected)
        {
            if (selectedParent != null)
                selectedParent.SetActive(selected);
        }
#endif
    }
}
