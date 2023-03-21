using System.IO;

namespace FriendsGamesTools.CodeGeneration
{
    public class FolderRequirement : GenerationRequirement
    {
        string name;
        public FolderRequirement(string name) => this.name = name;
        public FileRequirement RequireFile(string name) => AddAndGet(new FileRequirement(name));

        public string folder
        {
            get
            {
                string parentFolder;
                if (parent is FolderRequirement f)
                    parentFolder = f.folder;
                else
                    parentFolder = FriendsGamesManager.GeneratedFolder;
                return $"{parentFolder}/{name}";
            }
        }
        protected override bool completeSelf => Directory.Exists(folder);
        public override void Generate()
        {
            Directory.CreateDirectory(folder);
            base.Generate();
        }
    }
}