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
<<<<<<< Updated upstream
=======
        [SerializeField] private GameObject freezeBlock;

        [Header("MovementAnimationValues")] [SerializeField]
        private float bounceHeight;

        [SerializeField] private float bounceTime;

        [Header("Death")] [SerializeField] private float dieTime;
        [SerializeField] private ParticleSystem ashParticles;
        private AudioSource m_audioSource;
        
        public int damage;

        public static event Action<GameObject> EnemyDied;

        [SerializeField] private float m_currentSpeed;
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
>>>>>>> Stashed changes
        
        [Header("PoisonValues")] 
        [SerializeField] private int numberOfPoisonHits;
        [SerializeField] private float poisonHitTime;

        private NavMeshAgent m_navMeshAgent;
        private float m_health;
        private WaitForSeconds m_poisonTime;
        private WaitForSeconds m_slowDuration;
        private float m_currentSpeed;
        private MeshRenderer m_renderer;
        private Color m_defaultColor;
        private Tweener m_hurtEffectTween;
        private Transform m_playerTransform;
        
        private void Awake()
        {
<<<<<<< Updated upstream
            m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_currentSpeed = defaultSpeed;
            m_navMeshAgent.speed = m_currentSpeed;
            m_playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
=======
            m_audioSource = GetComponent<AudioSource>();

            NavMeshAgent = GetComponent<NavMeshAgent>();
            CurrentSpeed = defaultSpeed;
            NavMeshAgent.speed = CurrentSpeed;
            PlayerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
>>>>>>> Stashed changes
            
            m_health = maxHealth;
            m_poisonTime = new WaitForSeconds(poisonHitTime);
            m_slowDuration = new WaitForSeconds(slowDuration);
            
            m_renderer = GetComponent<MeshRenderer>();
            m_defaultColor = m_renderer.material.color;
        }

        private void Update()
        {
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
            m_currentSpeed *= newSpeedFactor;
            m_navMeshAgent.speed = m_currentSpeed;
            yield return m_slowDuration;

            m_currentSpeed /= newSpeedFactor;
            m_navMeshAgent.speed = m_currentSpeed;
        }
        
        private void ShowHurtEffect()
        {
            var effectDuration = 0.5f;
            m_hurtEffectTween?.Kill(); // If there is a hurt effect running, restart it

            m_hurtEffectTween = DOVirtual.Color(Color.red, m_defaultColor, effectDuration,
                colorValue => { m_renderer.material.color = colorValue; }).SetEase(Ease.OutExpo);
        }

<<<<<<< Updated upstream
        protected abstract void Die();
=======
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
            EnemyDied?.Invoke(gameObject);
            AudioSource.PlayClipAtPoint(m_audioSource.clip, transform.position);
            Destroy(gameObject);
        }
>>>>>>> Stashed changes
    }
}