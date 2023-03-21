using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public class LogsCapturer : MonoBehaviourSingleton<LogsCapturer>
    {
        public List<LogType> capturedTypes = new List<LogType> { LogType.Error, LogType.Exception, LogType.Assert, LogType.Log };
        public int firstLogsCount = 20;
        public int lastLogsCount = 40;
        public int firstErrorsCount = 10;
        public int lastErrorsCount = 10;

#if DEBUG_LOGS
        protected override void Awake() {
            base.Awake();
            Application.logMessageReceived += OnLogReceived;
        }
        protected override void OnDestroy() {
            base.OnDestroy();
            Application.logMessageReceived -= OnLogReceived;
        }
        public List<LogEntry> capturedLogs { get; private set; } = new List<LogEntry>();
        private void OnLogReceived(string logString, string stackTrace, LogType type) {
            if (!capturedTypes.Contains(type))
                return;
            capturedLogs.Add(new LogEntry { logString = logString, stackTrace = stackTrace, type = type });
            ClampLogsCount(type);
        }
        bool IsError(LogType type) => type == LogType.Error || type == LogType.Exception;
        void ClampLogsCount(LogType lastLogType) {
            var lastIsError = IsError(lastLogType);
            var lastMaxCount = lastIsError ? lastErrorsCount : lastLogsCount;
            int lastCount = 0;
            for (int i = 0; i < capturedLogs.Count; i++) {
                if (lastIsError != IsError(capturedLogs[i].type))
                    continue;
                lastCount++;
                if (lastCount > lastMaxCount) {
                    capturedLogs.RemoveAt(capturedLogs.Count - 1);
                    return;
                }
            }

            int countToDelete = lastCount - lastMaxCount;
            for (int i = 0; i < capturedLogs.Count; i++) {
                if (countToDelete <= 0)
                    return;
                if (lastIsError != IsError(capturedLogs[i].type))
                    continue;
                capturedLogs.RemoveAt(i);
                i--;
            }
        }
        (string title, string body) GetAllLogsString() {
            var title = $"{capturedLogs.Count} Logs";
            StringBuilder sb = new StringBuilder();
            foreach (var log in capturedLogs) {
                sb.Append(log.type);
                sb.Append(":");
                sb.AppendLine(log.logString);
                sb.AppendLine(log.stackTrace);
                sb.AppendLine();
            }
            return (title, sb.ToString());
        }
        public async Task<string> EmailLogs() {
            var (title, body) = GetAllLogsString();
            return await EmailToDevs.Send(title, body);
        }
        public void CopyAllLogs() {
            var (title, body) = GetAllLogsString();
            title = EmailToDevs.GetEmailTitle(title);
            var allLogs = $"{title}\n{body}";
            allLogs.CopyToClipboard();
        }
        public void ClearLogs() => capturedLogs.Clear();
#endif
    }
#if DEBUG_LOGS
    public class LogEntry
    {
        public string logString;
        public string stackTrace;
        public LogType type;
        public override string ToString() => $"{type}:{logString}\n{stackTrace}";
    }
#endif
}
