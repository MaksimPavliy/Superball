using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    public class HCTemplateModule_HowTo : HowToModule
    {
        public override string forWhat => "template for standard hyper-casual game";
        protected override void OnHowToGUI()
        {
            EditorGUIUtils.RichMultilineLabel("Allows developer to code only core game");
            EditorGUILayout.Space(20);

            EditorGUIUtils.LabelAtCenter("HOW To use HCTemplate to create a new game:");
            EditorGUIUtils.RichMultilineLabel("  <b>copy</b> unity project folder");
            EditorGUIUtils.LabelToCopy("  git lfs fetch --all origin");
            EditorGUIUtils.LabelToCopy("  git remote set-url origin git@bitbucket.org:friendsgamesincubator/%newgame%.git");
            EditorGUIUtils.LabelToCopy("  git push -u origin --all");
            EditorGUIUtils.LabelToCopy("  git lfs push --all origin");
            EditorGUIUtils.RichMultilineLabel("  setup new game <b>bundle id</b> for ios, anroid, setup new <b>game name</b> instead of <b>HC</b>");
            EditorGUIUtils.RichMultilineLabel("  <b>unlink</b> game services");
            EditorGUIUtils.RichMultilineLabel("  create new <b>game services</b> project");
            EditorGUIUtils.RichMultilineLabel("  <b>commit</b>");
            EditorGUIUtils.LabelToCopy("  <b>git push</b>");
            EditorGUIUtils.RichMultilineLabel("  setup apple bundle id, profiles on <b>developers.apple.com</b>");
            EditorGUIUtils.RichMultilineLabel("  setup android and appstore builds on <b>cloud</b>");

            EditorGUILayout.Space(20);
            EditorGUIUtils.RichMultilineLabel($"Then refer to <b>{LevelBasedModule.define}</b>, <b>{SkinsModule.define}</b>");
        }
        protected override string docsURL => "";
    }
}