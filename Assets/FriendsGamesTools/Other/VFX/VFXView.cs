using UnityEngine;

namespace FriendsGamesTools.VFX
{
    [ExecuteAlways]
    public class VFXView : MonoBehaviour
    {
        public float duration = -1;
        private void OnEnable()
        {
            if (Application.isPlaying)
                return;
            duration = 0;
            GetComponentsInChildren<ParticleSystem>(false).ForEach(p => duration = Mathf.Max(p.main.duration, duration));
        }
        VFXPool pool;
        public void Show(Vector3 pos, VFXPool pool = null)
        {
            this.pool = pool;
            transform.position = pos;
            gameObject.SetActive(true);
            remaining = duration;
        }
        float remaining;
        private void Update()
        {
            if (!Application.isPlaying)
                return;
            remaining -= UnityEngine.Time.deltaTime;
            if (remaining > 0)
                return;
            gameObject.SetActive(false);
            pool?.Return(this);
        }
    }
}