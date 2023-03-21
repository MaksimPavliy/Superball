using Cinemachine;
using FriendsGamesTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HcUtils
{
    public class CameraSwitch : MonoBehaviourHasInstance<CameraSwitch>
    {
        private List<CinemachineVirtualCamera> cameras;
        private CinemachineBrain brain;
   
        protected override void Awake()
        {
            base.Awake();
            cameras = GetComponentsInChildren<CinemachineVirtualCamera>(true).ToList();
        }

        private void Start() => brain = cameras[0].GetComponentInParent<CinemachineBrain>();
        public void ActivateCamera(string name)
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].gameObject.SetActive(cameras[i].gameObject.name.Equals(name));
            }

        }
        public void ActivateCamera(int index)
        {
            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].gameObject.SetActive(i == index);
            }
            
        }

        public void DisablePursuit()
        {
            cameras.ForEach(cam => cam.Follow = null);
            brain.enabled = false;
        }

    }
}