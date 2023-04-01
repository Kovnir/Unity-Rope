using System;

namespace Kovnir.Rope.Math
{
    [Serializable]
    public struct SecondOrderCalculatorParams
    {
        public float Frequency;
        public float Damping;
        public float Response;

        public SecondOrderCalculatorParams(float frequency, float damping, float response)
        {
            Frequency = frequency;
            Damping = damping;
            Response = response;
        }
    }
}