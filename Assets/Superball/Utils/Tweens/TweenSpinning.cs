using UnityEngine;

namespace HcUtils
{
    //Rotating target with angular speed
    public class TweenSpinning : FriendsGamesTools.UI.TweenInTime
    {
        [SerializeField]
        private Transform target;

        [SerializeField] 
        private Vector3 rotation;

        protected override void OnProgress(float progress)
        {
            var targetRotation = target.rotation.eulerAngles + rotation * Time.deltaTime;
            target.rotation = Quaternion.Euler(targetRotation);
        }

    }
}