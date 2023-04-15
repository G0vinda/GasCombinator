using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Dragon : MonoBehaviour
    {
        [Header("BreatheValues")]
        [SerializeField] private float maxAirLevel;
        [SerializeField] private float breatheSpeed;
        [SerializeField] private float airLevel;
        [SerializeField] private float minBreatheScaleFactor;
        [SerializeField] private float maxBreatheScaleFactor;

        [Header("FireValues")]
        [SerializeField] private float fireCoolDownTime;
        [SerializeField] private float fireBallAirCost;
        [SerializeField] private GameObject fireBallPrefab;
        [SerializeField] private Transform mouthPosition;

        private float _breatheAmount;
        private float _fireCooldown;

        private Vector3 _minBreatheSize;
        private Vector3 _maxBreatheSize;

        private void Awake()
        {
            _minBreatheSize = Vector3.one * minBreatheScaleFactor;
            _maxBreatheSize = Vector3.one * maxBreatheScaleFactor;
            transform.localScale = _minBreatheSize;
        }

        // Gets called from Player Input Component
        #region InputEventMethods

        public void OnBreathe(InputAction.CallbackContext context)
        {
            _breatheAmount = context.ReadValueAsButton() ? breatheSpeed : 0;
            if(Physics.Raycast(transform.position, Vector3.down, out var hit))
                Debug.Log("RaycastHitFloor: " + hit.collider.gameObject.name);
        }

        public void OnSpitFire(InputAction.CallbackContext context)
        {
            if(!context.ReadValueAsButton())
                return;
            
            if(_fireCooldown > 0 || airLevel < fireBallAirCost)
                return;
            
            Instantiate(fireBallPrefab, mouthPosition.position, mouthPosition.rotation);
            BreatheOut(fireBallAirCost);
            _fireCooldown = fireCoolDownTime;
        }

        #endregion
        

        private void Update()
        {
            BreatheIn();
            _fireCooldown = Mathf.Max(_fireCooldown - Time.deltaTime, 0);
        }

        private void BreatheIn()
        {
            if(_breatheAmount == 0)
                return;
            
            airLevel = Mathf.Min(airLevel + _breatheAmount * Time.deltaTime, maxAirLevel);
            var fillAmount = airLevel / maxAirLevel;
            transform.localScale = Vector3.Lerp(_minBreatheSize, _maxBreatheSize, fillAmount);
        }

        private void BreatheOut(float amount)
        {
            airLevel = Mathf.Max(airLevel - amount, 0);
            var fillAmount = airLevel > 0 ? airLevel / maxAirLevel : 0;
            transform.localScale = Vector3.Lerp(_minBreatheSize, _maxBreatheSize, fillAmount);
        }
    }
}
