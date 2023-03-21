using System.Text;
using UnityEngine;

namespace FriendsGamesTools.EditorTools.BuildModes
{
    public class ProductNameShouldBeSet : ProjectSettingItem
    {
        public override string name => "PRODUCT_NAME";
        public override string description => "Application.productName should be set";
        public override void GetReleaseCheckError(StringBuilder sb)
        {
            if (Application.productName.ToLower().Contains("default") || Application.productName.ToLower() == "New Unity Project".ToLower())
                sb.AppendLine(description);
        }
    }
}
