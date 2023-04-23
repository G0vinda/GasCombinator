using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private Material primaryMaterial;
        private Color defaultPrimaryColor;
        [SerializeField] private Material secondaryMaterial;
        private Color defaultSecondaryColor;
        [SerializeField] private Vaccum vaccum;
        private Dictionary<int, DragonAttributes> m_fireballTypeAttributes;


        private float m_fireCooldown;
        private Stack<Projectile.Projectile> m_storedProjectiles;
        private int m_maxProjectiles = 5;
        private WaitForSeconds m_multipleShotDelay;

        private float m_breatheAmount;
        private Vector3 m_minBreatheSize;
        private Vector3 m_maxBreatheSize;
        private float m_maxAirLevel;
        
        private PlayerController m_playerController;
        private PlayerHealth m_playerHealth;
        private int m_beanCount;

        private void Awake()
        {
            m_storedProjectiles = new Stack<Projectile.Projectile>();
            m_multipleShotDelay = new WaitForSeconds(multipleShotDelayTime);
            m_fireballTypeAttributes = new Dictionary<int, DragonAttributes>();
            m_fireballTypeAttributes[(int) Bean.Bean.Type.NEUTRAL] = new DragonAttributes();
            m_minBreatheSize = Vector3.one * minBreatheScaleFactor;
            m_maxBreatheSize = Vector3.one * maxBreatheScaleFactor;
            transform.localScale = m_minBreatheSize;
            m_maxAirLevel = m_maxProjectiles * projectileCost;
            
            m_playerController = GetComponent<PlayerController>();
            m_playerHealth = GetComponent<PlayerHealth>();

            defaultPrimaryColor = primaryMaterial.color;
            defaultSecondaryColor = secondaryMaterial.color;
        }
        
        public bool IncreaseHealth(int amount = 1)
        {
            var health = GetComponent<PlayerHealth>();
            return health != null && health.IncreaseHealth(amount);
        }

        public void RemoveBean(Bean.Bean bean)
        {
            vaccum.RemoveBean(bean);
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

            var projectile = m_storedProjectiles.Peek();
            if (!m_fireballTypeAttributes.ContainsKey((int) projectile.type))
            {
                m_fireballTypeAttributes[(int) projectile.type] = new DragonAttributes();
            }
            
            StartCoroutine(Shoot(1 
                                 + m_fireballTypeAttributes[(int)Bean.Bean.Type.NEUTRAL].ExtraShots 
                                 + m_fireballTypeAttributes[(int)projectile.type].ExtraShots));
            BreatheOut(projectileCost);
            m_fireCooldown = fireCoolDownTime;
        }

        public void OnFart(InputAction.CallbackContext context)
        {
            if(!context.performed || m_beanCount == 0)
                return;
            
            m_fireballTypeAttributes.Clear();
            m_playerController.BonusSpeed = 0;
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
            primaryMaterial.color = m_storedProjectiles.Count > 0 ? secondaryMaterial.color : defaultPrimaryColor;
            secondaryMaterial.color = m_storedProjectiles.Count > 1
                ? (m_storedProjectiles.ToArray()[1]).GetComponent<Projectile.Projectile>().DragonColor * 1.3f
                : defaultSecondaryColor;
            for (var i = 0; i < numberOfProjectiles; i++)
            {
                var newProjectile = Instantiate( projectile, mouthPosition.position, mouthPosition.rotation);
                newProjectile.Init(gameObject);
                newProjectile.slowEffect += m_fireballTypeAttributes[(int) newProjectile.type].ShotSlowEffect;
                newProjectile.Damage *= 1 + m_fireballTypeAttributes[(int) newProjectile.type].DamageMultiplier;
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
            primaryMaterial.color = newProjectile.GetComponent<Projectile.Projectile>().DragonColor;
            if (m_storedProjectiles.Count >= 1)
            {
                secondaryMaterial.color = m_storedProjectiles.Peek().GetComponent<Projectile.Projectile>().DragonColor * 1.3f;
            }
            m_storedProjectiles.Push(newProjectile);
        }

        private void OnApplicationQuit()
        {
            primaryMaterial.color = defaultPrimaryColor;
            secondaryMaterial.color = defaultSecondaryColor;
        }

        public void ConsumeBean(Bean.Bean bean)
        {
            if (!m_fireballTypeAttributes.ContainsKey((int) bean.BeanType))
            {
                m_fireballTypeAttributes[(int) bean.BeanType] = new DragonAttributes();
            }

            switch (bean.BeanEfect)
            {
                case Bean.Bean.Effect.EXTRA_SHOTS:
                    m_fireballTypeAttributes[(int) bean.BeanType].IncreaseExtraShots();
                    break;
                case Bean.Bean.Effect.SHOT_DAMAGE:
                    m_fireballTypeAttributes[(int) bean.BeanType].IncreaseDamageMultiplier(bean.ShotDamageMultiplier);
                    break;
                case Bean.Bean.Effect.BONUS_WALKING_SPEED:
                    //m_fireballTypeAttributes[(int) bean.BeanType].IncreaseWalkSpeedBonus(bean.WalkingSpeedBonus);
                    m_playerController.BonusSpeed += bean.WalkingSpeedBonus;
                    break;
                case Bean.Bean.Effect.SHOT_SLOW:
                    m_fireballTypeAttributes[(int) bean.BeanType].IncreaseShotSlow(bean.ShotSlow);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            m_beanCount++;
            vaccum.RemoveBean(bean);
            Destroy(bean.gameObject);
        }

        private class DragonAttributes
        {
            public int ExtraShots;
            public float DamageMultiplier;
            public float WalkSpeedBonus;
            public float ShotSlowEffect;
            public int ShotSpreadAmount;

            public DragonAttributes()
            {
                ToDefault();
            }
            
            public void IncreaseExtraShots(int amount = 1)
            {
                ExtraShots += amount;
            }

            public void IncreaseDamageMultiplier(float multiplier)
            {
                DamageMultiplier *= multiplier;
            } 
            
             public void IncreaseShotSlow(float amount)
            {
                ShotSlowEffect += amount;
            }
            
            public void IncreaseWalkSpeedBonus(float amount)
            {
                WalkSpeedBonus += amount;
            }

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
