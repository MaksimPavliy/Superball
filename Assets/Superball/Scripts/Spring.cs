using UnityEngine;

namespace Superball
{
    public class Spring : MonoBehaviour
    {
        [SerializeField] private Transform staticAnchor;
        [SerializeField] private Transform dynamicAnchor;
        [SerializeField] private Transform scaleParent;

        private Vector3 startScale;
        private float startDistance;
        private float AnchorDistance=>Mathf.Abs(staticAnchor.position.x - dynamicAnchor.position.x);
        private void Start()
        {
            startScale = scaleParent.localScale;
            startDistance = AnchorDistance;
        }

        private void Update()
        {
            scaleParent.position = dynamicAnchor.position;
            var scale = startScale;
            scale.x*= AnchorDistance / startDistance;
            scaleParent.localScale = scale;
        }
    }
}   