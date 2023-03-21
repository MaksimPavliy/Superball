namespace FriendsGamesTools.CodeGeneration
{
    public class PropertyRequirement : GenerationRequirement
    {
        string returnType, name;
        public PropertyRequirement(string returnType, string name)
        {
            this.returnType = returnType;
            this.name = name;
            visibility = new Visibility<PropertyRequirement>(this);
            virtualization = new Virtualization<PropertyRequirement>(this);
            staticity = new Staticity<PropertyRequirement>(this);
        }

        public readonly Visibility<PropertyRequirement> visibility;
        public readonly Virtualization<PropertyRequirement> virtualization;
        public readonly Staticity<PropertyRequirement> staticity;

        string expressionBody;
        public PropertyRequirement RequireExpressionBody(string expressionBody)
        {
            this.expressionBody = expressionBody;
            return this;
        }
        bool isAbstract => virtualization.virtualization == VirtualizationType.Abstract;
        string definitionLine => $"{indent2}{visibility}{staticity}{virtualization}{returnType} {name}{(isAbstract ? ";" : "")}";
        protected override bool completeSelf => parentFile.existingText.Contains(definitionLine) 
            && (string.IsNullOrEmpty(expressionBody) || parentFile.existingText.Contains(expressionBody));
        public override void Generate()
        {
            sb.AppendLine(definitionLine);
            if (!isAbstract)
            {
                // Create property body.
                if (!string.IsNullOrEmpty(expressionBody))
                    sb.AppendLine($"{indent3}=> {expressionBody};");
                else
                {
                    sb.AppendLine($"{indent2}{{");
                    sb.AppendLine($"{indent2}}}");
                }
            }
            base.Generate();
        }
    }
    
}