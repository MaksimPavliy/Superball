using FriendsGamesTools;
using FriendsGamesTools.EditorTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityAsyncAwaitUtil
{
    public class AsyncCoroutineRunner : MonoBehaviour
    {
        static AsyncCoroutineRunner _instance;

        public static AsyncCoroutineRunner Instance
        {
            get
            {
                if (_instance == null && !Application.isPlaying)
                    _instance = Utils.FindSceneObjectsWithInactive<AsyncCoroutineRunner>().FirstOrDefault();
                if (_instance == null)
                {
                    _instance = new GameObject("AsyncCoroutineRunner")
                        .AddComponent<AsyncCoroutineRunner>();
                }

                return _instance;
            }
        }

        void Awake()
        {
            // Don't show in scene hierarchy
            gameObject.hideFlags = HideFlags.HideAndDontSave;

            DontDestroyOnLoad(gameObject);
        }

        // Changed to make it work in editor using FGT's EditorCoroutine.
        public static new void StartCoroutine(IEnumerator ienumerator)
        {
#if UNITY_EDITOR && EDITOR_TOOLS
            if (Application.isPlaying)
                StartRuntimeCoroutine(ienumerator);
            else
                EditorCoroutine.StartCoroutine(ienumerator);
#else
            StartRuntimeCoroutine(ienumerator);
#endif
        }
        static void StartRuntimeCoroutine(IEnumerator ienumerator) => (Instance as MonoBehaviour).StartCoroutine(ienumerator);
    }
}
