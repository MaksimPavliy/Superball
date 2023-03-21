using System;

namespace FriendsGamesTools.CodeGeneration
{
    public class NameSpaceRequirement : GenerationRequirement
    {
        string name;
        public NameSpaceRequirement(string name) => this.name = name;
        public ClassRequirement RequireClass(string name) => AddAndGet(new ClassRequirement(name));
        public EnumRequirement RequireEnum(string name) => AddAndGet(new EnumRequirement(name));

        protected override bool completeSelf => true;
        public override void Generate()
        {
            sb.AppendLine($"namespace {name}");
            sb.AppendLine("{");
            base.Generate();
            sb.AppendLine("}");
        }
    }
}