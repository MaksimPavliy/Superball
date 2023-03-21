using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_LOCATIONS_HowTo : HowToModule
    {
        public override string forWhat => "changing locations in game";
        protected override void OnHowToGUI()
        {
            LocationsController.ShowOnGUI("Derive from this to implement locations",
                "<b>ChangeLocation()</b> - override if you need custom logic\n" + 
                "<b>GetMultiplier(locationInd)</b> - next location will multiply something\n" +
                "<b>canChangePrivate</b> - when player allowed to change location?\n" +
                "also override <b>maxLocations</b>\n" +
                "to loop locations you can override <b>loop</b>"
                );
            ILocationDependant.ShowOnGUI("use this interface on controllers that react to location changes",
                "<b>OnLocationChanged(newLocationInd)</b> - allow controllers to react to location changes");
            LocationsView.ShowOnGUI("Put this script to show locations differently and changing location UI",
                "useful UI inspector fields - <b>changeLocationButton</b>, <b>canChangeParent</b> <b>currMultiplier</b>, <b>nextMultiplier</b>\n" +
                "useful methods - <b>OnChangeLocationPressed()</b>, <b>ShowLocation(locationInd)</b> - override for custom logic"
                );
            GUILayout.Space(20);
        }
        protected override string docsURL => "https://docs.google.com/document/d/1bi3zNhxWXss4cbBPA_a96pPJW5rjlFUgoC3CcQ4ZaBo/edit?usp=sharing";

        ExampleScript LocationsController = new ExampleScript("LocationsController");
        ExampleScript ILocationDependant = new ExampleScript("ILocationDependant");
        ExampleScript LocationsView = new ExampleScript("LocationsView");
    }
}


