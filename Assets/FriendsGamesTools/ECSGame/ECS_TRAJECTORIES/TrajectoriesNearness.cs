#if ECS_TRAJECTORIES
using System.Collections.Generic;
using System.Linq;

namespace FriendsGamesTools.ECSGame
{
    public static class TrajectoriesNearness
    {
        static Dictionary<int, List<int>> endToStart, endToEnd, startToEnd, startToStart;
        public static bool TrajectoryIsNext(int ind1, bool reverse1, int ind2, bool reverse2)
        {
            if (endToStart == null)
                return false;
            Dictionary<int, List<int>> nexts;
            if (!reverse1)
            {
                if (!reverse2)
                    nexts = endToStart;
                else
                    nexts = endToEnd;
            }
            else
            {
                if (!reverse2)
                    nexts = startToStart;
                else
                    nexts = startToEnd;
            }
            return nexts.TryGetValue(ind1, out var inds) && inds.Contains(ind2);
        }
        public static void InitNearbyTrajectories()
        {
            if (TrajectoriesView.instance == null) return;
            endToStart = new Dictionary<int, List<int>>();
            endToEnd = new Dictionary<int, List<int>>();
            startToEnd = new Dictionary<int, List<int>>();
            startToStart = new Dictionary<int, List<int>>();
            var trajectories = TrajectoriesView.instance.trajectories;
            for (int i = 0; i < trajectories.Count; i++)
            {
                var ith = trajectories[i];
                if (ith.noNextTrajectory)
                    continue;
                for (int j = 0; j < trajectories.Count; j++)
                {
                    if (i == j) continue;
                    var jth = trajectories[j];
                    var ithLast = CoordinatesFrom.GetOriginal(ith.pts.Last().posTransform);
                    if (ithLast != null)
                    {
                        if (ithLast == CoordinatesFrom.GetOriginal(jth.pts.First().posTransform))
                            AddToDic(endToStart, i, j);
                        if (ithLast == CoordinatesFrom.GetOriginal(jth.pts.Last().posTransform))
                            AddToDic(endToEnd, i, j);
                    }
                    var ithFirst = CoordinatesFrom.GetOriginal(ith.pts.First().posTransform);
                    if (ithFirst)
                    {
                        if (ithFirst == CoordinatesFrom.GetOriginal(jth.pts.First().posTransform))
                            AddToDic(startToStart, i, j);
                        if (ithFirst == CoordinatesFrom.GetOriginal(jth.pts.Last().posTransform))
                            AddToDic(startToEnd, i, j);
                    }
                }
            }
            void AddToDic(Dictionary<int, List<int>> dic, int i, int j)
            {
                if (!dic.TryGetValue(i, out var list))
                {
                    list = new List<int>();
                    dic.Add(i, list);
                }
                list.Add(j);
            }
        }
    }
}
#endif