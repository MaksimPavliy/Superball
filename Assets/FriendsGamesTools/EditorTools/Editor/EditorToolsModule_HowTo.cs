using UnityEngine;

namespace FriendsGamesTools.EditorTools
{
    public class EditorToolsModule_HowTo : HowToModule
    {
        public override string forWhat => "tools for modifying Unity project";
        protected override void OnHowToGUI()
        {
            //GUILayout.Label("To get art resources from artists automatically updated, use UPDATE_ART module");
            GUILayout.Space(10);
            AssetsIterator.ShowOnGUI("Use this script to iterate resources in Unity project",
                "There are a number of useful methods like\n" +
                " - <b>IterateAssetsInFolderRecursively()</b>\n" +
                " - <b>IterateAllGameObjectsInProject()</b>\n" +
                " - <b>IterateOpenedScene()</b>\n" +
                " - <b>IterateGameObjectRecursively()</b>\n" +
                " - <b>IterateAssetsInFolder()</b>");
            EditorCoroutine.ShowOnGUI("write <i>await <b>EditorAsync</b>.WaitForSeconds(seconds);</i> to do async operations in editor");

            CollectionElementTitleAttribute.ShowOnGUI("show elements in collection in inspector in custom format",
                "Example:\n" +
                "<b>[CollectionElementTitle(\"{type} {width}x{length} after {delay}s\")]</b>\n" +
                "public List<EnemySpawn> spawns;");
        }
        protected override string docsURL => "";
        ExampleScript AssetsIterator = new ExampleScript("AssetsIterator");
        ExampleScript EditorCoroutine = new ExampleScript("EditorCoroutine");
        ExampleScript CollectionElementTitleAttribute = new ExampleScript("CollectionElementTitleAttribute");
    }
}