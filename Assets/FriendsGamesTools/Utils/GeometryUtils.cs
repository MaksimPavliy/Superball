using UnityEngine;

namespace FriendsGamesTools
{
    public static class GeometryUtils
    {
        // Line is infinite.
        public static float CalcDistSqrToLine(Vector3 pt, Vector3 linePt1, Vector3 linePt2)
        {
            var lineDir = linePt2 - linePt1;
            var t = GetProjectionBarycentric(pt, linePt1, linePt2);
            var x = linePt1 + lineDir * t;
            var distSqr = (x - pt).sqrMagnitude;
            return distSqr;
        }
        public static float CalcDistSqrToSegment(Vector3 pt, Vector3 segmPt1, Vector3 segmPt2)
        {
            var segmDir = segmPt2 - segmPt1;
            var t = GetProjectionBarycentric(pt, segmPt1, segmPt2);
            t = Mathf.Clamp01(t);
            var x = segmPt1 + segmDir * t;
            var distSqr = (x - pt).sqrMagnitude;
            return distSqr;
        }
        public static float GetProjectionBarycentric(Vector3 pt, Vector3 linePt1, Vector3 linePt2)
        {
            // x - pt projection on line.
            var lineDir = linePt2 - linePt1;
            // x=linePt1+lineDir*t
            // (pt-x) is perpendicular to lineDir
            // Vector3.Dot(pt-x, lineDir) = 0
            // Vector3.Dot(pt-linePt1-lineDir*t, lineDir) = 0
            // Vector3.Dot(pt-linePt1, lineDir) - Vector3.Dot(lineDir, lineDir)*t = 0
            // t = Vector3.Dot(pt-linePt1, lineDir)/Vector3.Dot(lineDir, lineDir)
            var t = Vector3.Dot(pt - linePt1, lineDir) / Vector3.Dot(lineDir, lineDir);
            return t;
        }
        public static Vector2 GetClosestPtFromPtToSquare(Vector2 pt, Vector2 squareCenter, Vector2 squareSize)
        {
            // d=pt-c
            // pt = c+d*t
            // pt.x = c.x+d.x*t
            // |d.x|*t < s.x
            // t<s.x/|d.x|
            // t<s.y/|d.y|
            var d = pt - squareCenter;
            var t = 1f;
            var minByX = squareSize.x * 0.5f / Mathf.Abs(d.x);
            if (minByX.IsSane())
                t = Mathf.Min(t, minByX);
            var minByY = squareSize.y * 0.5f / Mathf.Abs(d.y);
            if (minByY.IsSane())
                t = Mathf.Min(t, minByY);
            var closestPt = squareCenter + d * t;
            return closestPt;
        }
        public static Vector3 Project(Vector3 vector, Vector3 projectOn)
            => projectOn * Vector3.Dot(vector, projectOn) / projectOn.sqrMagnitude;
    }
}
