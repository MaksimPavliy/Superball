#if ECS_TRAJECTORIES
using UnityEditor;

namespace FriendsGamesTools.ECSGame.Iso
{
    [CustomEditor(typeof(Trajectory3DView))]
    public class Trajectory3DViewEditor : TrajectoryViewEditor<Trajectory3DView>
    {
    }
}
#endif