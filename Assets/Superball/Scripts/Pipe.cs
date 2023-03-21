using SplineMesh;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Superball
{
    public class Pipe : MonoBehaviour
    {
        public int typeIndex;

        [SerializeField] private PipeEntrance[] entrances;
        [SerializeField] private Material[] materials;
        [SerializeField] private Spline _spline;
        [SerializeField] private Rect _bounds;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            var pt1 = transform.TransformPoint(new Vector3(_bounds.xMin,0,_bounds.yMin));
            var pt2 = transform.TransformPoint(new Vector3(_bounds.xMin, 0, _bounds.yMax));
            var pt3 = transform.TransformPoint(new Vector3(_bounds.xMax, 0, _bounds.yMax));
            var pt4 = transform.TransformPoint(new Vector3(_bounds.xMax, 0, _bounds.yMin));

            Gizmos.DrawLine(pt1, pt2);
            Gizmos.DrawLine(pt2, pt3);
            Gizmos.DrawLine(pt3, pt4);
            Gizmos.DrawLine(pt1, pt4);
        }
        private int counter;

        private SuperballGeneralConfig _config => SuperballGeneralConfig.instance;

        public CurveSample GetSampleAtDistance(float distance) => _spline.GetSampleAtDistance(distance);
        public CurveSample GetClosestSample(Vector3 position) => _spline.GetProjectionSample(_spline.transform.InverseTransformPoint(position) );
        public Vector3 GetSampleWorldPosition(CurveSample sample) => _spline.transform.TransformPoint(sample.location);
        public Vector3 GetSampleWorldDirection(CurveSample sample) => _spline.transform.TransformDirection(sample.tangent.normalized);
        public PipeEntrance[] Entrances => entrances;
        public float Length=>_spline.Length;

        private void Start()
        {
            GetComponentInChildren<SplineMeshTiling>().material = materials[Random.Range(0, materials.Length)];
        }

        private void OnValidate()
        {
            if (_spline == null) _spline = GetComponentInChildren<Spline>();
            

        }

        public void Spawn(Vector3 position)
        {
            transform.position = new Vector3(position.x,position.y);

        }

       
    }
}