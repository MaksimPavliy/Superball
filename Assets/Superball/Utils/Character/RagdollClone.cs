#if UNITY_EDITOR
using UnityEngine;

namespace HcUtils
{
    //Copying ragdoll from same-rig model
    [ExecuteAlways]
    public class RagdollClone : MonoBehaviour
    {
        [SerializeField] Transform targetTransform;

        [ContextMenu("Clone")]
        private void Clone()
        {
            var transforms = GetComponentsInChildren<Transform>();

            RemoveallComponents<Joint>(transforms);
            RemoveallComponents<Collider>(transforms);
            RemoveallComponents<Rigidbody>(transforms);

            CloneTransforms(transforms);
            CloneComponents<Rigidbody>(transforms);
            CloneComponents<Collider>(transforms);
            CloneComponents<Joint>(transforms);
        }

        void RemoveallComponents<T>(Transform[] targetTransforms) where T : Component
        {
            foreach (Transform tr in targetTransforms)
            {
                var existingComp = tr.GetComponent<T>();
                DestroyImmediate(existingComp);
            }
        }

        void CloneTransforms(Transform[] targetTransforms)
        {
            var components = targetTransform.GetComponentsInChildren<Transform>();
            foreach (var comp in components)
            {
                var name = comp.gameObject.name;
                foreach (Transform tr in targetTransforms)
                {
                    if (name == tr.gameObject.name)
                    {
                        var existingComp = tr.GetComponent<Transform>();
                        existingComp.rotation = comp.rotation;
                        existingComp.position = comp.position;
                        existingComp.localScale = comp.localScale;
                    }
                }

            }
        }
        void CloneComponents<T>(Transform[] targetTransforms) where T : Component
        {
            var components = targetTransform.GetComponentsInChildren<T>();
            var componentType = typeof(T);
            foreach (var comp in components)
            {
                var compType = comp.GetType();
                var name = (comp as Component).gameObject.name;
                foreach (Transform tr in targetTransforms)
                {
                    if (name == tr.gameObject.name)
                    {
                        Debug.Log(name);
                        var existingComp = tr.GetComponent<T>();
                        if (existingComp != null)
                        {
                            Debug.Log(existingComp.name + " " + typeof(T));
                            DestroyImmediate(existingComp);
                        }
                        var newComponent = tr.gameObject.AddComponent(comp.GetType());
                        ComponentsCopy.GetCopyOf<Component>(newComponent, comp as Component);
                    }
                }
            }
        }
    }
}
#endif