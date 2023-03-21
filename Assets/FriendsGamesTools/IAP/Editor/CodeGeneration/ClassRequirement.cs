using System.Collections.Generic;

namespace FriendsGamesTools.CodeGeneration
{
    public class ClassRequirement : GenerationRequirement
    {
        string name;
        public ClassRequirement(string name)
        {
            this.name = name;
            visibility = new Visibility<ClassRequirement>(this);
            virtualization = new Virtualization<ClassRequirement>(this);
            partialization = new Partialization<ClassRequirement>(this);
        }
        public readonly Visibility<ClassRequirement> visibility;
        public readonly Virtualization<ClassRequirement> virtualization;
        public readonly Partialization<ClassRequirement> partialization;

        public MethodRequirement RequireMethod(string returnType, string name, params (string paramType, string paramName)[] parameters)
            => AddAndGet(new MethodRequirement(returnType, name, parameters));

        public PropertyRequirement RequireProperty(string returnType, string name)
            => AddAndGet(new PropertyRequirement(returnType, name));

        List<string> inheritedFrom = new List<string>();
        public ClassRequirement RequireInheritance(string inheritedFrom)
        {
            if (!this.inheritedFrom.Contains(inheritedFrom))
                this.inheritedFrom.Add(inheritedFrom);
            return this;
        }

        string definitionLine => $"{indent1}{visibility}{virtualization}{partialization}class {name}" +
            $"{(inheritedFrom.Count > 0 ? ": " : "")}{inheritedFrom.PrintCollection(", ", "")}";
        protected override bool completeSelf => parentFile.existingText.Contains(definitionLine);
        public override void Generate()
        {
            sb.AppendLine(definitionLine);
            sb.AppendLine($"{indent1}{{");
            base.Generate();
            sb.AppendLine($"{indent1}}}");
        }
    }
}