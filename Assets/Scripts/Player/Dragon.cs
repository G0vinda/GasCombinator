using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        [SerializeField] private float multipleShotDelayTime;

        [Header("References")]
        [SerializeField] private Material bodyMaterial;
        [SerializeField] private Vaccum vaccum;
        
        private float m_fireCooldown;
        private Stack<Projectile.Projectile> m_storedProjectiles;
        private int m_maxProjectiles = 5;
        private WaitForSeconds m_multipleShotDelay;

        private float m_breatheAmount;
        private Vector3 m_minBreatheSize;
        private Vector3 m_maxBreatheSize;
        private float m_maxAirLevel;

        private DragonAttributes m_dragonAttributes;
        private PlayerController m_playerController;
        private PlayerHealth m_playerHealth;
        private int m_beanCount;

        private void Awake()
        {
            m_storedProjectiles = new Stack<Projectile.Projectile>();
            m_multipleShotDelay = new WaitForSeconds(multipleShotDelayTime);
            
            m_minBreatheSize = Vector3.one * minBreatheScaleFactor;
            m_maxBreatheSize = Vector3.one * maxBreatheScaleFactor;
            transform.localScale = m_minBreatheSize;
            m_maxAirLevel = m_maxProjectiles * projectileCost;
            
            m_dragonAttributes.ToDefault();
            m_playerController = GetComponent<PlayerController>();
            m_playerHealth = GetComponent<PlayerHealth>();
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

            if(m_fireCooldown > 0 || m_storedProjectiles.Count == 0)
                return;

            StartCoroutine(Shoot(1 + m_dragonAttributes.ExtraShots));
            if (m_storedProjectiles.Count > 0)
            {
                bodyMaterial.color = m_storedProjectiles.Peek().GetComponent<Projectile.Projectile>().DragonColor;
            } 
            else
            {
                bodyMaterial.color = new Color(0.3146138f, 0.6603774f, 0.378644f);
            }
            
            BreatheOut(projectileCost);
            m_fireCooldown = fireCoolDownTime;
        }

        public void OnFart(InputAction.CallbackContext context)
        {
            if(!context.performed || m_beanCount == 0)
                return;
            
            m_dragonAttributes.ToDefault();
            m_playerController.bonusSpeed = m_dragonAttributes.WalkSpeedBonus;
            m_playerHealth.IncreaseHealth(m_beanCount);
            m_beanCount = 0;
        }

        #endregion

        private void Update()
        {
            BreatheIn();
            m_fireCooldown = Mathf.Max(m_fireCooldown - Time.deltaTime, 0);
        }

        private IEnumerator Shoot(int numberOfProjectiles)
        {
            var projectile = m_storedProjectiles.Pop();
            for (var i = 0; i < numberOfProjectiles; i++)
            {
                var newProjectile = Instantiate( projectile, mouthPosition.position, mouthPosition.rotation);
                newProjectile.slowEffect = m_dragonAttributes.ShotSlowEffect;
                yield return m_multipleShotDelay;
            }
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

        public void ConsumeBean(Bean.Bean bean)
        {
            switch (bean.BeanType)
            {
                case Bean.Bean.Type.EXTRA_SHOTS:
                    m_dragonAttributes.ExtraShots++;
                    break;
                case Bean.Bean.Type.BONUS_WALKING_SPEED:
                    m_dragonAttributes.WalkSpeedBonus += bean.WalkingSpeedBonus;
                    m_playerController.bonusSpeed = m_dragonAttributes.WalkSpeedBonus;
                    break;
                case Bean.Bean.Type.SHOT_SLOW:
                    m_dragonAttributes.ShotSlowEffect += bean.ShotSlow;
                    break;
                case Bean.Bean.Type.SHOT_SPREAD:
                    m_dragonAttributes.ShotSpreadAmount++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            m_beanCount++;
            Destroy(bean.gameObject);
        }

        private struct DragonAttributes
        {
            public int ExtraShots;
            public float WalkSpeedBonus;
            public float ShotSlowEffect;
            public int ShotSpreadAmount;

            public void ToDefault()
            {
                ExtraShots = 0;
                WalkSpeedBonus = 0;
                ShotSlowEffect = 0;
                ShotSpreadAmount = 0;
            }
        }
    }
}
