#if ECS_ISO_TRAJECTORIES
using UnityEditor;
namespace FriendsGamesTools.ECSGame.Iso
{
    [CustomEditor(typeof(TrajectoryIsoView)), CanEditMultipleObjects]
    public class TrajectoryIsoViewEditor : TrajectoryViewEditor<TrajectoryIsoView>
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}
#endif