using System;
using UnityEngine;

namespace HcUtils
{
    public interface IControllable
    {
        public void OnDragged(Vector2 dir);
        public void OnDragEnded();
        public event Action<bool> CanControlChanged;
    }
}