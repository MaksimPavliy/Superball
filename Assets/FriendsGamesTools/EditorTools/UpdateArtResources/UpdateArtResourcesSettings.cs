namespace FriendsGamesTools
{
    public class UpdateArtResourcesSettings : SettingsScriptable<UpdateArtResourcesSettings>
    {
        protected override bool inResources => false;
        protected override bool inRepository => false;

        public const string examplePath = @"D:\folder\folder";
        public string folder_in_assets = examplePath;
        public string folder_external = examplePath;

        // Means that art will be copied to folder_external_postprocessed folder,
        // postprocessed there and than copied to folder_in_assets.
        public bool postProcessing = false;
        public string folder_external_postprocessed = examplePath;

        // Clipping alpha to clear pixels that have less than clipAlphaThreshold transparency.
        // It removes invisible crap and makes sprite atlas packing much tighter.
        public bool clipAlpha = false; 
        public byte clipAlphaThreshold = 10;

        public bool setupDone =>
            !string.IsNullOrEmpty(folder_in_assets) && folder_in_assets != examplePath &&
            !string.IsNullOrEmpty(folder_external) && folder_external != examplePath;
    }
}