using UnityEngine;

namespace FriendsGamesTools
{
    public static class GizmosUtils
    {
        public static void DrawSphere(Vector3 pos, float radius, Color col) {
            Gizmos.color = col;
            Gizmos.matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
            Gizmos.DrawSphere(Vector3.zero, radius);
        }
        public static void DrawWireCube(Vector3 pos, Vector3 size, Color col)
            => DrawCube(pos, size, Quaternion.identity, col, true);
        public static void DrawCube(Vector3 pos, Vector3 size, Quaternion rotation, Color col, 
            bool wire = false)
        {
            Gizmos.color = col;
            Gizmos.matrix = Matrix4x4.TRS(pos, rotation, Vector3.one);
            if (wire)
                Gizmos.DrawWireCube(Vector3.zero, size);
            else
                Gizmos.DrawCube(Vector3.zero, size);
        }
        public static void DrawCube(Vector3 pos, Vector3 size, Color col,
            bool wire = false)
            => DrawCube(pos, size, Quaternion.identity, col, wire);
        public static void DrawLine(Vector3 pt1, Vector3 pt2, float width, Color col)
        {
            var dir = pt2 - pt1;
            DrawCube((pt1 + pt2) * 0.5f,
                new Vector3(width, width, dir.magnitude),
                Matrix4x4.LookAt(Vector3.zero, dir, Vector3.up).ExtractRotation(), col);
        }
    }
}