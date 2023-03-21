#if ECS_TRAJECTORIES
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    [ExecuteAlways]
    public class TrajectoriesView : MonoBehaviourHasInstance<TrajectoriesView>
    {
        public float viewScale = 1;
        public List<TrajectoryView> trajectories;
        public enum Type { SearchScene, SearchChildren }
        public Type type;
        public static void UpdateTrajectoriesListInEditMode()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return; // Only in editor.
            if (instance == null)
                return;
            if (instance.type == Type.SearchScene)
                instance.trajectories = Object.FindObjectsOfType<TrajectoryView>().ToList();
            else if (instance.type == Type.SearchChildren)
                instance.trajectories = instance.transform.GetComponentsInChildren<TrajectoryView>(true).ToList();
            for (int i = 0; i < instance.trajectories.Count; i++)
            {
                instance.trajectories[i].ind = i;
                UnityEditor.EditorUtility.SetDirty(instance.trajectories[i].gameObject);
            }
            UnityEditor.EditorUtility.SetDirty(instance.gameObject);
#endif
        }
        public static void UpdateTrajectoriesIndsInRuntime()
        {
            if (!Application.isPlaying || instance?.trajectories == null)
                return;
            var indsNotSet = instance.trajectories.All(t => t.ind == 0);
            if (indsNotSet) {
                for (int i = 0; i < instance.trajectories.Count; i++)
                    instance.trajectories[i].ind = i;
            }
            TrajectoriesNearness.InitNearbyTrajectories();
        }
        private void OnEnable()
        {
            if (!Application.isPlaying)
            {
                base.Awake();
                UpdateTrajectoriesListInEditMode();
            }
        }
        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
                UpdateTrajectoriesIndsInRuntime();
        }
    }
}
#endif