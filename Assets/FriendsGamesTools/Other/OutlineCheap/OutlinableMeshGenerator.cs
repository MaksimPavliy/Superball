#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.Outline
{
    public static class OutlinableMeshGenerator
    {
        public const string GeneratedFolderName = "OutlineCheap";
        public static string PathToGeneratedFolder => $"{FriendsGamesManager.GeneratedFolder}/{GeneratedFolderName}";
        public static Mesh CreateOutlinableMesh(Mesh meshOriginal, float weldDist = 0.01f)
        {
            var origVertices = meshOriginal.vertices;
            var weldDistSqr = weldDist * weldDist;
            List<List<int>> weldedInds = new List<List<int>>();
            Vector3 GetWeldedVertex(int ind)
            {
                var weldedPos = Vector3.zero;
                for (int i = 0; i < weldedInds[ind].Count; i++)
                    weldedPos += origVertices[weldedInds[ind][i]];
                weldedPos /= weldedInds[ind].Count;
                return weldedPos;
            }
            for (int origVertexInd = 0; origVertexInd < meshOriginal.vertexCount; origVertexInd++)
            {
                int vertexToWeldWith = -1;
                for (int weldedInd = 0; weldedInd < weldedInds.Count; weldedInd++)
                {
                    // Find center of already welded vertices.
                    Vector3 weldedPos = GetWeldedVertex(weldedInd);

                    // Check if they weld with current one.
                    var currPos = origVertices[origVertexInd];
                    if ((currPos - weldedPos).sqrMagnitude < weldDistSqr)
                    {
                        vertexToWeldWith = weldedInd;
                        break;
                    }
                }

                // Add vertex to already welded or make a new vertex.
                if (vertexToWeldWith != -1)
                    weldedInds[vertexToWeldWith].Add(origVertexInd);
                else
                    weldedInds.Add(new List<int> { origVertexInd });
            }
            var meshOutlinable = new Mesh();
            // Find weld vertices coo - mass centers of welded.
            Vector3[] weldedVertexes = new Vector3[weldedInds.Count];
            for (int i = 0; i < weldedVertexes.Length; i++)
                weldedVertexes[i] = GetWeldedVertex(i);
            meshOutlinable.vertices = weldedVertexes;
            // Find welded triangles - the same as in original mesh.
            var origTriangles = meshOriginal.triangles;
            int[] weldedTriangles = new int[origTriangles.Length];
            for (int i = 0; i < meshOriginal.triangles.Length; i++)
            {
                int origInd = origTriangles[i];
                int weldInd = weldedInds.FindIndex(origIndsToWeld => origIndsToWeld.Contains(origInd));
                weldedTriangles[i] = weldInd;
            }
            meshOutlinable.triangles = weldedTriangles;
            // Find welded normals and bone weights - average of all original ones.
            var origNormals = meshOriginal.normals;
            var origBoneWeights = meshOriginal.boneWeights;
            var weldedNormals = new Vector3[weldedInds.Count];
            var hasBones = origBoneWeights != null && origBoneWeights.Length > 0;
            var weldedBoneWeights = hasBones ? new BoneWeight[weldedInds.Count] : null;
            for (int i = 0; i < weldedInds.Count; i++)
            {
                Vector3 weldedNormal = Vector3.zero;
                for (int j = 0; j < weldedInds[i].Count; j++)
                {
                    int origInd = weldedInds[i][j];
                    weldedNormal += origNormals[origInd];
                }
                weldedNormal.Normalize();
                weldedNormals[i] = weldedNormal;
                if (hasBones)
                    weldedBoneWeights[i] = origBoneWeights[weldedInds[i][0]]; // Just take one original bone weight.
            }
            meshOutlinable.normals = weldedNormals;
            if (hasBones)
            {
                meshOutlinable.boneWeights = weldedBoneWeights;
                meshOutlinable.bindposes = meshOriginal.bindposes;
            }
            meshOutlinable.RecalculateBounds();
            var name = GetKey(meshOriginal);
            var path = $"{PathToGeneratedFolder}/{name}.asset";
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            AssetDatabase.CreateAsset(meshOutlinable, path);
            meshOutlinable = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            OutlinableSettings.instance.outlinables.Add(
                new OutlinableSettings.Outlinable { mesh = meshOutlinable, hash = GetHash(meshOriginal), weld = weldDist });
            EditorUtility.SetDirty(OutlinableSettings.instance);
            return meshOutlinable;
        }
        private static long GetHash(Mesh meshOriginal)
            => AssetDatabase.GetAssetPath(meshOriginal).ToHash().ToHash(meshOriginal.name.ToHash());
        private static string GetKey(Mesh meshOriginal)
            => $"{meshOriginal.name}_{GetHash(meshOriginal)}";
        public static Mesh GetOutlinableMesh(Mesh meshOriginal, float weldDist = 0.01f)
        {
            EnsureSettingsExist();
            var hash = GetHash(meshOriginal);
            var meshSetting = OutlinableSettings.instance.Get(hash);
            var mesh = meshSetting?.mesh ?? null;
            if (meshSetting==null || meshSetting.weld!=weldDist)
                mesh = CreateOutlinableMesh(meshOriginal, weldDist);
            return mesh;
        }
        static void EnsureSettingsExist()
        {
            // Not the best way. Calls editor assembly method from editor-defined, but default-assembly script.
            var genType = ReflectionUtils.GetTypeByName("SettingsInEditor`1", true);
            var type = genType.MakeGenericType(typeof(OutlinableSettings));
            type.CallStaticMethod("EnsureExists");
        }
    }
}
#endif