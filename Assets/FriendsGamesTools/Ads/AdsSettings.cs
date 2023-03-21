#if ADS

using FriendsGamesTools.EditorTools.BuildModes;

namespace FriendsGamesTools.Ads
{
    public class AdsSettings : SettingsScriptable<AdsSettings>
    {
        public bool mockedAdsSimulateNoAds;
        public string selectedManagerFullName;
        public AdSourceType typeInEditor = AdSourceType.mocked;
        public AdSourceType typeInDevelopBuild = AdSourceType.mocked;
        public AdSourceType typeInTestBuild = AdSourceType.omitShowingAndSuccess;
        public AdSourceType typeInReleaseBuild = AdSourceType.real;
        public AdSourceType typeInBuild
        {
            get
            {
                switch (BuildModeSettings.mode)
                {
                    default:
                    case BuildModeType.Develop: return typeInDevelopBuild;
                    case BuildModeType.Test: return typeInTestBuild;
                    case BuildModeType.Release: return typeInReleaseBuild;
                }
            }
            set
            {
                switch (BuildModeSettings.mode)
                {
                    default:
                    case BuildModeType.Develop: typeInDevelopBuild = value; break;
                    case BuildModeType.Test: typeInTestBuild = value; break;
                    case BuildModeType.Release: typeInReleaseBuild = value; break;
                }
            }
        }
        public bool omitAdsConfirmed = false;
    }
}
#endif