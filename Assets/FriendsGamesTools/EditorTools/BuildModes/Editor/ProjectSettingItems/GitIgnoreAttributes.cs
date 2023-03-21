using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public abstract class GitSettingsFileManager : ProjectSettingItem
    {
        public override string description => $"Ensure {name} is correct";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (!GetCompleted())
                sb.AppendLine($"{name} is not set correctly");
        }
        protected string path => name;
        string defaultContentsPath => FriendsGamesManager.MainPluginFolder + $"/EditorTools/BuildModes/Editor/Git/default{name}.txt";
        string defaultContents => PostProcessSettings(AssetDatabase.LoadAssetAtPath<TextAsset>(defaultContentsPath).text);
        string GetText()
        {
            if (!File.Exists(path))
                return string.Empty;
            else
                return File.ReadAllText(path).ToLf();
        }
        bool GetCompleted()
        {
            var text = GetText();
            if (string.IsNullOrEmpty(text))
                return false;
            if (text.Contains(forbiddenString))
                return false;
            return text.StartsWith(defaultContents);
        }
        public override bool canSetup => true;
        string PostProcessSettings(string settingsText) => string.Join(StringUtils.OSLineEnding, settingsText.ToLf().Split('\n').Distinct());
        protected override void Setup()
        {
            var text = defaultContents + "\n" + GetText();
            text = text.Replace(forbiddenString, "");
            text = PostProcessSettings(text);
            File.WriteAllText(path, text);
        }
        const string forbiddenString = "**/Fonts & Materials/*.asset filter=lfs diff=lfs merge=lfs -text\n";
    }
    public class GitIgnoreManager : GitSettingsFileManager
    {
        public override string name => ".gitignore";
    }
    public class GitAttributesManager : GitSettingsFileManager
    {
        public override string name => ".gitattributes";
    }
}
