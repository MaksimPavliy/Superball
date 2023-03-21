#if ECS_GAMEROOT

using UnityEngine;

namespace FriendsGamesTools.ECSGame
{
    public abstract class ViewControllerBase : MonoBehaviour
    {
        public ViewControllerBase() { }
        protected static GameRoot root => GameRoot.instance;
        public static T GetViewController<T>() where T : ViewControllerBase => root.GetViewController<T>();
        public static T Get<T>() where T : Controller => root.Get<T>();
        public virtual void InitDefault() { }
        public virtual void OnInited() { }
        public virtual void OnUpdate() { }
    }
    public abstract class ViewController<TSelf> : ViewControllerBase
        where TSelf : ViewController<TSelf>
    {
        public static TSelf instance { get; private set; }
        public static TSelf EnsureInstance()
        {
            if (instance != null) return instance;
            instance = root.GetComponentInChildren<TSelf>();
            if (instance != null) return instance;
            var ViewControllersName = "ViewControllers";
            var parent = root.transform.GetChild(ViewControllersName);
            if (parent == null)
            {
                parent = new GameObject(ViewControllersName).transform;
                parent.parent = root.transform;
                parent.SetDefaultLocalPosition();
            }
            var tr = new GameObject(typeof(TSelf).Name).transform;
            tr.parent = parent;
            tr.SetDefaultLocalPosition();
            instance = tr.gameObject.AddComponent<TSelf>();
            return instance;
        }
        public ViewController() : base() => instance = (TSelf)this;
    }
    public interface IOnSceneLoaded { void OnSceneLoaded(); }
}
#endif