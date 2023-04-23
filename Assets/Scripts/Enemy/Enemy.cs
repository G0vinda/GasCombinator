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

        [Header("MovementAnimationValues")] 
        [SerializeField] private float bounceHeight;
        [SerializeField] private float bounceTime;

        [Header("Death")] 
        [SerializeField] private float dieTime;
        [SerializeField] private ParticleSystem ashParticles;

        [Header("PoisonValues")] 
        [SerializeField] private int numberOfPoisonHits;
        [SerializeField] private float poisonHitTime;

        private float m_currentSpeed;
        private float CurrentSpeed
        {
            get => m_currentSpeed;
            set
            {
                if (FreezeTimer > 0)
                {
                    m_currentSpeed = 0;
                    return;   
                }

                m_currentSpeed = value;
                NavMeshAgent.speed = m_currentSpeed;
            }
        }
        
        protected Transform PlayerTransform;
        protected NavMeshAgent NavMeshAgent;
        protected float FreezeTimer;
        
        private float m_health;
        private WaitForSeconds m_poisonTime;
        private WaitForSeconds m_slowDuration;
        private MeshRenderer m_renderer;
        private Color m_defaultColor;
        
        private Tweener m_hurtEffectTween;
        private Tweener m_walkingTween;

        private void Awake()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
            CurrentSpeed = defaultSpeed;
            NavMeshAgent.speed = CurrentSpeed;
            PlayerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
            
            m_health = maxHealth;
            m_poisonTime = new WaitForSeconds(poisonHitTime);
            m_slowDuration = new WaitForSeconds(slowDuration);

            m_renderer = transform.GetComponentInChildren<MeshRenderer>();
            m_defaultColor = m_renderer.material.color;
            
            StartWalkingAnimation();
        }

        protected void FollowPlayer()
        {
            if(!NavMeshAgent.enabled)
                return;
            
            NavMeshAgent.destination = PlayerTransform.position;
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

        public virtual void Freeze(float freezeTime) // Call when Enemy gets stunned/frozen
        {
            StopWalkingAnimation();
            freezeBlock.SetActive(true);
            CurrentSpeed = 0;
            FreezeTimer = freezeTime;
        }

        protected bool ProcessFreeze()
        {
            if (FreezeTimer > 0)
            {
                FreezeTimer -= Time.deltaTime;
                if (FreezeTimer > 0)
                    return false;
                
                Unfreeze();
            }
            
            return true;
        }

        protected virtual void Unfreeze()
        {
            freezeBlock.SetActive(false);
            CurrentSpeed = defaultSpeed;
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

            CurrentSpeed = defaultSpeed;
        }
        
        private void ShowHurtEffect()
        {
            var effectDuration = 0.5f;
            m_hurtEffectTween?.Kill(); // If there is a hurt effect running, restart it

            m_hurtEffectTween = DOVirtual.Color(Color.red, m_defaultColor, effectDuration,
                colorValue => { m_renderer.material.color = colorValue; }).SetEase(Ease.OutExpo);
        }

        protected void StartWalkingAnimation()
        {
            NavMeshAgent.enabled = true;
            m_walkingTween = transform.DOMoveY(bounceHeight, bounceTime).SetLoops(-1, LoopType.Yoyo);
        }

        protected void StopWalkingAnimation()
        {
            m_walkingTween?.Kill(); 
            var position = transform.position;
            position.y = 0;
            transform.position = position;
            NavMeshAgent.enabled = false;
        }

        private void Die()
        {
            Instantiate(ashParticles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}