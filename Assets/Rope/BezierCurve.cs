using UnityEngine;

namespace BezierCurves
{
    public static class BezierCurve
    {
        private static Vector3 GetPointOnCurve(float time, Vector3 startPosition, Vector3 middlePosition,
            Vector3 endPosition)
        {
            float t = time;
            float u = 1f - t;

            Vector3 result =
                (u * u) * startPosition +
                (2 * u * t) * middlePosition +
                (t * t) * endPosition;
            return result;
        }

        public static Vector3[] GetCurve(Vector3 startPosition, Vector3 middlePosition, Vector3 endPosition,
            int segmentsCount)
        {
            Vector3[] segments = new Vector3[segmentsCount + 1];

            for (int i = 0; i <= segmentsCount; i++)
            {
                float time = i / (float)segmentsCount;
                Vector3 point = GetPointOnCurve(time, startPosition, middlePosition, endPosition);
                segments[i] = point;
            }

            return segments;
        }
    }
}