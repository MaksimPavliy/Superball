using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools
{
    // set showTouches to true and see hand for each touch.
    // call CreateTouch() and show custom touch for tutorial or smth.
    public class TouchesView : MonoBehaviourHasInstance<TouchesView>
    {
        [SerializeField] TouchView prefab;
        public Camera uiCamera;
        public int StartCount = 2;
#if TOUCHES
        Queue<TouchView> pool = new Queue<TouchView>();
        TouchView CreateaInstance() => Instantiate(prefab, transform);
        void InitPool()
        {
            for (int i = 0; i < StartCount; i++)
            {
                var inst = CreateaInstance();
                inst.gameObject.SetActive(false);
                pool.Enqueue(inst);
            }
        }
        private TouchView GetFromPool()
        {
            var item = pool.Count > 0 ? pool.Dequeue() : CreateaInstance();
            item.Appear();
            return item;
        }
        public void ReturnToPool(TouchView view) => pool.Enqueue(view);
        public bool showTouches;

        public TouchView CreateTouch()
        {
            var touch = GetFromPool();
            touch.Appear();
            return touch;
        }

        Dictionary<long, Touch> touchesFromInput = new Dictionary<long, Touch>();
        Dictionary<long, TouchView> touchViewsFromInputTouches = new Dictionary<long, TouchView>();
        protected override void Awake()
        {
            base.Awake();
            InitPool();
        }
        private void Update()
        {
            touchesFromInput.Clear();
            if (showTouches)
                TouchesManager.instance.touches.ForEach(t => touchesFromInput.Add(t.id, t));
            Utils.UpdatePrefabsDictionary(touchViewsFromInputTouches, touchesFromInput,
                t => GetFromPool(), t => t.Disappear(), (t, view) => view.screenPos = t.currScreenCoo);
        }
#endif
    }
}
