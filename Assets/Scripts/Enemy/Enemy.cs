using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private float defaultSpeed;
        [SerializeField] private float slowDuration;
        [SerializeField] private GameObject freezeBlock;
        
        [Header("PoisonValues")] 
        [SerializeField] private int numberOfPoisonHits;
        [SerializeField] private float poisonHitTime;

        private float m_currentSpeed;
        private float CurrentSpeed
        {
            get => m_currentSpeed;
            set
            {
                if (m_freezeTimer > 0)
                {
                    m_currentSpeed = 0;
                    return;   
                }

                m_currentSpeed = value;
                m_navMeshAgent.speed = m_currentSpeed;
            }
        }
        
        private NavMeshAgent m_navMeshAgent;
        private float m_health;
        private WaitForSeconds m_poisonTime;
        private WaitForSeconds m_slowDuration;
        private MeshRenderer m_renderer;
        private Color m_defaultColor;
        private Tweener m_hurtEffectTween;
        private Transform m_playerTransform;
        private float m_freezeTimer;
        
        private void Awake()
        {
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            CurrentSpeed = defaultSpeed;
            m_navMeshAgent.speed = CurrentSpeed;
            m_playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
            
            m_health = maxHealth;
            m_poisonTime = new WaitForSeconds(poisonHitTime);
            m_slowDuration = new WaitForSeconds(slowDuration);

            m_renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
            m_defaultColor = m_renderer.material.color;
        }

        private void Update()
        {
            if (m_freezeTimer > 0)
            {
                m_freezeTimer -= Time.deltaTime;
                if (m_freezeTimer > 0)
                    return;

                freezeBlock.SetActive(false);
                CurrentSpeed = defaultSpeed;
            }
            FollowPlayer();
        }

        private void FollowPlayer()
        {
            m_navMeshAgent.destination = m_playerTransform.position;
        }

        public void TakeDamage(float dmgAmount)
        {
            m_health -= dmgAmount;
            if (m_health <= 0)
                Die();

            ShowHurtEffect();
        }

        public void TakePoison(float dmgPerHit) // Call when Enemy gets poisoned
        {
            StartCoroutine(ProcessPoison(dmgPerHit));
        }

        public void TakeSlow(float slowFactor) // Call when Enemy gets slowed
        {
            StartCoroutine(ProcessSlow(slowFactor));
        }

        public void Freeze(float freezeTime) // Call when Enemy gets stunned/frozen
        {
            freezeBlock.SetActive(true);
            CurrentSpeed = 0;
            m_freezeTimer = freezeTime;
        }

        private IEnumerator ProcessPoison(float dmgPerHit)
        {
            var poisonCounter = numberOfPoisonHits;
            while (poisonCounter > 0)
            {   
                TakeDamage(dmgPerHit);
                poisonCounter--;
                yield return m_poisonTime;
            }
        }

        private IEnumerator ProcessSlow(float slowFactor)
        {
            var newSpeedFactor = 1 - slowFactor;
            CurrentSpeed *= newSpeedFactor;
            yield return m_slowDuration;

            CurrentSpeed /= newSpeedFactor;
        }
        
        private void ShowHurtEffect()
        {
            var effectDuration = 0.5f;
            m_hurtEffectTween?.Kill(); // If there is a hurt effect running, restart it

            m_hurtEffectTween = DOVirtual.Color(Color.red, m_defaultColor, effectDuration,
                colorValue => { m_renderer.material.color = colorValue; }).SetEase(Ease.OutExpo);
        }

        protected abstract void Die();
    }
}