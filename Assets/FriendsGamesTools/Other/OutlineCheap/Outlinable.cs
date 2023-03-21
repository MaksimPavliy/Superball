using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.Outline
{
    [ExecuteAlways]
    public class Outlinable : MonoBehaviour
    {
        public List<GameObject> outlinables = new List<GameObject>();
        public List<GameObject> cloneOverOutlinables = new List<GameObject>();

        public Material outlineMat;
        public float weldDist = 0.01f;
        public bool outlined
        {
            get => outlinables.Count > 0 ? outlinables[0].activeSelf : false;
            set
            {
                outlinables.ForEach(o => o.SetActive(value));
                cloneOverOutlinables.ForEach(o => o.SetActive(value));
            }
        }
    }
}