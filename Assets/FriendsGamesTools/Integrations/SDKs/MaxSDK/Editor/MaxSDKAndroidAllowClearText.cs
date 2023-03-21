#if MAX_SDK

namespace FriendsGamesTools.Integrations.MaxSDK
{
    public class MaxSDKAndroidAllowClearText
    {
        public enum Type { AdColony, FB, Amazon }
        Type type;
        public MaxSDKAndroidAllowClearText(Type type) => this.type = type;

        const string SecurityParamName = "android:networkSecurityConfig";
        const string SecurityParamValue = "@xml/network_security_config";
        string[] SecurityParamPath = new string[] { "manifest", "application" };
        const string mainTag = "network-security-config";
        const string domainConfigTag = "domain-config";

        const string AdColonyText =
            "\n" +
            "    <!-- For AdColony and Smaato - all cleartext traffic allowed -->\n" +
            "    <base-config cleartextTrafficPermitted=\"true\">\n" +
            "        <trust-anchors>\n" +
            "            <certificates src=\"system\"/>\n" +
            "        </trust-anchors>\n" +
            "    </base-config>\n" +
            "    <!-- End AdColony cleartext requirement -->\n";
        const string FBText =
                "\n" +
                "        <!-- For Facebook -->\n" +
                "        <domain includeSubdomains=\"true\">127.0.0.1</domain>";
        const string AmazonText =
                "\n" +
                "        <!-- For Amazon -->\n" +
                "        <domain includeSubdomains=\"true\">amazon-adsystem.com</domain>";

        public bool GetCompleted()
        {
            var manifest = new AndroidManifestManager();
            if (!manifest.exists)
                return false;
            var securityParam = manifest.GetParam(SecurityParamName, SecurityParamPath);
            if (SecurityParamValue != securityParam)
                return false;

            var security = new MaxSDKNetworkSecurityConfigManager();
            string text;
            switch (type)
            {
                default:
                case Type.AdColony: text = AdColonyText; break;
                case Type.FB: text = FBText; break;
                case Type.Amazon: text = AmazonText; break;
            }
            return security.contents.Contains(text);
        }
        public void DoSetup()
        {
            var manifest = new AndroidManifestManager();
            manifest.ReplaceParam(SecurityParamValue, SecurityParamName, SecurityParamPath);
            manifest.Save();

            var security = new MaxSDKNetworkSecurityConfigManager();
            if (type == Type.AdColony)
            {
                security.EnsureExistsInLongTag(AdColonyText, mainTag);
            }
            else
            {
                if (!security.TagExists(mainTag, domainConfigTag))
                    security.AddTagLong(domainConfigTag, "cleartextTrafficPermitted=\"true\"", mainTag);
                if (type == Type.FB)
                    security.EnsureExistsInLongTag(FBText, mainTag, domainConfigTag);
                if (type == Type.Amazon)
                    security.EnsureExistsInLongTag(AmazonText, mainTag, domainConfigTag);
            }
            security.Save();
        }
    }
}
#endif