using Kovnir.Rope.Math;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

namespace Rope
{
    public sealed class SmoothFollower : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private SecondOrderCalculatorParams dynamicsParams;

        Vector3SecondOrderCalculator positionCalculator;
        QuaternionSecondOrderCalculator rotationCalculator;

        void Awake()
        {
            InitDynamics();
        }

        void InitDynamics()
        {
            positionCalculator = new Vector3SecondOrderCalculator(target.position, dynamicsParams);
            rotationCalculator = new QuaternionSecondOrderCalculator(target.rotation, dynamicsParams);
        }

        void Update()
        {
            transform.position = positionCalculator.Update(Time.deltaTime, target.position);
            transform.rotation = rotationCalculator.Update(Time.deltaTime, target.rotation);
        }
    }
}