using MiniJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FriendsGamesTools
{
    public static class PackagesManager 
    {
        public struct PackageDesc {
            public string package;
            public string version;
        }
        const string path = "Packages/manifest.json";
        public static List<PackageDesc> Update()
        {
            _packages = new List<PackageDesc>();
            var json = File.ReadAllText(path);
            var manifestDict = Json.Deserialize(json) as Dictionary<string, object>;
            var dependancies = manifestDict["dependencies"] as Dictionary<string, object>;
            foreach (var (package, version) in dependancies)
                _packages.Add(new PackageDesc { package = package, version = version.ToString() });

            if (manifestDict.ContainsKey(scopedRegistriesKey))
            {
                var scopedRegistries = manifestDict[scopedRegistriesKey] as List<object>;
                googleRegisterExists = scopedRegistries?.Any(r => (r as Dictionary<string, object>)["url"] as string == GoogleURL) ?? false;
            }
            else
                googleRegisterExists = false;
            return _packages;
        } 
        static List<PackageDesc> _packages;
        public static IReadOnlyList<PackageDesc> packages => _packages ?? Update();
        public static (string package, string version) SplitPackageId(string packageId) {
            var parts = packageId.Split('@');
            return (parts[0], parts[1]);
        }
        public static bool IsInProject(string package)
        {
            if (!package.Contains("@"))
                return packages.Any(p => p.package == package);
            else {
                string version;
                (package, version) = SplitPackageId(package);
                return IsInProject(package, version);
            }
        }
        public static bool IsInProject(string package, string minVersion, bool exactVersion = false)
            => packages.Any(p => p.package == package && ((exactVersion && minVersion == p.version) || StringUtils.VersionGreaterEquals(minVersion, p.version)));

        public static bool googleRegisterExists { get; private set; }
        const string GoogleURL = "https://unityregistry-pa.googleapis.com";
        const string scopedRegistriesKey = "scopedRegistries";
        public static void AddGooglePackagesRegister()
        {
            var json = File.ReadAllText(path);
            var manifestDict = Json.Deserialize(json) as Dictionary<string, object>;
            List<object> scopedRegistries;
            if (manifestDict.TryGetValue(scopedRegistriesKey, out var obj))
                scopedRegistries = obj as List<object>;
            else
            {
                scopedRegistries = new List<object>();
                manifestDict.Add(scopedRegistriesKey, scopedRegistries);
            }
            var google = new Dictionary<string, object>();
            google.Add("name", "Game Package Registry by Google");
            google.Add("url", GoogleURL);
            google.Add("scopes", new List<object> { "com.google" });
            scopedRegistries.Add(google);
            json = Json.Serialize(manifestDict);
            File.WriteAllText(path, json);
        }
    }
}