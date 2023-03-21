#if IAP
using FriendsGamesTools.EditorTools;
using FriendsGamesTools.ZipUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace FriendsGamesTools.IAP
{
    public class AppleAppStoreUploaderManager
    {
        // Install transporter:
        // https://help.apple.com/itc/transporteruserguide/#/apdAbeb95d60
        // Or direct link for windows: https://itunesconnect.apple.com/WebObjects/iTunesConnect.woa/ra/resources/download/Transporter__Windows/bin
        // For Mac OS its installed with XCode automatically, but here is separate download path.
        // https://apps.apple.com/us/app/transporter/id1450874784?mt=12
        // App metadata format doc - iap purchases format.
        // https://help.apple.com/asc/appsspec/


        ProcessLauncher transporter = new ProcessLauncher();
        AppStoresCredentials storesCredentials => SettingsInEditor<AppStoresCredentials>.instance;
        AppleAppStoreCredentials credentials => storesCredentials.appleAppStore;
        IAPSettings config => SettingsInEditor<IAPSettings>.instance;
        const string ReportsFolder = "Temp/AppleReports/";
        const string ReportType = "In-App Purchases";
        enum RequestReportType { ReportDownloaded, ReportRequested, Error }
        public int linesReceived { get; private set; }

        // The only method to execute iTMSTransporter command. Uses credentials from settings.
        async Task<(bool success, string output, string error)> Execute(string Command, string additionalArgs)
        {
            // Create command.
            var fileName = $"{credentials.transporterPath}";
            var arguments = $" -m {Command} -u {credentials.appleAppStoreLoginEmail} " +
                $"-p {credentials.appleAppStoreAppPassword} " +
                $"-account_type itunes_connect -asc_provider {config.appleTeamID}" +
                $" -WONoPause true {additionalArgs}";
            //{config.appleTeamID}

            // Execute command.
            linesReceived = 0;
            var (success, output, error) = await transporter.Execute(fileName, arguments,
                outputLine => linesReceived++, errorLine => linesReceived++);

            // Clear credentials.transporterFolder if its wrong.
            if (!success)
            {
                var wrongTransporterFolder = $"Cant launch iTMSTransporter at {credentials.transporterPath}";
                //credentials.transporterPath = "";
                EditorUtils.SetDirty(storesCredentials);
                return (false, string.Empty, wrongTransporterFolder);
            }

            return (true, output, error);
        }

        private async Task<(RequestReportType result, string reportPath)> RequestReport()
        {
            var (success, output, error) = await Execute("requestReport", $"-type \"{ReportType}\" -destination {ReportsFolder}");
            if (!success)
            {
                UnityEngine.Debug.LogError(error);
                return (RequestReportType.Error, "");
            }

            if (output.Contains("Thank you for submitting your request for a catalog report for In-App Purchases")
                || output.Contains("You will receive an email notification when your report is available for download"))
                return (RequestReportType.ReportRequested, "");

            const string SuccessString = "Report download success: ";
            if (output.Contains(SuccessString))
            {
                var lineWithPath = output.GetLineWith(SuccessString);
                var path = lineWithPath.Replace(SuccessString, "");
                return (RequestReportType.ReportDownloaded, path);
            }

            UnityEngine.Debug.LogError($"cant recognize OUTPUT:\n{output}\nERROR:\n{error}");
            return (RequestReportType.Error, "");
        }

        enum ReportStatusResult { Ready, InProgress, Error }
        private async Task<ReportStatusResult> GetReportStatus()
        {
            var (success, output, error) = await Execute("listReports", $"-type \"{ReportType}\" -destination {ReportsFolder}");
            if (!success)
            {
                UnityEngine.Debug.LogError(error);
                return ReportStatusResult.Error;
            }
            const string ReportsLine = "Report Listing:";
            var reportsStartInd = output.IndexOf(ReportsLine);
            if (reportsStartInd == -1)
            {
                UnityEngine.Debug.LogError($"cant parse listReport result, there's no '{ReportsLine}' line in\n{output}\n\n\nwhile error is\n{error}");
                return ReportStatusResult.Error;
            }

            output = output.Substring(reportsStartInd);
            var lines = output.ToLf().Split('\n');
            if (lines.Length < 3)
            {
                UnityEngine.Debug.LogError($"cant parse listReport result, output is strange:\n{output}");
                return ReportStatusResult.Error;
            }

            var reportLine = lines[2];
            var reportParts = reportLine.Split('|');
            if (reportParts.Length<4)
            {
                UnityEngine.Debug.LogError($"cant parse listReport result, reportLine is '{reportLine}'");
                return ReportStatusResult.Error;
            }

            var statusString = reportParts[3].Replace(" ", "");
            if (statusString == "Succeeded")
                return ReportStatusResult.Ready;
            else
                return ReportStatusResult.InProgress;
        }

        async Task<List<AppleIAPProductFromStore>> GetReports()
        {
            UnityEngine.Debug.Log("Requesting IAP report from Apple");
            var (result, reportPath) = await RequestReport();
            if (result == RequestReportType.Error)
                return null;

            if (result == RequestReportType.ReportRequested)
            {
                UnityEngine.Debug.Log("Report requested, waiting report");
                do
                {
                    const float RequestDelay = 10;
                    await EditorAsync.WaitForSeconds(RequestDelay);
                    var status = await GetReportStatus();
                    if (status == ReportStatusResult.Error)
                        return null;
                    if (status == ReportStatusResult.Ready)
                        break;
                    UnityEngine.Debug.Log("Still waiting report");
                } while (true);

                // Request again. 
                UnityEngine.Debug.Log("Report ready, downloading");
                (result, reportPath) = await RequestReport();
                if (result != RequestReportType.ReportDownloaded)
                    return null;
            }

            return UnzipParseReport(reportPath);
        }

        List<AppleIAPProductFromStore> UnzipParseReport(string reportPath)
        {
            var unzippedFolder = reportPath.Replace(".zip", "");
            UnityEngine.Debug.Log($"unzipping {reportPath} to {unzippedFolder}");
            Zip.UncompressZip(reportPath, unzippedFolder);
            var txtFiles = Directory.GetFiles(unzippedFolder, "*.txt");
            if (txtFiles.Length == 0)
            {
                UnityEngine.Debug.Log($"unzipped {reportPath} folder has no txt files");
                return null;
            }
            var report = File.ReadAllText(txtFiles[0]);
            var lines = report.ToLf().Split('\n');
            var linesValues = lines.ConvertAll(line => line.Split('\t'));
            linesValues.RemoveAt(0); // Remove header line.
            var products = new List<AppleIAPProductFromStore>();
            linesValues.ForEach(lineValues =>
            {
                if (lineValues.Length <= 1)
                    return;
                UnityEngine.Debug.Assert(lineValues.Length == 12);
                var product = new AppleIAPProductFromStore();
                product.APPLE_ID = lineValues[0];
                product.REFERENCE_NAME = lineValues[1];
                product.PRODUCT_ID = lineValues[2];
                product.ADDON_TYPE = lineValues[3];
                product.ADDON_ITC_STATUS = lineValues[4];
                product.AUTO_RENEWABLE_DURATION = lineValues[5];
                product.FREE_TRIAL_DURATION = lineValues[6];
                product.MARKETING_OPTIN_INCENTIVE_DURATION = lineValues[7];
                product.CLEARED_FOR_SALE = lineValues[8];
                product.APP_NAME = lineValues[9];
                product.APP_APPLE_ID = lineValues[10];
                product.LOCALE = lineValues[11];
                products.Add(product);
                //UnityEngine.Debug.Log(JsonUtility.ToJson(product));
            });
            // Filter out products of other games.
            products = products.Filter(product => product.APP_APPLE_ID == SettingsInEditor<FGTSettings>.instance.appleAppId);
            return products;
        }

        string outputFolder => $"{Application.dataPath.Replace("/Assets", "")}/Temp/{config.appleSKU}.itmsp";
        private void PrepareUpdateFolderUpload(List<AppleIAPProductFromStore> productsFromStore)
        {
            Directory.CreateDirectory(outputFolder);

            var doc = new XmlDocument();
            var xmldecl = doc.CreateXmlDeclaration("1.0", "UTF-8", "");
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmldecl, root);

            XmlElement package = (XmlElement)doc.AppendChild(doc.CreateElement("package"));
            package.SetAttribute("xmlns", "http://apple.com/itunes/importer");
            package.SetAttribute("version", "software5.11");
            package.AppendChild(doc.CreateElement("provider")).InnerText = config.appleTeamID;
            package.AppendChild(doc.CreateElement("team_id")).InnerText = config.appleTeamID;
            var software = package.AppendChild(doc.CreateElement("software"));
            software.AppendChild(doc.CreateElement("vendor_id")).InnerText = config.appleSKU;
            var softwareMetadata = software.AppendChild(doc.CreateElement("software_metadata"));
            var inAppPurchases = softwareMetadata.AppendChild(doc.CreateElement("in_app_purchases"));
            // Remove obsolete purchases.
            var productIdsToRemove = productsFromStore.Filter(p
                => !config.allProducts.Any(p1 => p1.productId == p.PRODUCT_ID)).ConvertAll(p => p.PRODUCT_ID);
            productIdsToRemove.ForEach(productId => {
                var inAppPurchase = CreateInAppPurchaseWithId(productId, inAppPurchases);
                inAppPurchase.SetAttribute("remove", "true");
            });
            XmlElement CreateInAppPurchaseWithId(string productId, XmlNode parentNode)
            {
                var inAppPurchase = (XmlElement)parentNode.AppendChild(doc.CreateElement("in_app_purchase"));
                inAppPurchase.AppendChild(doc.CreateElement("product_id")).InnerText = productId;
                return inAppPurchase;
            }
            // Add/Update consumables and non-consumables.
            config.consumables.ForEach(product => CreateNonSubscriptionInAppPurchase(product));
            config.nonConsumables.ForEach(product => CreateNonSubscriptionInAppPurchase(product));
            XmlElement locale;
            XmlElement CreateInAppPurchase(AbstractProductSettings product, XmlNode parentNode)
            {
                // Basic data.
                var inAppPurchase = CreateInAppPurchaseWithId(product.productId, parentNode);
                inAppPurchase.AppendChild(doc.CreateElement("reference_name")).InnerText = product.GetProductIdSuffix();
                inAppPurchase.AppendChild(doc.CreateElement("type")).InnerText = GetTypeStringForXML(product);
                // Desription.
                locale = (XmlElement)inAppPurchase.AppendChild(doc.CreateElement("locales")).AppendChild(doc.CreateElement("locale"));
                locale.SetAttribute("name", "en-US");
                locale.AppendChild(doc.CreateElement("title")).InnerText = product.title;
                locale.AppendChild(doc.CreateElement("description")).InnerText = product.description;
                // Screenshot.
                var reviewScreenshot = inAppPurchase.AppendChild(doc.CreateElement("review_screenshot"));
                var screenShotName = Path.GetFileName(product.appleReviewScreenshotPath);
                var screenShotMoveFrom = product.appleReviewScreenshotPath;
                var screenShotMoveTo = $"{outputFolder}/{screenShotName}";
                File.Copy(screenShotMoveFrom, screenShotMoveTo, true);
                reviewScreenshot.AppendChild(doc.CreateElement("file_name")).InnerText = screenShotName;
                var fileBytes = File.ReadAllBytes(screenShotMoveTo);
                reviewScreenshot.AppendChild(doc.CreateElement("size")).InnerText = fileBytes.Length.ToString();
                var checksum = (XmlElement)reviewScreenshot.AppendChild(doc.CreateElement("checksum"));
                checksum.SetAttribute("type", "md5");
                checksum.InnerText = MD5Utils.GetMD5(fileBytes);
                return inAppPurchase;
            }
            XmlElement CreateNonSubscriptionInAppPurchase(AbstractProductSettings product)
            {
                var inAppPurchase = CreateInAppPurchase(product, inAppPurchases);
                var productNode = inAppPurchase.AppendChild(doc.CreateElement("products")).AppendChild(doc.CreateElement("product"));
                productNode.AppendChild(doc.CreateElement("cleared_for_sale")).InnerText = product.active ? "true" : "false";
                productNode.AppendChild(doc.CreateElement("wholesale_price_tier")).InnerText = 
                    StorePriceTierUtils.ToApplePriceTier((decimal)product.price, product.GetProductTypeEditMode()).ToString();
                return inAppPurchase;
            }
            // Create subscription group.
            if (config.subscription.products.Count > 0)
            {
                var subscriptionGroup = (XmlElement)inAppPurchases.AppendChild(doc.CreateElement("subscription_group"));
                subscriptionGroup.SetAttribute("name", "DefaultSubscriptionGroup"); // Cant be changed here, only from site.
                locale = (XmlElement)subscriptionGroup.AppendChild(doc.CreateElement("locales")).AppendChild(doc.CreateElement("locale"));
                locale.SetAttribute("name", "en-US");
                locale.AppendChild(doc.CreateElement("title")).InnerText = config.subscription.title;
                locale.AppendChild(doc.CreateElement("app_name")).InnerText = Application.productName;
                config.subscription.products.ForEach(product => CreateSubscriptionInAppPurchase(product));
                XmlElement CreateSubscriptionInAppPurchase(SubscriptionProductSettings product)
                {
                    var inAppPurchase = CreateInAppPurchase(product, subscriptionGroup);
                    inAppPurchase.AppendChild(doc.CreateElement("duration")).InnerText = product.duration.ToAppleAppeAppStore().ToExportString();
                    inAppPurchase.AppendChild(doc.CreateElement("cleared_for_sale")).InnerText = (product.active && config.subscription.exists) ? "true" : "false";
                    inAppPurchase.AppendChild(doc.CreateElement("rank")).InnerText = "1";
                    var prices = inAppPurchase.AppendChild(doc.CreateElement("prices"));
                    var offers = inAppPurchase.AppendChild(doc.CreateElement("offers"));
                    Utils.ForEach<CountryCodesISO3166Alpha2>(country =>
                    {
                        var price = prices.AppendChild(doc.CreateElement("price"));
                        price.AppendChild(doc.CreateElement("territory")).InnerText = country.ToString();
                        price.AppendChild(doc.CreateElement("tier")).InnerText =
                            StorePriceTierUtils.ToApplePriceTier((decimal)product.price, product.GetProductTypeEditMode()).ToString();
                        if (config.subscription.freeTrialExists)
                        {
                            var offer = offers.AppendChild(doc.CreateElement("offer"));
                            offer.AppendChild(doc.CreateElement("territory")).InnerText = country.ToString();
                            offer.AppendChild(doc.CreateElement("type")).InnerText = "free-trial";
                            offer.AppendChild(doc.CreateElement("duration")).InnerText = config.subscription.GetFreeTrialDuration().ToAppleAppeAppStore().ToExportString();
                            offer.AppendChild(doc.CreateElement("start_date")).InnerText = DateTime.UtcNow.ToString("yyyy-MM-dd");
                        }
                    });
                    return inAppPurchase;
                }
            }
            // Output xml.
            var xml = doc.ToFormattedXML();
            Debug.Log(xml);
            var xmlFilePath = $"{outputFolder}/metadata.xml";
            File.WriteAllText(xmlFilePath, xml);
        }

        string GetTypeStringForXML(AbstractProductSettings product)
        {
            var type = product.GetProductTypeEditMode();
            switch (type)
            {
                default:
                case ProductType.Consumable: return "consumable";
                case ProductType.NonConsumable: return "non-consumable";
                case ProductType.Subscription: return "auto-renewable";
            }
        }

        async Task<bool> UploadFiles() {
            //transporter.logging = true;
            var (success, output, error) = await Execute("upload", $"-f {outputFolder}");
            //transporter.logging = false;
            if (!success)
            {
                Debug.LogError($"upload failed:\n{error}");
                return false;
            }
            success = output.Contains("packages were uploaded successfully:");
            if (!success)
            {
                Debug.LogError($"upload failed, output does not contain success string:\n{output}");
                return false;
            }
            // Success.
            return true;
        }

        public async Task<bool> Upload()
        {
            var productsFromStore = await GetReports();// UnzipParseReport(@"D:\Programming\FriendsGames\FriendsGamesToolsExample\FriendsGamesToolsExample\Temp\AppleReports\77HBQK6TTN_inapps.tzhkjlef.zip");
            PrepareUpdateFolderUpload(productsFromStore);
            return await UploadFiles();
        }
    }

    public static class XMLUtils
    {
        public static string ToFormattedXML(this XmlDocument document)
        {
            MemoryStream mStream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.UTF8);

            writer.Formatting = Formatting.Indented;

            // Write the XML into a formatting XmlTextWriter
            document.WriteContentTo(writer);
            writer.Flush();
            mStream.Flush();

            // Have to rewind the MemoryStream in order to read
            // its contents.
            mStream.Position = 0;

            // Read MemoryStream contents into a StreamReader.
            StreamReader sReader = new StreamReader(mStream);

            // Extract the text from the StreamReader.
            string formattedXml = sReader.ReadToEnd();

            mStream.Close();
            writer.Close();

            return formattedXml;
        }
    }
    public static class MD5Utils
    {
        public static string GetMD5(byte[] bytes)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var md5Bytes = md5.ComputeHash(bytes);
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < md5Bytes.Length; i++)
                    sBuilder.Append(md5Bytes[i].ToString("x2"));
                return sBuilder.ToString();
            }
        }
    }

    //[Serializable]
    public class AppleIAPProductFromStore
    {
        public string APPLE_ID; // 1495029090 1495060906 1495060903
        public string REFERENCE_NAME; // com.friendsgamesincubator.fgttest.subscription1 com.friendsgamesincubator.fgttest.consumable2 com.friendsgamesincubator.fgttest.consumable1
        public string PRODUCT_ID; // com.friendsgamesincubator.fgttest.subscription1 com.friendsgamesincubator.fgttest.consumable2 com.friendsgamesincubator.fgttest.consumable1
        public string ADDON_TYPE; // Auto-Renewable Subscription Consumable
        public static class Type
        {
            public const string Consumable = "Consumable";
            public const string NonConsumable = "Non-Consumable";
            public const string AutoRenewableSubscription = "Auto-Renewable Subscription";
            public const string NonRenewingSubscription = "Non-Renewing Subscription";
        }
        public string ADDON_ITC_STATUS; // Ready to Submit
        public string AUTO_RENEWABLE_DURATION; // 1 Month
        public string FREE_TRIAL_DURATION; // 1 Week
        public string MARKETING_OPTIN_INCENTIVE_DURATION;
        public string CLEARED_FOR_SALE; // Y Y Y
        public string APP_NAME;
        public string APP_APPLE_ID; // 1495023292 1495023292 1495023292
        public string LOCALE;
    }

    public enum CountryCodesISO3166Alpha2
    {
        US, JP, GY, UZ, CL, BT, LV, BY, NL, TR, PG, MO, TZ, CZ, VE, KH,
        PH, GW, MN, FR, KE, LC, CY, KW, KG, PW, SL, AE, ZA, MR, IS, CN,
        NP, SE, AG, VN, TM, MW, EG, IN, MT, LK, TC, FJ, RO, GB, NG, UY,
        PY, YE, AM, BW, PK, GR, IE, BO, CO, NI, MU, TN, KY, TD, CH, LU,
        AI, BF, CV, HR, VC, GH, DZ, HK, NE, AT, BG, NZ, RU, BR, DM, OM,
        HU, GD, SN, TT, BN, GM, LA, QA, AU, HN, NO, PL, NA, DK, MD, PA,
        BH, DE, MS, ES, SG, ML, FM, SB, TH, KR, BS, VG, SC, LB, ID, BB,
        GT, CA, BE, AZ, MG, MZ, KN, UA, AL, MY, AR, PE, PT, EC, SZ, CG,
        SV, SA, FI, JM, BZ, EE, IT, DO, ST, AO, TJ, ZW, TW, SR, SK, UG,
        MX, IL, JO, SI, BM, MK, LR, BJ, KZ, CR, LT,
        RW, MV, NR, VU, XK, GE, CM, ME, BA, GA, ZM, CD, MA, AF, MM, IQ, RS, TO, CI, LY
    }
}
#endif