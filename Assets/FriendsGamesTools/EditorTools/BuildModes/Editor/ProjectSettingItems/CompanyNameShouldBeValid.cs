using System.Text;
using UnityEditor;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class CompanyNameShouldBeValid : ProjectSettingItem
    {
        public override string name => "COMPANY_NAME";
        public override string description => $"Company name should be {FriendsGamesConstants.CompanyName}";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (PlayerSettings.companyName != FriendsGamesConstants.CompanyName)
                sb.AppendLine(description);
        }
        public override bool canSetup => true;
        protected override void Setup()
        {
            if (PlayerSettings.companyName != FriendsGamesConstants.CompanyName)
                PlayerSettings.companyName = FriendsGamesConstants.CompanyName;
        }
    }

}
