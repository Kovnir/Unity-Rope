using BezierCurves;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class Rope : MonoBehaviour
    {
        [SerializeField] private Transform ropeStart;
        [SerializeField] private Transform ropeEnd;
        [SerializeField] private float length;
        
        [SerializeField] private Transform ropeMiddledsa;
        
        [Space] [SerializeField] [Range(3, 50)]
        private int segmentsCount = 10;

        private LineRenderer lineRenderer;
        private Vector3 ropeMiddle;


        private void Awake()
        {
            TryGetLineRenderer();
            UpdateMiddlePosition();
        }

        private void TryGetLineRenderer()
        {
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();
            }
        }

        private void UpdateMiddlePosition()
        {
            ropeMiddle = (ropeStart.position + ropeEnd.position) / 2 + Vector3.down * length;
        }

        private void Update()
        {
            UpdateMiddlePosition();
            UpdateLineRenderer();
        }

        private void UpdateLineRenderer()
        {
            Vector3[] segments = BezierCurve.GetCurve(
                ropeStart.position,
                ropeMiddle,
                ropeEnd.position,
                segmentsCount);
            lineRenderer.positionCount = segments.Length;
            lineRenderer.SetPositions(segments);
        }

        private void OnDrawGizmos()
        {
            if (ropeStart == null || ropeEnd == null)
            {
                return;
            }

            UpdateMiddlePosition();
            TryGetLineRenderer();
            UpdateLineRenderer();
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ropeStart.position, ropeEnd.position);
            Gizmos.DrawSphere(ropeMiddle, 0.1f);
        }
    }
}