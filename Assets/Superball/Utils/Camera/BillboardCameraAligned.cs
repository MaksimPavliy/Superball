using UnityEngine;
namespace HcUtils
{
    public abstract class BillboardCameraAligned : MonoBehaviour
    {
        protected virtual Camera Camera=>null;
        void Update()
        {
            if (Camera == null)
                return;
            transform.rotation=Camera.transform.rotation;
        }
    }
}