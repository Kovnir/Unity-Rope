using System;
using GLHelper;
using UnityEngine;

namespace DefaultNamespace
{
    //inspired by (stolen from) https://www.youtube.com/watch?v=KPoeNZZ6H4s
    public sealed class SecondOrderDynamics
    {
        [Serializable]
        public struct Params
        {
            public float f;
            public float z;
            public float r;
        }
        
        public struct Consts
        {
            public float _w;
            public float _z;
            public float _d;
            public float k1;
            public float k2;
            public float k3;
        }

        private Vector3 previousTargetPosition;
        private Vector3 currentPosition;
        private Vector3 currentVelocity;
        private Consts consts;

        public SecondOrderDynamics(Vector3 initialPosition, Params @params)
        {
            currentPosition = initialPosition;
            previousTargetPosition = initialPosition;
            currentVelocity = Vector3.zero;

            float f = @params.f;
            float z = @params.z;
            float r = @params.r;

            float _w = 2 * Mathf.PI * f;
            float _z = z;
            float  _d = _w * Mathf.Sqrt(Mathf.Abs(_z * _z - 1));
            float  k1 = z / (Mathf.PI * f);
            float  k2 = 1 / (_w * _w);
            float  k3 = r * z / _w;
            
            consts = new Consts
            {
                _w = _w,
                _z = _z,
                _d = _d,
                k1 = k1,
                k2 = k2,
                k3 = k3
            };
        }

        public Vector3 Update(float deltaTime, Vector3 targetPosition)
        {
            (currentPosition, currentVelocity) = Update(deltaTime, targetPosition, previousTargetPosition, currentPosition, currentVelocity, consts);
            previousTargetPosition = targetPosition;
            return currentPosition;
        }
        
        
        public static (Vector3 currentPosition, Vector3 currentVelocity) Update(
            float deltaTime,
            Vector3 targetPosition, Vector3 previousTargetPosition,
            Vector3 currentPosition, Vector3 currentVelocity,
            Consts consts)
        {
            Vector3 deltaPosition = targetPosition - previousTargetPosition; //estimate velocity
            float k2Stable;

            if (consts._w * deltaTime < consts._z) //clamp k2 to avoid instability with jitters
            {
                k2Stable = Mathf.Max(Mathf.Max(consts.k2, deltaTime * deltaTime / 2 + deltaTime * consts.k1 / 2), deltaTime * consts.k1);
            }
            else
            {
                float t1 = Mathf.Exp(-consts._z * consts._w * deltaTime);
                float alpha = 2 * t1 * (consts._z <= 1 ? Mathf.Cos(deltaTime * consts._d) : (float)Math.Cosh(deltaTime * consts._d));
                float beta = t1 * t1;
                float t2 = deltaTime / (1 * beta - alpha);
                k2Stable = deltaTime * t2;
            }

            currentPosition = currentPosition + deltaTime * currentVelocity; //integrate position by velocity
            currentVelocity = currentVelocity + deltaTime * (targetPosition + consts.k3 * deltaPosition - currentPosition - consts.k1 * currentVelocity) / k2Stable; //integrate velocity by acceleration
            return (currentPosition, currentVelocity);
        }

        
        public void DrawInspector(Material material, Rect clipRect, Rect frameSize)
        {
            const int BORDER = 3;

            GUI.BeginClip(clipRect);
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            material.SetPass(0);

            GLDraw.Rect(0, 0, frameSize.width, frameSize.yMax, Color.black);
            GLDraw.EmptyRect(0, 0, frameSize.width, frameSize.yMax, BORDER, Color.gray);

            GLDraw.Line(BORDER, 100, frameSize.width - BORDER, 100, new Color(0.7f, 0.7f, 0.7f));

            GLDraw.Lines(new Color(0.7f, 0.7f, 0), new Vector2(BORDER, 150), new Vector2(100, 150),
                new Vector2(100, 50), new Vector2(frameSize.width, 50));
            GLDraw.Lines(new Color(0.7f, 0.7f, 0), new Vector2(BORDER, 150 + 1), new Vector2(100 + 1, 150 + 1),
                new Vector2(100 + 1, 50 + 1), new Vector2(frameSize.width, 50 + 1));

            for (int i = 0; i < frameSize.width; i += 5)
            {
                
                
            }


            GL.PopMatrix();
            GUI.EndClip();
        }
    }
}