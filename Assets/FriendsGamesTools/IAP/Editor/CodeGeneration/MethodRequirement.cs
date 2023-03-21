using System;
using System.Text;

namespace FriendsGamesTools.CodeGeneration
{
    public class MethodRequirement : GenerationRequirement
    {
        string returnType, name;
        (string paramType, string paramName)[] parameters;
        public MethodRequirement(string returnType, string name, params (string paramType, string paramName)[] parameters)
        {
            this.returnType = returnType;
            this.name = name;
            this.parameters = parameters;
            visibility = new Visibility<MethodRequirement>(this);
            virtualization = new Virtualization<MethodRequirement>(this);
        }

        public readonly Visibility<MethodRequirement> visibility;
        public readonly Virtualization<MethodRequirement> virtualization;

        string body;
        bool bodyIsExpression;
        public MethodRequirement RequireExpressionBody(string expressionBody)
        {
            body = expressionBody;
            bodyIsExpression = true;
            return this;
        }
        public MethodRequirement RequireBody(string body)
        {
            this.body = body;
            bodyIsExpression = false;
            return this;
        }

        bool isAbstract => virtualization.virtualization == VirtualizationType.Abstract;
        string definitionLine => $"{indent2}{visibility}{virtualization}{returnType} {name}({parameters.ConvertAll(p => $"{p.paramType} {p.paramName}").PrintCollection(", ", "")}){(isAbstract?";":"")}";
        protected override bool completeSelf => parentFile.existingText.Contains(definitionLine);
        public override void Generate()
        {
            sb.AppendLine(definitionLine);
            if (!isAbstract)
            {
                // Create method body.
                if (string.IsNullOrEmpty(body))
                    bodyIsExpression = false;
                if (bodyIsExpression)
                    sb.AppendLine($"{indent3}=> {body};");
                else
                {
                    sb.AppendLine($"{indent2}{{");
                    var lines = body.ToLf().Split(new string[] { "\n" }, StringSplitOptions.None);
                    foreach (var line in lines)
                    {
                        sb.Append(indent3);
                        sb.AppendLine(line);
                    }
                    sb.AppendLine($"{indent2}}}");
                }
            }
            base.Generate();
        }
    }
    
}