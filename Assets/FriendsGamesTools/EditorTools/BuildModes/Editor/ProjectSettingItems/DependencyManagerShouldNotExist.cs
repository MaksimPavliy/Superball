using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class DependencyManagerShouldNotExist : ProjectSettingItem
    {
        public override string name => "DEPENDENCY_MANAGER_KILLING";
        public override string description => $"{folder} should not be in project";
        const string folder = "Assets/ExternalDependencyManager";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (Directory.Exists(folder))
                sb?.AppendLine(description + "\n It should be in packages instead");
        }
        public override bool canSetup => true;
        protected override void Setup()
        {
            Directory.Delete(folder, true);
        }
    }
}
