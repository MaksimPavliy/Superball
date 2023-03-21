using System;
using System.Collections.Generic;

namespace FriendsGamesTools.CodeGeneration
{
    public class EnumRequirement : GenerationRequirement
    {
        string name;
        public EnumRequirement(string name) => this.name = name;

        List<string> values = new List<string>();
        public EnumRequirement RequireValue(string value)
        {
            values.Add(value);
            return this;
        }
        protected override bool completeSelf
        {
            get
            {
                var enumType = ReflectionUtils.GetTypeByName(name);
                if (enumType == null)
                    return false;
                var enumValues = Enum.GetValues(enumType).ConvertAll(v=>v.ToString());
                if (values.Count != enumValues.Count)
                    return false;
                foreach (var valueString in values)
                {
                    if (!enumValues.Contains(valueString))
                        return false;
                }
                return true;
            }
        }
        public override void Generate()
        {
            sb.AppendLine($"{indent1}public enum {name}");
            sb.AppendLine($"{indent1}{{");
            values.ForEach(v => sb.AppendLine($"{indent2}{v},"));
            sb.AppendLine($"{indent1}}}");
            base.Generate();
        }
    }
    public enum Tro
    {
        asd,
        asdd,
        qfv,
    }
}