using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FriendsGamesTools.CodeGeneration
{
    public class FileRequirement : GenerationRequirement
    {
        string name;
        public FileRequirement(string name) => this.name = name;
        public NameSpaceRequirement RequireNameSpace(string name) => AddAndGet(new NameSpaceRequirement(name));

        #region Wrap all with define
        List<string> defineWrappers = new List<string>();
        public FileRequirement RequireDefineWrapper(string define)
        {
            if (!defineWrappers.Contains(define))
                defineWrappers.Add(define);
            return this;
        }
        void GenerateDefinesWrapperStart()
        {
            if (defineWrappers.Count == 0)
                return;
            fileStringBuilder.AppendLine($"#if {defineWrappers.PrintCollection(" && ")}");
        }
        void GenerateDefinesWrapperEnd()
        {
            if (defineWrappers.Count == 0)
                return;
            fileStringBuilder.AppendLine("#endif");
        }
        #endregion

        #region Usings
        private List<string> usings = new List<string>();
        public FileRequirement RequireUsing(string @namespace)
        {
            if (!usings.Contains(@namespace))
                usings.Add(@namespace);
            return this;
        }
        void GenerateUsings()
        {
            usings.ForEach(ns => fileStringBuilder.AppendLine($"using {ns};"));
            if (usings.Count > 0)
                fileStringBuilder.AppendLine("");
        }
        #endregion

        #region Entries count
        private List<(string code, int count)> codeToCheckCount = new List<(string code, int count)>();
        public FileRequirement RequireCodeEntriesCount(string code, int count)
        {
            codeToCheckCount.Add((code, count));
            return this;
        }
        bool CodeCountingComplete => codeToCheckCount.All(check => existingText.Count(check.code) == check.count);
        #endregion

        public string path
        {
            get
            {
                string parentFolder;
                if (parent is FolderRequirement f)
                    parentFolder = f.folder;
                else
                    parentFolder = FriendsGamesManager.GeneratedFolder;
                return $"{parentFolder}/{name}.cs";
            }
        }
        public string existingText
        {
            get
            {
                var path = this.path;
                if (!File.Exists(path))
                    return string.Empty;
                return File.ReadAllText(path);
            }
        }
        protected override bool completeSelf => File.Exists(path) && CodeCountingComplete;
        public StringBuilder fileStringBuilder { get; private set; } = new StringBuilder();
        public override void Generate()
        {
            fileStringBuilder.Clear();
            GenerateDefinesWrapperStart();
            GenerateUsings();
            base.Generate();
            GenerateDefinesWrapperEnd();
            File.WriteAllText(path, fileStringBuilder.ToString());
        }
    }
}