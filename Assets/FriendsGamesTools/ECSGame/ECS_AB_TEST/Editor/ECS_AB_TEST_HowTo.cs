using UnityEngine;

namespace FriendsGamesTools.ABTests
{
    public class ECS_AB_TEST_HowTo : HowToModule
    {
        public override string forWhat => "AB tests";
        protected override void OnHowToGUI()
        {
            ABTestsController.ShowOnGUI("Derive from this script",
                $"Setup AB tests from here and press <b>{ABTestModule.RunCodegeneration}</b>\n" +
                "For each test for each option controller will have bool <b>TestName_TestOption</b> attributes\n");
        }
        protected override string docsURL => "https://docs.google.com/document/d/16p0sqSz0EST23Unzh7oWjPQ0vkIvMYP4bKja450bCIU/edit?usp=sharing";
        ExampleScript ABTestsController = new ExampleScript("ABTestsController");
    }
}