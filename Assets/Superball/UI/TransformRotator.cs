using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Superball
{
    public class TransformRotator : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 rotation;
        private void Update()
        {
            var targetRotation = target.rotation.eulerAngles+ rotation * Time.deltaTime;
            target.rotation = Quaternion.Euler(targetRotation);
        }
    }
}