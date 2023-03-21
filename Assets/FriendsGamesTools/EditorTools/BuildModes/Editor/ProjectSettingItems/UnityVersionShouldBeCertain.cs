using System.Text;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class UnityVersionShouldBeCertain : ProjectSettingItem
    {
        public override string name => "UNITY_VERSION";
        public override string description
            => $"{FriendsGamesConstants.CompanyName} requires unity version {FriendsGamesConstants.UnityVersion} or {FriendsGamesConstants.UnityVersionOld}";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (Application.unityVersion != FriendsGamesConstants.UnityVersion && Application.unityVersion != FriendsGamesConstants.UnityVersionOld)
                sb.AppendLine(description);
        }
    }
}
