#if UI
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FriendsGamesTools.UI
{
    public class UIUnderPos : MonoBehaviourHasInstance<UIUnderPos>
    {
        [SerializeField] Canvas rootOfWorld; // 2D games are all from UI, so I need to separate game world from game UI somehow.
        [SerializeField] Transform rootTransformOfWorld;
        public bool FingersAboveUI()
        {
            int count = 0;
            IterateUIUnderFingers(go=>count++);
            return count > 0;
        }
        List<GameObject> uiUnderFingers = new List<GameObject>();
        float underFingersLastTime = -1;
        public void IterateUIUnderFingers(Action<GameObject> onUIUnderPos)
        {
            if (underFingersLastTime < Time.realtimeSinceStartup)
            {
                uiUnderFingers.Clear();
                underFingersLastTime = Time.realtimeSinceStartup;
                if (!Input.touchSupported)
                    IterateUIUnderPos(Input.mousePosition, go => uiUnderFingers.Add(go));
                else
                {
                    var touches = Input.touches;
                    touches.ForEach(t => IterateUIUnderPos(t.position, go => uiUnderFingers.Add(go)));
                }
            }
            uiUnderFingers.ForEach(onUIUnderPos);
        }
        List<RaycastResult> hits = new List<RaycastResult>();
        public void IterateUIUnderPos(Vector3 screenPos, Action<GameObject> onUIUnderPos)
        {
            if (EventSystem.current == null)
                return;
            var pe = new PointerEventData(EventSystem.current);
            pe.position = screenPos;
            hits.Clear();
            EventSystem.current.RaycastAll(pe, hits);
            foreach (RaycastResult h in hits)
            {
                var root = h.gameObject.transform.root;
                if (root == rootTransformOfWorld) continue;
                if (rootOfWorld != null && rootOfWorld.transform == root) continue;
                onUIUnderPos(h.gameObject);
            }
        }
    }
}
#endif