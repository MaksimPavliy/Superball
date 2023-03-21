#if ECS_GAMEROOT
using FriendsGamesTools.CodeGeneration;
using System.Collections.Generic;
using System.Text;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public static class AOTForMobilesCodegen
    {
        public static void Generate()
        {
            var codeGen = new CodeGenerator();
            RequireAOT(codeGen);
            codeGen.Generate();
        }
        public static void RequireAOT(CodeGenerator codeGen)
        {
            var folder = RequireECSFolder(codeGen);
            RequireAOT(folder);
        }
        public const string namespaceName = "FriendsGamesTools.ECSGame.DataMigration";
        private static FolderRequirement RequireECSFolder(CodeGenerator codeGen)
            => codeGen.RequireFolder("AOTForECSOnMobiles");
        private static void RequireAOT(FolderRequirement folder)
        {
            // Generate AOT.
            var aotClassName = "ECSMethodsForAOT";
            var aotFile = folder.RequireFile(aotClassName).RequireDefineWrapper("!UNITY_EDITOR").RequireDefineWrapper(ECSModuleFolder.define);
            var @namespace = aotFile.RequireNameSpace(namespaceName);
            var aotClass = @namespace.RequireClass(aotClassName).visibility.RequirePublic();
            var callECSMethods = aotClass.RequireMethod("void", "CallECSMethods").visibility.RequirePublic();
            var callECSBody = new StringBuilder();
            var manager = "manager";
            var e = "e";
            callECSBody.AppendLine($"var {manager} = Unity.Entities.World.Active.EntityManager;");
            callECSBody.AppendLine($"var {e} = new Unity.Entities.Entity();");
            WorldMetaDataUtils.componentTypes.ForEach(comp => {
                var getCompCode = $"{manager}.GetComponentData<{comp.FullNameInCode()}>({e})";
                callECSBody.AppendLine($"{manager}.SetComponentData({e}, {getCompCode});");
                callECSBody.AppendLine($"{manager}.AddComponent<{comp.FullNameInCode()}>({e});");
                callECSBody.AppendLine($"{manager}.AddComponentData<{comp.FullNameInCode()}>({e}, {getCompCode});");
                callECSBody.AppendLine($"{manager}.HasComponent<{comp.FullNameInCode()}>({e});");
            });
            WorldMetaDataUtils.bufferItemTypes.ForEach(buffer => {
                callECSBody.AppendLine($"{manager}.GetBuffer<{buffer.FullNameInCode()}>({e});");
                callECSBody.AppendLine($"{manager}.AddBuffer<{buffer.FullNameInCode()}>({e});");
                callECSBody.AppendLine($"{manager}.HasComponent({e}, Unity.Entities.ComponentType.ReadWrite<{buffer.FullNameInCode()}>());");
            });
            callECSMethods.RequireBody(callECSBody.ToString());
        }
        static void MigrationMethod(string returnType, ClassRequirement @class, string name, bool entityParams, params (string paramType, string paramName)[] parameters)
        {
            var list = parameters.ToList();
            if (entityParams)
                AddEntityParams(list);
            AddWorldParams(list);
            @class.RequireMethod(returnType, name, list.ToArray()).virtualization.RequireAbstract().visibility.RequireProtected();
        }
        public const string UnversionedWorldData = "UnversionedWorldData";
        public const string UnversionedEntity = "UnversionedEntity";
        static void AddEntityParams(List<(string paramType, string paramName)> parameters)
        {
            parameters.Add((UnversionedEntity, "oldEntity"));
            parameters.Add((UnversionedEntity, "newEntity"));
        }
        static void AddWorldParams(List<(string paramType, string paramName)> parameters)
        {
            parameters.Add((UnversionedWorldData, "oldWorld"));
            parameters.Add((UnversionedWorldData, "newWorld"));
        }
    }
}
#endif