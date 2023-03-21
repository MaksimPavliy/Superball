#if FLURRY
using FlurrySDK;

namespace FriendsGamesTools.Integrations
{
    public class FlurryManager : IntegrationManager<FlurryManager>
    {
        public string ModuleName => "FLURRY";
        void Start()
        {
            new Flurry.Builder()
                  .WithCrashReporting(true)
                  .WithLogEnabled(true)
                  .WithLogLevel(Flurry.LogLevel.VERBOSE)
                  .WithMessaging(true)
                  .Build(FlurrySettings.instance.key);
        }
    }
}
#elif SDKs
namespace FriendsGamesTools.Integrations
{
    public class FlurryManager : IntegrationManager<FlurryManager>
    {
    }
}
#endif