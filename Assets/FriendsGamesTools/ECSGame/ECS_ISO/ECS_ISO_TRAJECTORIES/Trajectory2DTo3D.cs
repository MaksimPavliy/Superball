#if ECS_ISO_TRAJECTORIES && UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace FriendsGamesTools.ECSGame.Iso
{
    [ExecuteAlways]
    public class Trajectory2DTo3D : MonoBehaviour
    {
        private void OnEnable()
        {
            var trajectories = GetComponentInParent<TrajectoriesView>();

            var oldTr = GetComponent<TrajectoryIsoView>();
            var newTr = gameObject.AddComponent<Trajectory3DView>();
            newTr.dontKillClose = oldTr.dontKillClose;
            newTr.notNext = oldTr.notNext.Clone();
            trajectories?.trajectories.ForEach(otherTr=> {
                if (otherTr.notNext.Contains(oldTr))
                {
                    otherTr.notNext = otherTr.notNext.ConvertAll(tr => tr == oldTr ? newTr : tr);
                    EditorUtility.SetDirty(otherTr.gameObject);
                }
            });
            newTr.noNextTrajectory = oldTr.noNextTrajectory;
            newTr.ind = oldTr.ind;
            newTr.pts = oldTr.pts.Clone();
            newTr.col = oldTr.col;
            newTr.draw = oldTr.draw;
            newTr.selectedPtInd = oldTr.selectedPtInd;

            EditorUtility.SetDirty(gameObject);

            if (PrefabUtils.openedPrefab == null)
            {
                DestroyImmediate(oldTr);
                DestroyImmediate(this);
            }
        }
    }
}
#endif