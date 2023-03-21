using System.Collections.Generic;
using UnityEngine;

namespace HcUtils
{
    public class CameraSelector : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> cameras;

        [SerializeField] 
        private List<Transform> transforms;

        [SerializeField]
        private Transform cameraParent;

        [SerializeField]
        private Transform cinematicTransform;

        protected Transform targetTransform = null;
        private bool cinematic = false;
        public int Count => cameras.Count;
        
        int lastCameraIndex = -1;
        protected virtual int activeIndex => 0;

        protected virtual void Start()
        {
            SetActiveCamera(activeIndex);
        }

        protected virtual void StartCinematic()
        {
            cinematic = true;
            targetTransform = cinematicTransform;
        }
        public virtual void SetActiveCamera(int index)
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].SetActive(i == index);
            }

            for (int i = 0; i < transforms.Count; i++)
            {
                transforms[i].gameObject.SetActive(i == index);
                if (i == index) targetTransform = transforms[i];
            }
        }

        protected virtual void Update()
        {
            if (targetTransform)
            {
                cameraParent.position = Vector3.Lerp(cameraParent.position, targetTransform.position, (cinematic?15:5f) * Time.deltaTime);
                cameraParent.rotation = Quaternion.Lerp(cameraParent.rotation, targetTransform.rotation, (cinematic?10f:2f) * Time.deltaTime);
            }
        }
    }
}