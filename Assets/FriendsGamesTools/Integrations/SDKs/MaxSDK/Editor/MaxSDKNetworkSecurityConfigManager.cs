#if MAX_SDK

namespace FriendsGamesTools.Integrations.MaxSDK
{
    public class MaxSDKNetworkSecurityConfigManager : XMLFileManager
    {
        const string path = "Assets/Plugins/Android/res/xml/network_security_config.xml";
        protected override string defaultContents => 
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
            "<network-security-config>\n" +
            "</network-security-config>";
        public MaxSDKNetworkSecurityConfigManager() 
            : base(path) { }


        /*
<?xml version="1.0" encoding="utf-8"?>
<network-security-config>
    <!-- For AdColony and Smaato - all cleartext traffic allowed -->
    <base-config cleartextTrafficPermitted="true">
        <trust-anchors>
            <certificates src="system"/>
        </trust-anchors>
    </base-config>
    <!-- End AdColony cleartext requirement -->
    <domain-config cleartextTrafficPermitted="true">
        <!-- For Facebook -->
        <domain includeSubdomains="true">127.0.0.1</domain>

        <!-- For Amazon -->
        <domain includeSubdomains="true">amazon-adsystem.com</domain>
    </domain-config>
</network-security-config>   
    */
    }
}
#endif