using System;
using UnityEngine;

namespace HcUtils
{
    public interface ISkinSettable
    {
        void SetSkin(int ind);
    }

    public interface ISkinController
    {
        void Subscribe(Action<int> action);
        void Unsubscribe(Action<int> action);
        void InvokeSkinActivated(int ind);
        int GetSkinInd();

    }
    public abstract class CharacterSkinSetter : MonoBehaviour
    {
        private ISkinSettable skinSettable;

        private ISkinController skinController;

        protected virtual object controller => null;
        
        [SerializeField]
        private MonoBehaviour skin;

        private void Start()
        {
            if (controller is ISkinController && skin is ISkinSettable)
            {
                skinController = controller as ISkinController;
                skinSettable = skin as ISkinSettable;

                if (skinSettable != null)
                {
                    SetSkin(skinController.GetSkinInd());
                    skinController.Subscribe(SetSkin);
                }
            }
        }

        private void OnDestroy() => skinController?.Unsubscribe(SetSkin);
        void SetSkin(int ind) => skinSettable.SetSkin(ind);
    }
}