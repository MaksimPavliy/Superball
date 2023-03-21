using UnityEditor;

namespace FriendsGamesTools.CodeGeneration
{
    public class CodeGenerator : GenerationRequirement
    {
        public FolderRequirement RequireFolder(string name) => AddAndGet(new FolderRequirement(name));
        protected override bool completeSelf => true;
        public override void Generate()
        {
            base.Generate();
            AssetDatabase.Refresh();
        }
    }
}