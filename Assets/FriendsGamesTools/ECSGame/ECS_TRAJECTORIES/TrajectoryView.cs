#if ECS_TRAJECTORIES
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    [ExecuteAlways]
    public abstract class TrajectoryView : MonoBehaviour
    {
        public bool dontKillClose;
        public List<TrajectoryView> notNext = new List<TrajectoryView>();
        public bool noNextTrajectory;
        public int ind;
        public List<TrajectoryPt> pts = new List<TrajectoryPt>();
        public Color col = Color.green;
        public float PtSize => 20f * viewScale;
        float viewScale => TrajectoriesView.instance != null ? TrajectoriesView.instance.viewScale : 1;
        const float SelectedPtSizeMul = 2f;
        const float LineSizeMul = 0.3f;
        const float SelectedTrajectorySizeMul = 3f;
        private void OnEnable() => TrajectoriesView.UpdateTrajectoriesListInEditMode();
        private void OnDrawGizmos() => DrawGizmos(1);
        public bool draw = true;
        private void DrawGizmos(float linesSizeMul)
        {
            if (!draw) return;
            // Draw lines.
            for (int i = 0; i < pts.Count - 1; i++)
            {
                var pt = pts[i];
                var pt2 = pts[i + 1];
                var dir = pt2.pos - pt.pos;
                GizmosUtils.DrawLine(pt.pos, pt2.pos, linesSizeMul * PtSize * LineSizeMul, col);
            }
            // Draw pts.
            for (int i = 0; i < pts.Count; i++)
            {
                var pt = pts[i];
                GizmosUtils.DrawCube(pt.pos, Vector3.one * PtSize, col);
            }
        }
        public int selectedPtInd;
        public TrajectoryPt selectedPt => pts.IndIsValid(selectedPtInd) ? pts[selectedPtInd] : null;
        public TrajectoryPt prevPt => selectedPtInd > 0 ? pts[selectedPtInd - 1] : null;
        public TrajectoryPt nextPt => selectedPtInd < pts.Count - 1 ? pts[selectedPtInd + 1] : null;
        public TrajectoryPt GetPt(int ind, bool reverse) => reverse ? pts[pts.Count - 1 - ind] : pts[ind];
        private void OnDrawGizmosSelected()
        {
            if (!draw) return;
            DrawGizmos(SelectedTrajectorySizeMul);
            if (pts.IndIsValid(selectedPtInd))
            {
                var pt = pts[selectedPtInd];
                GizmosUtils.DrawCube(pt.pos, Vector3.one * PtSize * SelectedPtSizeMul, col);
            }
        }
        //private void OnDestroy()
        //{
        //    pts.ForEach(pt => pt.Destroy());
        //}
        public virtual float CalcTotalLength()
        {
            if (pts.Count == 0)
                return 0;
            float length = 0;
            var iso = pts[0].pos;
            for (int i = 0; i < pts.Count - 1; i++)
                length += (pts[i].pos - pts[i + 1].pos).magnitude;
            return length;
        }

        #region Space-specific
        // Game space is 3D-space for 3D-games and Isometric space for isometric games.
        public abstract Vector3 GetGameSpacePos(TrajectoryPt pt);
        public abstract void SetGameSpacePos(TrajectoryPt pt, Vector3 gameSpacePos);
        public abstract float GetGameSpaceDist(Vector3 gameSpacePos1, Vector3 gameSpacePos2);
        public float GetGameSpaceDist(TrajectoryPt pt1, TrajectoryPt pt2)
            => GetGameSpaceDist(GetGameSpacePos(pt1), GetGameSpacePos(pt2));
        public abstract string gameSpaceName { get; }
        #endregion
    }
}
#endif