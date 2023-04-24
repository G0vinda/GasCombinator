using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Bean;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

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
        public float maximumShotSpread;
        [SerializeField] private float fireCoolDownTime;
        [SerializeField] private Transform mouthPosition;
        [SerializeField] private float multipleShotDelayTime;

        [Header("References")]
        [SerializeField] private Material primaryMaterial;
        private Color defaultPrimaryColor;
        [SerializeField] private Material secondaryMaterial;
        private Color defaultSecondaryColor;
        [SerializeField] private Vaccum vaccum;
        [SerializeField] private BeanSpawner beanSpawner;
        
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
        
        // Beansw
        private int m_collectedBeansCount;
        private DragonAttributes m_dragonAttributes;

        public static event Action<List<KeyValuePair<int, string>>> BeansChanged;

        [HideInInspector] public List<KeyValuePair<int, string>> CollectedBeanInfo;

        private void Awake()
        {
            m_storedProjectiles = new Stack<Projectile.Projectile>();
            m_multipleShotDelay = new WaitForSeconds(multipleShotDelayTime);
            m_dragonAttributes = new DragonAttributes();
            CollectedBeanInfo = new List<KeyValuePair<int, string>>();
            /*m_fireballTypeAttributes = new Dictionary<int, ShotAttributes>();
            m_fireballTypeAttributes[(int) Bean.Bean.Type.NEUTRAL] = new ShotAttributes();*/
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

            if(m_fireCooldown > 0 || m_storedProjectiles.Count == 0 ||  vaccum.gameObject.activeSelf)
                return;

            var projectile = m_storedProjectiles.Peek();
            /*if (!m_fireballTypeAttributes.ContainsKey((int) projectile.type))
            {
                m_fireballTypeAttributes[(int) projectile.type] = new ShotAttributes();
            }*/
            
            Shoot();
            BreatheOut(projectileCost);
            m_fireCooldown = fireCoolDownTime;
        }

        public void OnFart(InputAction.CallbackContext context)
        {
            if(!context.performed || m_collectedBeansCount == 0)
                return;
            
           Fart();
        }

        private void Fart()
        {
            /*m_fireballTypeAttributes.Clear();*/
            m_playerController.BonusSpeed = 0;
            m_playerHealth.IncreaseHealth(m_collectedBeansCount * (
                                          BlueBean.Attributes.ActivatedEffects.Contains(BlueBean.Effect.DoubleFartHeal) ? 2 : 1));
            m_collectedBeansCount = 0;

            if (RedBean.Attributes.ActivatedEffects.Contains(RedBean.Effect.KillFart))
            {
                var colliders = Physics.OverlapSphere(transform.position, RedBean.Attributes.KillFartRadius);
                foreach (var hitCollider in colliders)
                {
                    if (hitCollider.TryGetComponent<Enemy.Enemy>(out Enemy.Enemy enemy))
                    {
                        enemy.TakeDamage(Single.PositiveInfinity);
                    }
                }
            }
            else if (BlueBean.Attributes.ActivatedEffects.Contains(BlueBean.Effect.FreezeFart))
            {
                var colliders = Physics.OverlapSphere(transform.position, Single.PositiveInfinity);
                foreach (var hitCollider in colliders)
                {
                    if (hitCollider.TryGetComponent<Enemy.Enemy>(out Enemy.Enemy enemy))
                    {
                        enemy.Freeze(BlueBean.Attributes.FartFreezeTime, 
                            BlueBean.Attributes.PermanentSlowMultiplierPerBean * BlueBean.Attributes.Collected);
                    }
                }
            }
            else if (GreenBean.Attributes.ActivatedEffects.Contains(GreenBean.Effect.PoisonFart))
            {
                var colliders = Physics.OverlapSphere(transform.position, Single.PositiveInfinity);
                foreach (var hitCollider in colliders)
                {
                    if (hitCollider.TryGetComponent<Enemy.Enemy>(out Enemy.Enemy enemy))
                    {
                        enemy.TakePoison(10.0f + GreenBean.Attributes.PoisonDamagePerBean * GreenBean.Attributes.Collected,
                            1.0f - (GreenBean.Attributes.PoisonTickSpeedPerBean * GreenBean.Attributes.Collected), 
                            6 + (GreenBean.Attributes.ExtraPoisonTickPerBean * GreenBean.Attributes.Collected));
                    }
                }
            }
            ResetBeans();
            Debug.Log("I JUST FARTED!!!");
        }

        #endregion

        private void Update()
        {
            BreatheIn();
            m_fireCooldown = Mathf.Max(m_fireCooldown - Time.deltaTime, 0);
        }

        private void Shoot()
        {
            var projectile = m_storedProjectiles.Pop();
            primaryMaterial.color = m_storedProjectiles.Count > 0 ? secondaryMaterial.color : defaultPrimaryColor;
            secondaryMaterial.color = m_storedProjectiles.Count > 1
                ? (m_storedProjectiles.ToArray()[1]).GetComponent<Projectile.Projectile>().DragonColor * 1.3f
                : defaultSecondaryColor;

            List<Projectile.Projectile> newProjectiles = new List<Projectile.Projectile>();
            newProjectiles.Add(Instantiate( projectile, mouthPosition.position, mouthPosition.rotation));


            switch (projectile.type)
            {
                case Bean.Bean.Type.RED:
                    if (RedBean.Attributes.ActivatedEffects.Contains(RedBean.Effect.Spread))
                    {
                        var extraShotCount = RedBean.Attributes.Collected * RedBean.Attributes.ExtraShotPerBean;
                        for (int i = 1; i <= extraShotCount; i++)
                        {
                            var aimOffset = maximumShotSpread * ((int)((i + 1) / 2) / 3.0f) * (i % 2 == 0 ? -1 : 1);
                            newProjectiles.Add(Instantiate( projectile, mouthPosition.position, 
                            Quaternion.Euler(mouthPosition.rotation.eulerAngles.x,  
                                mouthPosition.rotation.eulerAngles.y + aimOffset,
                                mouthPosition.rotation.eulerAngles.z)));
                        }
                    }

                    foreach (var newProjectile in newProjectiles)
                    {
                        var hit = Random.Range(0.0f, 1.0f);
                        if (RedBean.Attributes.ActivatedEffects.Contains(RedBean.Effect.InstantKill))
                        {
                            if (hit <= RedBean.Attributes.InstantKillChancePerBean * RedBean.Attributes.Collected)
                            {
                                newProjectile.Damage = Single.PositiveInfinity;
                                continue;
                            }
                        }
                        if (RedBean.Attributes.ActivatedEffects.Contains(RedBean.Effect.CriticalChance))
                        {
                            
                            if (hit <= RedBean.Attributes.CriticalChancePerBean * RedBean.Attributes.Collected)
                            {
                                newProjectile.Damage *= RedBean.Attributes.BaseCriticalMultiplier +
                                                        (RedBean.Attributes.Collected *
                                                         RedBean.Attributes.CriticalMultiplierPerBean);
                            }
                        }
                    }
                break;
            case Bean.Bean.Type.BLUE:
                if (BlueBean.Attributes.ActivatedEffects.Contains(BlueBean.Effect.Spread))
                {
                    var extraShotCount = (int) (BlueBean.Attributes.Collected * BlueBean.Attributes.ExtraShotPerBean);
                    for (int i = 1; i <= extraShotCount; i++)
                    {
                        var aimOffset = maximumShotSpread * ((int)((i + 1) / 2) / 3.0f) * (i % 2 == 0 ? -1 : 1);
                        newProjectiles.Add(Instantiate(projectile, mouthPosition.position, 
                            Quaternion.Euler(mouthPosition.rotation.eulerAngles.x,  
                                mouthPosition.rotation.eulerAngles.y + aimOffset,
                                mouthPosition.rotation.eulerAngles.z)));
                    }
                }

                if (BlueBean.Attributes.ActivatedEffects.Contains(BlueBean.Effect.PermanentFreeze))
                {
                    foreach (var newProjectile in newProjectiles)
                    {
                        ((Projectile.IceProjectile)newProjectile).unfrozenSpeedMultiplier =
                           1 / (1 + BlueBean.Attributes.PermanentSlowMultiplierPerBean * BlueBean.Attributes.Collected);
                    }
                }
                break;
            case Bean.Bean.Type.GREEN:
                if (GreenBean.Attributes.ActivatedEffects.Contains(GreenBean.Effect.SpreadChance))
                {
                    var extraShotCount = 7;
                    var hit = Random.Range(0.0f, 1.0f);
                    for (int i = 1; i <= extraShotCount && 
                                    hit <= (GreenBean.Attributes.SpreadChancePerBean * GreenBean.Attributes.Collected);
                         i++)
                    {
                        var aimOffset = maximumShotSpread * ((int)((i + 1) / 2) / 3.0f) * (i % 2 == 0 ? -1 : 1);
                        newProjectiles.Add(Instantiate(projectile, mouthPosition.position, 
                            Quaternion.Euler(mouthPosition.rotation.eulerAngles.x,  
                                mouthPosition.rotation.eulerAngles.y + aimOffset,
                                mouthPosition.rotation.eulerAngles.z)));
                        hit = Random.Range(0.0f, 1.0f);
                    }
                }
                if (GreenBean.Attributes.ActivatedEffects.Contains(GreenBean.Effect.PoisonDamage))
                {
                    foreach (var newProjectile in newProjectiles)
                    {
                        ((Projectile.PoisonProjectile)newProjectile).Damage +=
                            GreenBean.Attributes.PoisonDamagePerBean * GreenBean.Attributes.Collected;
                    }
                }
                if (GreenBean.Attributes.ActivatedEffects.Contains(GreenBean.Effect.PoisonDuration))
                {
                    foreach (var newProjectile in newProjectiles)
                    {
                        ((Projectile.PoisonProjectile)newProjectile).numberOfPoisonHits +=
                            GreenBean.Attributes.ExtraPoisonTickPerBean * GreenBean.Attributes.Collected;
                    }
                }
               if (GreenBean.Attributes.ActivatedEffects.Contains(GreenBean.Effect.PoisonSpeed))
                {
                    foreach (var newProjectile in newProjectiles)
                    {
                        ((Projectile.PoisonProjectile)newProjectile).poisonHitTime -=
                            GreenBean.Attributes.PoisonTickSpeedPerBean * GreenBean.Attributes.Collected;
                    }
                }
                break;
            }
            foreach (var newProjectile in newProjectiles)
            {
                newProjectile.Init(this.gameObject);
            }
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
                /*newProjectile.slowEffect += m_fireballTypeAttributes[(int) newProjectile.type].ShotSlowEffect;
                newProjectile.Damage *= 1 + m_fireballTypeAttributes[(int) newProjectile.type].DamageMultiplier;*/
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
            /*if (!m_fireballTypeAttributes.ContainsKey((int) bean.BeanType))
            {
                m_fireballTypeAttributes[(int) bean.BeanType] = new ShotAttributes();
            }*/

            if (m_collectedBeansCount >= 5)
            {
                Fart();
            }
            
            switch (bean.BeanType)
            {
                case Bean.Bean.Type.RED:
                    RedBean.Attributes.ActivatedEffects.Add(((RedBean) bean).effectOrder[RedBean.Attributes.Collected]);
                    RedBean.Attributes.Collected++;
                    CollectedBeanInfo.Add( new KeyValuePair<int, string>(
                        (int) Bean.Bean.Type.RED, RedBean.Attributes.EffectInfo[RedBean.Attributes.Collected]));
                    BeansChanged?.Invoke(CollectedBeanInfo);
                    break;
                case Bean.Bean.Type.BLUE:
                    BlueBean.Attributes.ActivatedEffects.Add(((BlueBean) bean).effectOrder[BlueBean.Attributes.Collected]);
                    BlueBean.Attributes.Collected++;
                    CollectedBeanInfo.Add( new KeyValuePair<int, string>(
                        (int) Bean.Bean.Type.BLUE, BlueBean.Attributes.EffectInfo[ BlueBean.Attributes.Collected]));
                    BeansChanged?.Invoke(CollectedBeanInfo);
                    break;
                case Bean.Bean.Type.GREEN:
                    GreenBean.Attributes.ActivatedEffects.Add(((GreenBean) bean).effectOrder[GreenBean.Attributes.Collected]);
                    GreenBean.Attributes.Collected++;
                    CollectedBeanInfo.Add( new KeyValuePair<int, string>(
                        (int) Bean.Bean.Type.GREEN, GreenBean.Attributes.EffectInfo[GreenBean.Attributes.Collected]));
                    BeansChanged?.Invoke(CollectedBeanInfo);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            beanSpawner.beanProbabilities[(int)bean.BeanType - 1] *= 1.1f;
            /*switch (bean.BeanEfect)
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
            }*/
            m_collectedBeansCount++;
            Debug.Log(m_collectedBeansCount + "/5 Beans collected");
            vaccum.RemoveBean(bean);
            Destroy(bean.gameObject);
        }

        private void ResetBeans()
        {
            RedBean.Attributes.ToDefault();
            BlueBean.Attributes.ToDefault();
            GreenBean.Attributes.ToDefault();
            CollectedBeanInfo.Clear();
            BeansChanged?.Invoke(CollectedBeanInfo);
        }
        
        private struct DragonAttributes
        {
            public enum FartEffect
            {
                NONE,
                RED,
                BLUE,
                GREEN
            }
            
            public int IgnoreDamageChance;
            public int IgnoreEffectChance;
            public FartEffect ExtraFartEffect;

            public void ToDefault()
            {
                IgnoreDamageChance = 0;
                IgnoreEffectChance = 0;
                ExtraFartEffect = FartEffect.NONE;
            }
        }
        
        private class ShotAttributes
        {
            public int ExtraShots = 0;
            public float DamageMultiplier = 1;
            public float ShotSlowEffect = 0;
           
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
        }
    }
}
