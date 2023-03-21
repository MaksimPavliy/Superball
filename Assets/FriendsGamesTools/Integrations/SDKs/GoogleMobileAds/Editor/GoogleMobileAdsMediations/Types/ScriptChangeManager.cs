using System.IO;
using UnityEditor;

namespace FriendsGamesTools
{
    public class ScriptChangeManager
    {
        string scriptPath;
        public string text = "";
        public ScriptChangeManager(string scriptPath) // "MaxSdk/Mediation/Google/SomeName.cs"
        {
            this.scriptPath = scriptPath;
            try
            {
                text = File.ReadAllText(scriptPath).ToLf();
            } catch { }
        }
        static string FindScriptPath(string scriptName, string searchFolder)
        {
            var scriptPathes = AssetDatabase.FindAssets($"t:MonoScript {scriptName}")
                .ConvertAll(guid => AssetDatabase.GUIDToAssetPath(guid));
            return scriptPathes.Find(path => path.Contains(searchFolder));
        }
        public ScriptChangeManager(string scriptName, string searchFolder) // "MaxSdk/Mediation/Google"
            : this(FindScriptPath(scriptName, searchFolder)) { }
        public void Save()
        {
            var dir = Path.GetDirectoryName(scriptPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var contentsToSave = text.ToCrLf();
            File.WriteAllText(scriptPath, contentsToSave, System.Text.Encoding.UTF8);
            AssetDatabase.Refresh();
        }
    }
}