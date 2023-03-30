using Kovnir.Rope.Math;
using UnityEngine;

namespace Kovnir.Rope
{
    [RequireComponent(typeof(LineRenderer))]
    public sealed class Rope : MonoBehaviour
    {
        [SerializeField] private Transform ropeStart;
        [SerializeField] private Transform ropeEnd;
        
        [SerializeField] private float length;
        [SerializeField] private int segmentsCount = 10;
        
        [SerializeField] private SecondOrderCalculator.Params dynamicsParams;


        private LineRenderer lineRenderer;
        private Vector3 ropeMiddle;
        private Vector3? ropeMiddlePrevious;
        private Vector3 ropeTarget;

        SecondOrderCalculator calculator;

        private void Awake()
        {
            TryGetLineRenderer();
            UpdateMiddlePosition();
            InitDynamics();
            UpdateTarget();
        }

        [ContextMenu("Init Dynamics")]
        void InitDynamics()
        {
            calculator = new SecondOrderCalculator(ropeMiddle, dynamicsParams);
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
            var start = ropeStart.position;
            var end = ropeEnd.position;
            ropeMiddle = (start + end) / 2
                         + Vector3.down * Mathf.Lerp(length, 0,
                             Vector3.Distance(start, end) / length);
        }

        private void UpdateTarget()
        {
            if (Application.isPlaying)
            {
                ropeTarget = calculator.Update(Time.deltaTime, ropeMiddle);
            }
            else
            {
                ropeTarget = ropeMiddle;
            }
        }

        private void Update()
        {
            UpdateMiddlePosition();
            UpdateTarget();
            UpdateLineRenderer();
        }

        private void UpdateLineRenderer()
        {
            Vector3[] segments = BezierCurve.GetCurve(
                ropeStart.position,
                ropeTarget,
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

            if (!Application.isPlaying)
            {
                UpdateMiddlePosition();

                if (calculator == null)
                {
                    InitDynamics();
                }

                UpdateTarget();
                TryGetLineRenderer();
                UpdateLineRenderer();
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(ropeMiddle, 0.1f);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(ropeTarget, 0.1f);
        }
    }
}