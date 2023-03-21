using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public class ECS_ISO_TRAJECTORIES_HowTo : HowToModule
    {
        public override string forWhat => "creating isometric preset trajectories visually, moving entities on them, make queues";
        protected override void OnHowToGUI()
        {
            TrajectoriesView.ShowOnGUI("Put this to your top UI canvas for isometric content");
            TrajectoryIsoView.ShowOnGUI("Create trajectories by adding this script to some go",
                "Select pts in its inspector and position trajectory gameobject itself\n" +
                "There are buttons to add/remove pts, edit existing ones\n" +
                "Trajectories are shown on scene using gizmos");
            DebugShowItemsOnTrajectory.ShowOnGUI("Optional: put this to the scene to see prefabs all over trajectory and therefore check its z-order");
            TrajectoryMover.ShowOnGUI("Add this to entity that can move. Set its speed to smth");
            TrajectoryMoving.ShowOnGUI("Use this to start moving",
                "use <b>TrajectoryMoving.StartMove(mover, trajectoryInd)</b> to start move\n" +
                "use <b>TrajectoryMoving.SetViewPosition(e, tr)</b> or <b>TrajectoryMoving.GetWorldPos(mover)</b> to show items on their mover positions");
            Queuing.ShowOnGUI("Optional: add queues",
                "Derive from <b>Queuing<TItem></b> class\n" +
                "<b>TItem</b> is a component on entites that can queue\n" +
                "override <b>itemWidth</b> to set how far from each other they will stop");

            GUILayout.Space(10);
        }
        protected override string docsURL => "https://drive.google.com/open?id=1VT6RPa4jDTsyQOxPZQd7jsd_fcBZvjxaxsm7ytozhZg";
        ExampleScript TrajectoriesView = new ExampleScript("TrajectoriesView");
        ExampleScript TrajectoryIsoView = new ExampleScript("TrajectoryIsoView");
        ExampleScript DebugShowItemsOnTrajectory = new ExampleScript("DebugShowItemsOnTrajectory");
        ExampleScript TrajectoryMover = new ExampleScript("TrajectoryMover");
        ExampleScript TrajectoryMoving = new ExampleScript("TrajectoryMoving");
        ExampleScript Queuing = new ExampleScript("Queuing");
    }
}


