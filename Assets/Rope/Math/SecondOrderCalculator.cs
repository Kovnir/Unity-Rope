using System;
using UnityEngine;

namespace Kovnir.Rope.Math
{
    //inspired by (stolen from) https://www.youtube.com/watch?v=KPoeNZZ6H4s
    public sealed class SecondOrderCalculator
    {
        [Serializable]
        public struct Params
        {
            public float Frequency;
            public float Damping;
            public float Response;

            public Params(float frequency, float damping, float response)
            {
                Frequency = frequency;
                Damping = damping;
                Response = response;
            }
        }

        public struct Consts
        {
            public float _w;
            public float _z;
            public float _d;
            public float k1;
            public float k2;
            public float k3;

            public static Consts Create(Params @params)
            {
                float f = @params.Frequency;
                float z = @params.Damping;
                float r = @params.Response;

                float _w = 2 * Mathf.PI * f;
                float _z = z;
                float _d = _w * Mathf.Sqrt(Mathf.Abs(_z * _z - 1));
                float k1 = z / (Mathf.PI * f);
                float k2 = 1 / (_w * _w);
                float k3 = r * z / _w;

                return new Consts
                {
                    _w = _w,
                    _z = _z,
                    _d = _d,
                    k1 = k1,
                    k2 = k2,
                    k3 = k3
                };
            }
        }

        private Vector3 previousTargetPosition;
        private Vector3 currentPosition;
        private Vector3 currentVelocity;
        private Consts consts;

        public SecondOrderCalculator(Vector3 initialPosition, Params @params)
        {
            currentPosition = initialPosition;
            previousTargetPosition = initialPosition;
            currentVelocity = Vector3.zero;

            consts = Consts.Create(@params);
        }

        public Vector3 Update(float deltaTime, Vector3 targetPosition)
        {
            (currentPosition, currentVelocity) = Update(deltaTime, targetPosition, previousTargetPosition,
                currentPosition, currentVelocity, consts);
            previousTargetPosition = targetPosition;
            return currentPosition;
        }


        private static (Vector3 currentPosition, Vector3 currentVelocity) Update(
            float deltaTime,
            Vector3 targetPosition, Vector3 previousTargetPosition,
            Vector3 currentPosition, Vector3 currentVelocity,
            Consts consts)
        {
            Vector3 targetVelocity = (targetPosition - previousTargetPosition) / deltaTime; //estimate velocity

            float k1Stable;
            float k2Stable;

            if (consts._w * deltaTime < consts._z) //clamp k2 to avoid instability with jitters
            {
                k1Stable = consts.k1;
                k2Stable = Mathf.Max(Mathf.Max(consts.k2, deltaTime * deltaTime / 2 + deltaTime * consts.k1 / 2),
                    deltaTime * consts.k1);
            }
            else
            {
                float t1 = Mathf.Exp(-consts._z * consts._w * deltaTime);
                float alpha = 2 * t1 * (consts._z <= 1
                    ? Mathf.Cos(deltaTime * consts._d)
                    : (float)System.Math.Cosh(deltaTime * consts._d));
                float beta = t1 * t1;
                float t2 = deltaTime / (1 + beta - alpha);
                k1Stable = (1 - beta) * t2;
                k2Stable = deltaTime * t2;
            }

            currentPosition += deltaTime * currentVelocity; //integrate position by velocity
            currentVelocity += deltaTime *
                               (targetPosition + consts.k3 * targetVelocity - currentPosition -
                                k1Stable * currentVelocity) /
                               k2Stable; //integrate velocity by acceleration
            return (currentPosition, currentVelocity);
        }
    }
}