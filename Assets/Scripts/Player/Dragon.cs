using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Dragon : MonoBehaviour
    {
        [SerializeField] private float minAirLevel;
        [SerializeField] private float maxAirLevel;
        [SerializeField] private float breatheSpeed;
        [SerializeField] private float airLevel;
    
        // Gets called from Player Input Component
        public void OnBreathe(InputAction.CallbackContext context)
        {
            
        }

        private void Update()
        {
            
        }
    }
}
