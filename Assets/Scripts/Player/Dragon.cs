using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class Dragon : MonoBehaviour
    {
        [Header("BreatheValues")]
        [SerializeField] private float breatheSpeed;
        [SerializeField] private float airLevel;
        [SerializeField] private float projectileCost = 20.0f;
        private float maxAirLevel;
        [SerializeField] private float minBreatheScaleFactor;
        [SerializeField] private float maxBreatheScaleFactor;

        [Header("FireValues")]
        [SerializeField] private float fireCoolDownTime;
        [SerializeField] private Transform mouthPosition;
        private Stack<GameObject> m_storedProjectiles;
        private int m_maxProjectiles = 5;

        [Header("References")]
        [SerializeField] private Gases gasesAreas;
        [SerializeField] private Material bodyMaterial;
        
        private float m_breatheAmount;
        private float m_fireCooldown;

        private Vector3 m_minBreatheSize;
        private Vector3 m_maxBreatheSize;

        private void Awake()
        {
            m_storedProjectiles = new Stack<GameObject>();
            m_minBreatheSize = Vector3.one * minBreatheScaleFactor;
            m_maxBreatheSize = Vector3.one * maxBreatheScaleFactor;
            transform.localScale = m_minBreatheSize;
            maxAirLevel = m_maxProjectiles * projectileCost;
        }

        // Gets called from Player Input Component
        #region InputEventMethods

        public void OnBreathe(InputAction.CallbackContext context)
        {
            m_breatheAmount = context.ReadValueAsButton() ? breatheSpeed : 0;
        }

        public void OnSpitFire(InputAction.CallbackContext context)
        {
            if(!context.ReadValueAsButton())
                return;
            var test = m_storedProjectiles;
            int a = m_storedProjectiles.Count;
            if(m_fireCooldown > 0 || m_storedProjectiles.Count == 0)
                return;
            Instantiate( m_storedProjectiles.Pop(), mouthPosition.position, mouthPosition.rotation);
            if (m_storedProjectiles.Count > 0)
            {
                bodyMaterial.color = m_storedProjectiles.Peek().GetComponent<Projectile>().DragonColor;
            } 
            else
            {
                bodyMaterial.color = bodyMaterial.color = new Color(0.3146138f, 0.6603774f, 0.378644f);
            }
            BreatheOut(projectileCost);
            m_fireCooldown = fireCoolDownTime;
        }

        #endregion
        

        private void Update()
        {
            BreatheIn();
            m_fireCooldown = Mathf.Max(m_fireCooldown - Time.deltaTime, 0);
        }

        private void BreatheIn()
        {
            if(m_breatheAmount == 0)
                return;
            
            airLevel = Mathf.Min(airLevel + m_breatheAmount * Time.deltaTime, maxAirLevel);
            var fillAmount = airLevel / maxAirLevel;
            float projectiles = (m_storedProjectiles.Count + 1.0f) / m_maxProjectiles;
            if (fillAmount >= projectiles)
            {
                Debug.Log(fillAmount + " >= " + projectiles + ": adding Projectile!");
                AddProjectile();
            }

            transform.localScale = Vector3.Lerp(m_minBreatheSize, m_maxBreatheSize, fillAmount);
        }

        private void BreatheOut(float amount)
        {
            airLevel = Mathf.Max(airLevel - amount, 0);
            var fillAmount = airLevel > 0 ? airLevel / maxAirLevel : 0;
            transform.localScale = Vector3.Lerp(m_minBreatheSize, m_maxBreatheSize, fillAmount);
        }

        private void AddProjectile()
        {
            var newProjectile = gasesAreas.GetProjectile(transform.position.x, transform.position.z);
            bodyMaterial.color = newProjectile.GetComponent<Projectile>().DragonColor;
            m_storedProjectiles.Push(newProjectile);
        }

        private void OnApplicationQuit()
        {
            bodyMaterial.color = new Color(0.3146138f, 0.6603774f, 0.378644f);
        }
    }
}
