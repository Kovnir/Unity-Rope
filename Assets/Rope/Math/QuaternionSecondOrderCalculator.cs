using UnityEngine;

namespace Kovnir.Rope.Math
{
    public sealed class QuaternionSecondOrderCalculator : SecondOrderCalculator<Quaternion>
    {
        public struct QuaternionData : IData<Quaternion>
        {
            private Quaternion value;

            public QuaternionData(Quaternion value)
            {
                this.value = value;
            }

            public IData<Quaternion> Default => new QuaternionData(Quaternion.identity);

            IData<Quaternion> IData<Quaternion>.Add(IData<Quaternion> other)
            {
                return new QuaternionData(value * other.GetData());
            }

            IData<Quaternion> IData<Quaternion>.Sub(IData<Quaternion> other)
            {
                return new QuaternionData(value * Quaternion.Inverse(other.GetData()));
            }

            IData<Quaternion> IData<Quaternion>.Mul(float other)
            {
                return new QuaternionData(Quaternion.Slerp(Quaternion.identity, value, other));
            }

            IData<Quaternion> IData<Quaternion>.Div(float other)
            {
                return new QuaternionData(Quaternion.Slerp(Quaternion.identity, value, 1.0f / other));
            }

            public Quaternion GetData() => value;
        }

        public QuaternionSecondOrderCalculator(Quaternion initialRotation, SecondOrderCalculatorParams @params)
            : base(new QuaternionData(initialRotation), @params)
        {
        }

        public override Quaternion Update(float deltaTime, Quaternion targetRotation)
        {
            return base.Update(deltaTime, new QuaternionData(targetRotation)).GetData();
        }
    }
}