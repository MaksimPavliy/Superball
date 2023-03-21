using SplineMesh;
using UnityEngine;

namespace PoopyTube
{
    public class SplineDebugView : MonoBehaviour
    {
        [SerializeField] private Spline m_spline;
        [SerializeField] private Color m_color;
        [SerializeField] private int m_pointsCount = 10;
        [SerializeField] private bool m_road = false;
        [SerializeField] private float _roadWidth = 1f;
        private void OnValidate()
        {
            if (m_spline == null) m_spline = GetComponentInChildren<Spline>();
        }
        private void OnDrawGizmos()
        {
            if (m_spline == null) return;
            Gizmos.color = m_color;

            float length = m_spline.Length;
            float t = 0;
            Vector3 lastPosition = m_spline.transform.TransformPoint(m_spline.GetSample(t).location);
            Gizmos.DrawWireSphere(lastPosition, 0.2f);

            while (t < length)
            {
                t = Mathf.MoveTowards(t, length, length / m_pointsCount);
                CurveSample sample = m_spline.GetSampleAtDistance(t);
                Vector3 position = m_spline.transform.TransformPoint(sample.location);
                Gizmos.DrawLine(lastPosition, position);
                if (m_road)
                {
                    Vector3 right = m_spline.transform.TransformDirection(Vector3.Cross(sample.tangent, sample.up));
                    Gizmos.DrawLine(position + right * _roadWidth/2f, position - right * _roadWidth/2f);
                }

                lastPosition = position;
            }

        }
    }
}