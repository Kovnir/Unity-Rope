using UnityEngine;

namespace Kovnir.Rope.Math
{
    //inspired by (stolen from) https://www.youtube.com/watch?v=KPoeNZZ6H4s
    public abstract class SecondOrderCalculator<T> where T : struct
    {
        public interface IData<T> where T : struct
        {
            public static IData<T> operator +(IData<T> a, IData<T> b) => a.Add(b);
            public static IData<T> operator -(IData<T> a, IData<T> b) => a.Sub(b);
            public static IData<T> operator *(IData<T> a, float b) => a.Mul(b);
            public static IData<T> operator *(float b, IData<T> a) => a.Mul(b);
            public static IData<T> operator /(IData<T> a, float b) => a.Div(b);
            public static IData<T> operator +(IData<T> a, Vector3 b) => a.Add(b);
            public static IData<T> operator +(Vector3 b, IData<T> a) => a.Add(b);
            public static IData<T> operator -(IData<T> a, Vector3 b) => a.Sub(b);
            
            public abstract Vector3 ToVector3();
            protected abstract IData<T> Add(IData<T> other);
            protected abstract IData<T> Sub(IData<T> other);
            protected abstract IData<T> Mul(float other);
            protected abstract IData<T> Div(float other);
            protected abstract IData<T> Add(Vector3 other);
            protected abstract IData<T> Sub(Vector3 other);
            
            public abstract T GetData();
        }

        public struct Consts
        {
            public float _w;
            public float _z;
            public float _d;
            public float k1;
            public float k2;
            public float k3;

            public static Consts Create(SecondOrderCalculatorParams @params)
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

        private IData<T> previousTarget;
        private IData<T> current;
        private Vector3 currentVelocity;
        
        private Consts consts;

        public SecondOrderCalculator(IData<T> initial, SecondOrderCalculatorParams @params)
        {
            current = initial;
            previousTarget = initial;
            currentVelocity = Vector3.zero;

            consts = Consts.Create(@params);
        }

        public abstract T Update(float deltaTime, T targetPosition);

        protected IData<T> Update(float deltaTime, IData<T> targetPosition)
        {
            (current, currentVelocity) = Update(deltaTime, targetPosition, previousTarget,
                current, currentVelocity, consts);
            previousTarget = targetPosition;
            return current;
        }


        private static (IData<T> currentPosition, Vector3 currentVelocity) Update(
            float deltaTime,
            IData<T> targetPosition, IData<T> previousTargetPosition,
            IData<T> currentPosition, Vector3 currentVelocity,
            Consts consts)
        {
            IData<T> targetVelocity = (targetPosition - previousTargetPosition) / deltaTime; //estimate velocity

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
            IData<T> velocity = currentVelocity + deltaTime *
                               (targetPosition + consts.k3 * targetVelocity - currentPosition -
                                k1Stable * currentVelocity) /
                               k2Stable; //integrate velocity by acceleration
            currentVelocity = velocity.ToVector3();
            return (currentPosition, currentVelocity);
        }
    }
}