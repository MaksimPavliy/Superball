
namespace FriendsGamesTools.DebugTools.ChromaKey
{
    public class ChromaKeyModule : DebugToolsModule
    {
        public const string define = "CHROMA_KEY";
        public override string Define => define;
        public override HowToModule HowTo() => new ChromaKeyModule_HowTo();
        protected override string debugViewPath => "DebugTools/CHROMA_KEY/ChromaKeyDebugView";

#if CHROMA_KEY
        protected override void OnCompiledEnable()
        {
            base.OnCompiledEnable();
            SettingsInEditor<ChromaKeySettings>.EnsureExists();
        }
#endif
    }
}