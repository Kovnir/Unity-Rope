using UnityEngine;

namespace Kovnir.Rope.Editor
{
    public static class GLDraw
    {
        public static void Rect(float x, float y, float width, float height, Color color)
        {
            GL.Begin(GL.QUADS);
            GL.Color(color);
            GL.Vertex3(x, y, 0);
            GL.Vertex3(width, y, 0);
            GL.Vertex3(width, height, 0);
            GL.Vertex3(x, height, 0);
            GL.End();
        }

        public static void EmptyRect(float x, float y, float width, float height, Color color)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex3(x, y, 0);
            GL.Vertex3(width, y, 0);
            GL.Vertex3(width, y, 0);
            GL.Vertex3(width, height, 0);
            GL.Vertex3(width, height, 0);
            GL.Vertex3(x, height, 0);
            GL.Vertex3(x, height, 0);
            GL.Vertex3(x, y, 0);
            GL.End();
        }

        public static void EmptyRect(float x, float y, float width, float height, int thickness, Color color)
        {
            for (int i = 0; i < thickness; i++)
            {
                EmptyRect(x + i, y + i, width - i, height - i, color);
            }
        }

        public static void Line(float x1, float y1, float x2, float y2, Color color)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);
            GL.Vertex3(x1, y1, 0);
            GL.Vertex3(x2, y2, 0);
            GL.End();
        }

        public static void Lines(Color color, params Vector2[] points)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);
            for (int i = 0; i < points.Length - 1; i++)
            {
                GL.Vertex3(points[i].x, points[i].y, 0);
                GL.Vertex3(points[i + 1].x, points[i + 1].y, 0);
            }

            GL.End();
        }

        public static void Lines(Color color, Rect bounds, params Vector2[] points)
        {
            GL.Begin(GL.LINES);
            GL.Color(color);
            for (int i = 0; i < points.Length - 1; i++)
            {
                if (!bounds.Contains(points[i]) || !bounds.Contains(points[i + 1]))
                {
                    //change the point to stay inside the bounds
                    var p1 = points[i];
                    var p2 = points[i + 1];
                    if (p1.y < bounds.yMin)
                    {
                        p2.x += (p2.x - p2.x) * (bounds.yMin - p1.y) / (p2.y - p1.y);
                        p1.y = bounds.yMin;
                    }
                    else if (p1.y > bounds.yMax)
                    {
                        p2.x += (p2.x - p2.x) * (bounds.yMax - p1.y) / (p2.y - p1.y);
                        p1.y = bounds.yMax;
                    }
                    if (p2.y < bounds.yMin)
                    {
                        p2.x += (p2.x - p2.x) * (bounds.yMin - p2.y) / (p1.y - p2.y);
                        p2.y = bounds.yMin;
                    }
                    else if (p2.y > bounds.yMax)
                    {
                        p2.x += (p2.x - p2.x) * (bounds.yMax - p2.y) / (p1.y - p2.y);
                        p2.y = bounds.yMax;
                    }

                    points[i] = p1;
                    points[i + 1] = p2;
                }



                GL.Vertex3(points[i].x, points[i].y, 0);
                GL.Vertex3(points[i + 1].x, points[i + 1].y, 0);
            }

            GL.End();
        }
        private static void AdjustPointToBounds(Vector2 point, Vector2 otherPoint, float bound, Rect bounds, bool adjustX = true)
        {
            if (adjustX)
            {
                point.y += (otherPoint.y - point.y) * (bound - point.x) / (otherPoint.x - point.x);
                point.x = bound;
            }
            else
            {
                point.x += (otherPoint.x - point.x) * (bound - point.y) / (otherPoint.y - point.y);
                point.y = bound;
            }
        }
    }
}