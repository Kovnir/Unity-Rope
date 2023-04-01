using Kovnir.Rope.Math;
using UnityEngine;
using Quaternion = System.Numerics.Quaternion;

namespace Rope
{
    public sealed class SmoothFollower : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private SecondOrderCalculatorParams dynamicsParams;

        Vector3SecondOrderCalculator calculator;
        Vector3SecondOrderCalculator calculator1;

        void Awake()
        {
            InitDynamics();
        }

        void InitDynamics()
        {
            calculator = new Vector3SecondOrderCalculator(target.position, dynamicsParams);
            calculator1 = new Vector3SecondOrderCalculator(target.rotation.eulerAngles, dynamicsParams);
        }

        void Update()
        {
            transform.position = calculator.Update(Time.deltaTime, target.position);
            transform.rotation =
                UnityEngine.Quaternion.Euler(calculator1.Update(Time.deltaTime, target.rotation.eulerAngles));
        }
    }
}