using Kovnir.Rope.Math;
using UnityEngine;

namespace Rope
{
    public sealed class SmoothFollower : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private SecondOrderCalculator.Params dynamicsParams;
        
        SecondOrderCalculator calculator;

        void Awake()
        {
            InitDynamics();
        }
        
        void InitDynamics()
        {
            calculator = new SecondOrderCalculator(target.position, dynamicsParams);
        }

        void Update()
        {
            transform.position = calculator.Update(Time.deltaTime, target.position);
        }
    }
}
