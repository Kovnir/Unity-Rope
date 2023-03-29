using System;
using UnityEngine;

namespace DefaultNamespace
{
    //inspired by (stolen from) https://www.youtube.com/watch?v=KPoeNZZ6H4s
    public class SecondOrderDynamics
    {
        [Serializable]
        public struct Params
        {
            public float f;
            public float z;
            public float r;
        }
        
        private Vector3 xp; //previous position
        private Vector3 y;  //current position
        private Vector3 yd; //current velocity
        
        private float _w, _z, _d, k1, k2, k3;
        
        public SecondOrderDynamics(Vector3 initialPosition, Params @params)
        {
            y = initialPosition;
            xp = initialPosition;
            yd = Vector3.zero;
            
            float f = @params.f;
            float z = @params.z;
            float r = @params.r;
            
            _w = 2 * Mathf.PI * f;
            _z = z;
            _d = _w * Mathf.Sqrt(Mathf.Abs(_z * _z - 1));
            k1 = z / (Mathf.PI * f);
            k2 = 1/ (_w * _w);
            k3 = r * z / _w;
        }

        public Vector3 Update(float T, Vector3 x, Vector3? xd = null)
        {
            if (xd == null)
            {
                xd = x - xp; //estimate velocity
                xp = x;
            }
            float k1Stable, k2Stable;

            if (_w * T < _z)  //clamp k2 to avoid instability with jitters
            {
                k1Stable = k1;
                k2Stable = Mathf.Max(Mathf.Max(k2, T*T/2 + T*k1/2), T*k1);
            }
            else
            {
                float t1 = Mathf.Exp(-_z * _w * T);
                float alpha = 2* t1 * (_z <= 1 ? Mathf.Cos(T*_d) : (float) Math.Cosh(T * _d));
                float beta = t1 * t1;
                float t2 = T / (1*beta - alpha);
                k1Stable = (1 - beta) * t2;
                k2Stable = T * t2;
            }

            y = y + T * yd;//integrate position by velocity
            yd = yd + T * (x + k3 * xd.Value - y - k1 * yd) / k2Stable;//integrate velocity by acceleration
            return y;
        }
    }
}