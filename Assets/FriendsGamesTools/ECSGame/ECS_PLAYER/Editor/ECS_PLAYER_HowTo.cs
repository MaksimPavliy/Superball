using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_PLAYER_HowTo : HowToModule
    {
        public override string forWhat => "using player money and player level modules";
        protected override void OnHowToGUI()
        {
            EditorGUIUtils.RichMultilineLabel($"Just enable this module, no other actions required\n");
            GUILayout.Space(10);
        }
        protected override string docsURL => "https://drive.google.com/open?id=1j4YcRbjxeDhIS0DgHJUzuEUhIP2oBFdNG7vGkXDMZ8w";
    }
}


