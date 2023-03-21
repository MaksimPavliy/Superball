#if ECS_TRAJECTORIES
using FriendsGamesTools;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    [ExecuteAlways]
    public class DebugShowItemsOnTrajectory : MonoBehaviour
    {
        public Transform prefab;
        public List<GameObject> trajectoriesParents = new List<GameObject>();
        class TrajectoryWithItems
        {
            public TrajectoryView tr;
            public List<ItemOnTrajectory> items = new List<ItemOnTrajectory>();
        }
        class ItemOnTrajectory
        {
            public Transform item;
            public int ptInd;
            public float ptProgress;
        }
        List<TrajectoryWithItems> trajectories = new List<TrajectoryWithItems>();
        public float itemsIsoInterval = 1;
        const float minItemsGameSpaceInterval = 0.1f; // Dont hang trying to show bilions of items.
        const bool reverse = false;
        bool ShowAllTrajectories() => shownTrajectoryInd == -1 || shownTrajectoryInd >= trajectories.Count;
        bool ShowTrajectory(int trajectoryInd)
            => ShowAllTrajectories() || shownTrajectoryInd == trajectoryInd;

        // TODO: make this readonly.
        public int existingTrajectories;
        public TrajectoryView shownTrajectory;
        public List<TrajectoryPt> shownTrajectoryPts = new List<TrajectoryPt>();
        public int shownTrajectoryInd = -1;
        void KillExistingItems()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);
            shownTrajectoryPts.Clear();
            trajectories.Clear();
        }
        private void OnDisable()
        {
            if (!Application.isPlaying && Utils.PrefabChangesAllowed(gameObject))
                KillExistingItems();
        }
        private void OnEnable()
        {
            if (Application.isPlaying || !Utils.PrefabChangesAllowed(gameObject))
            {
                gameObject.DestroyEditOrPlayMode();
                Destroy(gameObject);
                return;
            }
            // Kill old prefabs.
            KillExistingItems();
            // Get all trajectories.
            trajectories = new List<TrajectoryWithItems>();
            trajectoriesParents.ForEach(p =>
            {
                trajectories.AddRange(p.GetComponentsInChildren<TrajectoryView>().ConvertAll(
                    t => new TrajectoryWithItems { tr = t }));
            });
            existingTrajectories = trajectories.Count;
            // Create new items.
            itemsIsoInterval = Mathf.Max(itemsIsoInterval, minItemsGameSpaceInterval);
            trajectories.ForEach(t =>
            {
                int ptInd = 0;
                float ptProgress = 0;
                bool isMoving = true;
                bool first = true;
                do
                {
                    var interval = itemsIsoInterval;
                    if (first)
                    {
                        first = false;
                        interval *= 0;// Utils.Random(0f, 1f);
                    }
                    (ptInd, ptProgress, isMoving) = TrajectoryMoving.Move(t.tr, reverse, ptInd, ptProgress, interval);
                    var worldPos = TrajectoryMoving.GetWorldPos(t.tr, ptInd, ptProgress, reverse);
                    var item = Instantiate(prefab, worldPos, prefab.rotation, transform);
                    item.gameObject.SetActive(ShowTrajectory(trajectories.IndexOf(t)));
                    t.items.Add(new ItemOnTrajectory { item = item, ptInd = ptInd, ptProgress = ptProgress });
                } while (isMoving);
            });
        }
        private void Update()
        {
            // Update item's positions.
            trajectories.ForEach(t =>
            {
                int ind = trajectories.IndexOf(t);
                t.items.ForEach(i =>
                {
                    var show = ShowTrajectory(ind);
                    i.item.gameObject.SetActive(show);
                    if (!show)
                        return;
                    var worldPos = TrajectoryMoving.GetWorldPos(t.tr, i.ptInd, i.ptProgress, reverse);
                    i.item.position = worldPos;
                });
            });
            shownTrajectory = ShowAllTrajectories() ? null : trajectories[shownTrajectoryInd].tr;
            shownTrajectoryPts.Clear();
            if (shownTrajectory != null)
                shownTrajectoryPts.AddRange(shownTrajectory.pts);
        }
    }
}
#endif