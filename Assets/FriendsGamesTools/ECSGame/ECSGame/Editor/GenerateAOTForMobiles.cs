#if ECS_GAMEROOT
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace FriendsGamesTools.ECSGame.DataMigration
{
    public class GenerateAOTForMobiles : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report) => AOTForMobilesCodegen.Generate();
    }
}
#endif