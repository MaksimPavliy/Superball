#if UNITY_EDITOR && EDITOR_TOOLS
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.EditorTools
{
    [ExecuteAlways]
    public class FindMissingScripts : MonoBehaviour
    {
        public List<GameObject> withMissing = new List<GameObject>();
        private void OnEnable()
        {
            withMissing.Clear();
            FindMissingScriptsRecursively(gameObject, withMissing);
        }
        public static void FindMissingScriptsRecursively(GameObject g, List<GameObject> withMissingScripts)
        {
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    withMissingScripts.Add(g);
                    break;
                }
            }
            foreach (Transform childT in g.transform)
                FindMissingScriptsRecursively(childT.gameObject, withMissingScripts);
        }
    }
}
#endif