using UnityEngine;

namespace FriendsGamesTools
{
    public static class DebugDrawUtils
    {
        public static void DrawBox(Vector3 pos, Vector2 size, Vector3 normal, Vector3 up, Color col) {
            var toSide = Vector3.Cross(normal, up).normalized;
            var toUp = Vector3.Cross(toSide, normal).normalized;
            toSide *= size.x * 0.5f;
            toUp *= size.y * 0.5f;
            Debug.DrawLine(pos + toSide + toUp, pos + toSide - toUp, col);
            Debug.DrawLine(pos - toSide + toUp, pos - toSide - toUp, col);
            Debug.DrawLine(pos + toSide + toUp, pos - toSide + toUp, col);
            Debug.DrawLine(pos + toSide - toUp, pos - toSide - toUp, col);
        }
        public static void DrawCross(Vector3 pos, float size, Color col, float duration = 0)
        {
            Debug.DrawLine(pos + new Vector3(size, 0, 0), pos - new Vector3(size, 0, 0), col, duration);
            Debug.DrawLine(pos + new Vector3(0, size, 0), pos - new Vector3(0, size, 0), col, duration);
            Debug.DrawLine(pos + new Vector3(0, 0, size), pos - new Vector3(0, 0, size), col, duration);
        }
        public static void DrawCircle(Vector3 pos, Vector3 normal, float radius, Color col, float sectionLength = 1f, int minSections = 3, float duraiton = 0) {
            var toSide = Utils.GetPerpendicular(normal).normalized * radius;
            var ptsCount = Mathf.Max(minSections, Mathf.CeilToInt(radius * Mathf.PI * 2f / sectionLength));
            var rotate = Quaternion.AngleAxis(360f / ptsCount, normal);
            var prevPt = toSide;
            for (int i = 0; i < ptsCount; i++) {
                var nextPt = rotate * prevPt;
                Debug.DrawLine(prevPt + pos, nextPt + pos, col, duraiton);
                prevPt = nextPt;
            }
        }
    }
}