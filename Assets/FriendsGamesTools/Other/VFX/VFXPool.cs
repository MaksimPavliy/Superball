using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.VFX
{
    public class VFXPool : MonoBehaviour
    {
        [SerializeField] VFXView prefab;
        [SerializeField] int initialCount = 10;
        Queue<VFXView> inPool = new Queue<VFXView>();
        void Awake()
        {
            for (int i = 0; i < initialCount; i++)
                Instantiate();
        }
        void Instantiate()
        {
            var inst = MonoBehaviour.Instantiate(prefab, transform);
            inst.gameObject.SetActive(false);
            inPool.Enqueue(inst);
        }
        public void Show(Vector3 pos)
        {
            if (inPool.Count == 0)
                Instantiate();
            var inst = inPool.Dequeue();
            inst.Show(pos, this);
        }
        public void Return(VFXView inst)
        {
            inPool.Enqueue(inst);
        }
    }
}