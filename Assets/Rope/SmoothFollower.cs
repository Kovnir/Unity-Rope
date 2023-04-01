using Kovnir.Rope.Math;
using UnityEngine;

namespace Rope
{
    public sealed class SmoothFollower : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private SecondOrderCalculatorParams dynamicsParams;

        Vector3SecondOrderCalculator positionCalculator;
        Vector3SecondOrderCalculator rotationCalculator;

        void Awake()
        {
            InitDynamics();
        }

        void InitDynamics()
        {
            positionCalculator = new Vector3SecondOrderCalculator(target.position, dynamicsParams);
            rotationCalculator = new Vector3SecondOrderCalculator(target.forward, dynamicsParams);
        }

        void Update()
        {
            transform.position = positionCalculator.Update(Time.deltaTime, target.position);
            transform.forward = rotationCalculator.Update(Time.deltaTime, target.forward);
        }
    }
}