using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Dragon : MonoBehaviour
    {
        [Header("BreatheValues")]
        [SerializeField] private float breatheSpeed;
        [SerializeField] private float airLevel;
        [SerializeField] private float projectileCost;
        [SerializeField] private float minBreatheScaleFactor;
        [SerializeField] private float maxBreatheScaleFactor;

        [Header("FireValues")]
        [SerializeField] private float fireCoolDownTime;
        [SerializeField] private Transform mouthPosition;

        [Header("References")]
        [SerializeField] private Material bodyMaterial;
        [SerializeField] private Vaccum vaccum;
        
        private float m_fireCooldown;
        private Stack<Projectile.Projectile> m_storedProjectiles;
        private int m_maxProjectiles = 5;

        private float m_breatheAmount;
        private Vector3 m_minBreatheSize;
        private Vector3 m_maxBreatheSize;
        private float m_maxAirLevel;

        public bool IncreaseHealth(int amount = 1)
        {
            var health = GetComponent<PlayerHealth>();
            return health != null && health.IncreaseHealth(amount);
        }
        
        private void Awake()
        {
            m_storedProjectiles = new Stack<Projectile.Projectile>();
            m_minBreatheSize = Vector3.one * minBreatheScaleFactor;
            m_maxBreatheSize = Vector3.one * maxBreatheScaleFactor;
            transform.localScale = m_minBreatheSize;
            m_maxAirLevel = m_maxProjectiles * projectileCost;
        }

        // Gets called from Player Input Component
        #region InputEventMethods

        public void OnBreathe(InputAction.CallbackContext context)
        {
            vaccum.gameObject.SetActive(context.ReadValueAsButton());
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
                bodyMaterial.color = m_storedProjectiles.Peek().GetComponent<Projectile.Projectile>().DragonColor;
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
            
            airLevel = Mathf.Min(airLevel + m_breatheAmount * Time.deltaTime, m_maxAirLevel);
            var fillAmount = airLevel / m_maxAirLevel;
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
            var fillAmount = airLevel > 0 ? airLevel / m_maxAirLevel : 0;
            transform.localScale = Vector3.Lerp(m_minBreatheSize, m_maxBreatheSize, fillAmount);
        }

        private void AddProjectile()
        {
            var newProjectile = GasArea.GetProjectileFromArea(transform.position.x, transform.position.z);
            bodyMaterial.color = newProjectile.GetComponent<Projectile.Projectile>().DragonColor;
            m_storedProjectiles.Push(newProjectile);
        }

        private void OnApplicationQuit()
        {
            bodyMaterial.color = new Color(0.3146138f, 0.6603774f, 0.378644f);
        }
    }
}
