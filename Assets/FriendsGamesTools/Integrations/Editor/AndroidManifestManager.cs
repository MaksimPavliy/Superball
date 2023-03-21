using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public class AndroidManifestManager : XMLFileManager
    {
        public enum Permission
        {
            Unknown,
            ACCESS_NETWORK_STATE,
            ACCESS_WIFI_STATE,
            INTERNET,
            READ_PHONE_STATE
        }
        const string defaultPath = "Assets/Plugins/Android/AndroidManifest.xml";
        const string defaultContentsPath = FriendsGamesManager.MainPluginFolder + "/Integrations/Editor/DefaultManifest.txt";
        protected override string lineEndings => "\r\n";
        protected override string defaultContents
        {
            get
            {
                var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(defaultContentsPath);
                var text = textAsset.text;
                text = text.Replace("PACKAGE_ID", Application.identifier);
                return text;
            }
        }
        public AndroidManifestManager(string path = null) : base(string.IsNullOrEmpty(path)? defaultPath:path) { }        
        
        private string GetPermissionString(Permission permission) => $"android.permission.{permission}";
        public bool ContainsPermission(Permission permission) => contents.Contains(GetPermissionString(permission));
        public AndroidManifestManager EnsurePermisson(Permission permission)
        {
            var lines = contents.Split('\n').ToList();
            string permissionString = GetPermissionString(permission);
            int existingPermissionInd = lines.FindIndex(line=>line.Contains(permissionString));
            if (existingPermissionInd==-1)
            {
                string permissionComment = "<!-- Permissions -->";
                var insertInd = lines.FindLastIndex(line=>line.Contains(permissionComment));
                string addedPermissionString = $"  <uses-permission android:name=\"android.permission.{permission}\" />";
                if (insertInd == -1)
                {
                    int manifestInd = lines.FindIndex(line => line.Contains("<manifest"));
                    int manifestEndInd = manifestInd;
                    while (!lines[manifestEndInd].Contains(">"))
                        manifestEndInd++;
                    insertInd = manifestEndInd + 1;
                }
                lines.Insert(insertInd + 1, addedPermissionString);
            }
            contents = string.Join("\n", lines);
            return this;
        }
        public AndroidManifestManager AddInApplicationTag(string text)
        {
            var applicationTagInd = contents.IndexOf("<application");
            applicationTagInd = contents.IndexOf(">", applicationTagInd);
            applicationTagInd = contents.IndexOf("\n", applicationTagInd) + 1;
            contents = contents.Insert(applicationTagInd, text);
            return this;
        }
        public AndroidManifestManager SetPackageName(string packageName)
        {
            const string packageStart = "package=\"";
            contents = contents.ReplaceLineWith(packageStart, oldLine => {
                var startInd = oldLine.IndexOf(packageStart) + packageStart.Length;
                var endInd = oldLine.IndexOf("\"", startInd) - 1;
                var oldPackageName = oldLine.Substring(startInd, endInd - startInd + 1);
                var newLine = oldLine.Replace(oldPackageName, packageName);
                return newLine;
            });
            return this;
        }
        public string GetPackageName()
        {
            const string prefix = "package=\"";
            int prefixInd = contents.IndexOf(prefix);
            if (prefixInd == -1)
                return null;
            int startInd = prefixInd + prefix.Length;
            int endInd = contents.IndexOf("\"", startInd) - 1;
            var id = contents.Substring(startInd, endInd - startInd + 1);
            return id;
        }
    }
}