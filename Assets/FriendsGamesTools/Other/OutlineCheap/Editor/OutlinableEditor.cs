using System;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.Outline
{
    [CustomEditor(typeof(Outlinable)), CanEditMultipleObjects]
    public class OutlinableEditor : Editor
    {
        Outlinable tgt => (Outlinable)target;
        OutlinableSettings settings => SettingsInEditor<OutlinableSettings>.instance;
        enum SettingsMode { shared, custom }
        SettingsMode mode = SettingsMode.shared;
        bool customSettings => mode == SettingsMode.custom;
        private void OnEnable()
        {
            CreateDefaultMaterialIfNeeded();
            mode = (tgt.weldDist == settings.weldDist && tgt.outlineMat == settings.material)
                ? SettingsMode.shared:SettingsMode.custom;
            if (meshGenerationAllowed && tgt.outlinables.Count == 0 && tgt.enabled)
                UpdateOutlinables();
        }

        private void CreateDefaultMaterialIfNeeded()
        {
            if (OutlinableSettings.instance.material != null)
                return;
            var path = $"{OutlinableMeshGenerator.PathToGeneratedFolder}/Outlinable.mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null)
                return;
            mat = new Material(Shader.Find("Mobile/OutlineCheap"));
            AssetDatabase.CreateAsset(mat, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            OutlinableSettings.instance.material = AssetDatabase.LoadAssetAtPath<Material>(path);
            EditorUtils.SetDirty(OutlinableSettings.instance);
        }

        bool multiple => targets.Length > 1;
        void ForEachTarget(Action<Outlinable> action) => targets.ForEach(target => action((Outlinable)target));
        public override void OnInspectorGUI()
        {
            GUI.enabled = meshGenerationAllowed;
            var changed = false;
            var changedShared = false;
            if (EditorGUIUtils.Toolbar("settings", ref mode, ref changed))
            {
                if (!customSettings)
                    ApplySharedSettings();
            }
            if (!customSettings)
            {
                EditorGUIUtils.ObjectField("material", ref settings.material, ref changedShared, -1, false);
                EditorGUIUtils.FloatField("weld dist", ref settings.weldDist, ref changedShared);
            } else
            {
                if (!multiple)
                {
                    EditorGUIUtils.ObjectField("material", ref tgt.outlineMat, ref changed, -1, false);
                    EditorGUIUtils.FloatField("weld dist", ref tgt.weldDist, ref changed);
                }
            }
            if (EditorGUIUtils.LayerField("layer above posteffects", ref settings.layerAbovePostEffects, ref changed))
                UpdateOutlinables();
            if (GUILayout.Button("update"))
                UpdateOutlinables();
            GUI.enabled = true;
            var outlined = tgt.outlined;
            if (EditorGUIUtils.Toggle("outline shown", ref outlined, ref changed))
                ForEachTarget(tgt=>tgt.outlined = outlined);
            if (changedShared) {
                if (!customSettings)
                    ApplySharedSettings();
                EditorUtils.SetDirty(settings);
            }
            if (changed)
                EditorUtils.SetDirty(tgt);
            if (!meshGenerationAllowed)
                EditorGUIUtils.Error("open prefab to edit");
        }
        void ApplySharedSettings()
        {
            ForEachTarget(tgt =>
            {
                tgt.outlineMat = settings.material;
                tgt.weldDist = settings.weldDist;
            });
        }


        #region Mesh generation
        bool meshGenerationAllowed => !Application.isPlaying && Utils.PrefabChangesAllowed(tgt.gameObject);
        void UpdateOutlinables() => ForEachTarget(tgt => UpdateOutlinables(tgt));
        private void UpdateOutlinables(Outlinable tgt)
        {
            if (tgt.outlineMat == null)
                tgt.outlineMat = settings.material;
            const string outlineStr = "(Outline)";
            const string overOutlineStr = "(OverOutline)";
            var originalTransforms = tgt.transform.GetComponentsInChildren<MeshFilter>(true).ConvertAll(m => m.transform);
            originalTransforms.AddRange(tgt.transform.GetComponentsInChildren<SkinnedMeshRenderer>(true).ConvertAll(m => m.transform));
            for (int i = originalTransforms.Count - 1; i >= 0; i--)
            {
                var o = originalTransforms[i];
                if (o != null && (o.gameObject.name.EndsWith(outlineStr) || o.gameObject.name.EndsWith(overOutlineStr)))
                {
                    MonoBehaviour.DestroyImmediate(o.gameObject);
                    o = null;
                }
                if (o == null)
                    originalTransforms.RemoveAt(i);
            }
            for (int i = originalTransforms.Count - 1; i >= 0; i--)
            {
                if (originalTransforms[i] == null)
                    originalTransforms.RemoveAt(0);
            }
            tgt.outlinables.Clear();
            tgt.cloneOverOutlinables.Clear();
            var postProcessingMode = settings.layerAbovePostEffects != tgt.gameObject.layer;
            originalTransforms.ForEach(o =>
            {
                var outlinableGO = CreateClone(outlineStr, o,
                    mesh => OutlinableMeshGenerator.GetOutlinableMesh(mesh, tgt.weldDist),
                    tgt.outlineMat);

                tgt.outlinables.Add(outlinableGO);
                if (postProcessingMode)
                    tgt.cloneOverOutlinables.Add(CreateClone(overOutlineStr, o, mesh => mesh,
                        o.GetComponent<Renderer>().sharedMaterial));
            });
            tgt.transform.IterateChildren(tgtChild => EditorUtility.SetDirty(tgtChild.gameObject));
        }
        GameObject CreateClone(string suffix, Transform original, Func<Mesh, Mesh> changeMesh, Material material)
        {
            var originalMeshFilter = original.GetComponent<MeshFilter>();
            var originalSkinnedMeshRenderer = original.GetComponent<SkinnedMeshRenderer>();
            var mesh = originalMeshFilter != null ? originalMeshFilter.sharedMesh : originalSkinnedMeshRenderer.sharedMesh;
            mesh = changeMesh(mesh);
            var cloneName = $"{tgt.gameObject.name}{suffix}";
            var outlinableGO = originalMeshFilter!=null?
                new GameObject(cloneName, typeof(MeshRenderer), typeof(MeshFilter)) :
                 new GameObject(cloneName, typeof(SkinnedMeshRenderer));
            outlinableGO.layer = settings.layerAbovePostEffects;
            outlinableGO.transform.parent = original;
            outlinableGO.transform.SetDefaultLocalPosition();
            if (originalMeshFilter != null)
            {
                var meshFilter = outlinableGO.GetComponent<MeshFilter>();
                meshFilter.sharedMesh = mesh;
                var renderer = outlinableGO.GetComponent<MeshRenderer>();
                renderer.material = material;
                renderer.receiveShadows = false;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            } else
            {
                var skinned = outlinableGO.GetComponent<SkinnedMeshRenderer>();
                skinned.sharedMesh = mesh;
                skinned.material = material;
                skinned.rootBone = originalSkinnedMeshRenderer.rootBone;
                skinned.receiveShadows = false;
                skinned.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                skinned.bones = originalSkinnedMeshRenderer.bones;
            }
            return outlinableGO;
        }
        #endregion
    }
}