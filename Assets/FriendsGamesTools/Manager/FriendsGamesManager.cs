namespace FriendsGamesTools
{
    public class FriendsGamesManager : MonoBehaviourHasInstance<FriendsGamesManager>
    {
        public const string AssetsFolder = "Assets";
        public const string MainPluginName = "FriendsGamesTools";
        public const string MainPluginFolder = AssetsFolder + "/" + MainPluginName;
        public const string GeneratedFolder = MainPluginFolder + "Generated";
        protected override void Awake()
        {
            if (instance!=null)
            {
                Destroy(gameObject);
                return;
            }
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}