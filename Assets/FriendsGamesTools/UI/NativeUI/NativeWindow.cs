using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    public abstract class NativeWindow : MonoBehaviour
    {
        [SerializeField] Button close;
#if UI
        protected static NativeWindow Show(string path)
        {
            var prefab = Resources.Load<NativeWindow>(path);
            var inst = Instantiate(prefab);
            inst.Show();
            return inst;
        }
        protected virtual void Show() {
            close?.onClick.AddListener(OnClosePressed);
        }
        public virtual void OnClosePressed() => Close();
        protected virtual void Close() => Destroy(gameObject);
        protected virtual void Update() { }
#endif
    }
}