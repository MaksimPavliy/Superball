namespace FriendsGamesTools.DebugTools
{
    public class DebugLogsOnDeviceModule : DebugToolsModule
    {
        public const string define = "DEBUG_LOGS";
        public override string Define => define;
        public override HowToModule HowTo() => new DEBUG_LOGS_HowTo();
        protected override string debugViewPath => "DebugTools/DEBUG_LOGS/LogsDebugView";
    }
}