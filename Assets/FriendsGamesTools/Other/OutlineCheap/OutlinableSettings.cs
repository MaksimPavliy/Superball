using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.Outline
{
    public class OutlinableSettings : SettingsScriptable<OutlinableSettings>
    {
        [Serializable]
        public class Outlinable
        {
            public long hash;
            public Mesh mesh;
            public float weld;
        }
        public List<Outlinable> outlinables = new List<Outlinable>();
        public Material material;
        public float weldDist = 0.01f;
        public int layerAbovePostEffects = 0;
#if UNITY_EDITOR
        protected override string SubFolder => OutlinableMeshGenerator.GeneratedFolderName;
        public Outlinable Get(long hash) => outlinables.Find(o => o.hash == hash);
#else
        protected override string SubFolder => "";
#endif
    }
}