using UnityEngine;

namespace Kovnir.Rope.Math
{
    public sealed class Vector3SecondOrderCalculator : SecondOrderCalculator<Vector3>
    {
        public struct Vector3Data : IData<Vector3>
        {
            private Vector3 value;

            public Vector3Data(Vector3 value)
            {
                this.value = value;
            }

            public Vector3 ToVector3() => value;

            IData<Vector3> IData<Vector3>.Add(IData<Vector3> other)
            {
                return new Vector3Data(value + other.GetData());
            }

            IData<Vector3> IData<Vector3>.Sub(IData<Vector3> other)
            {
                return new Vector3Data(value - other.GetData());
            }

            IData<Vector3> IData<Vector3>.Mul(float other)
            {
                return new Vector3Data(value * other);
            }

            IData<Vector3> IData<Vector3>.Div(float other)
            {
                return new Vector3Data(value / other);
            }

            IData<Vector3> IData<Vector3>.Add(Vector3 other)
            {
                return new Vector3Data(value + other);
            }

            IData<Vector3> IData<Vector3>.Sub(Vector3 other)
            {
                return new Vector3Data(value - other);
            }

            public Vector3 GetData() => value;
        }

        public Vector3SecondOrderCalculator(Vector3 initialPosition, SecondOrderCalculatorParams @params)
            : base(new Vector3Data(initialPosition), @params)
        {
        }

        public override Vector3 Update(float deltaTime, Vector3 targetPosition)
        {
            return base.Update(deltaTime, new Vector3Data(targetPosition)).GetData();
        }
    }
}