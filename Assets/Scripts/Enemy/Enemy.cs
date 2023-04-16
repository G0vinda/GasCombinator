using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    //[RequireComponent(typeof(NavMeshAgent))]
    public abstract class Enemy : MonoBehaviour
    {
        [SerializeField] private int maxHealth;
        [SerializeField] private float defaultSpeed;
        [SerializeField] private float slowDuration;
        
        [Header("PoisonValues")] 
        [SerializeField] private int numberOfPoisonHits;
        [SerializeField] private float poisonHitTime;

        private NavMeshAgent m_navMeshAgent;
        private float m_health;
        private WaitForSeconds m_poisonTime;
        private WaitForSeconds m_slowDuration;
        private float m_currentSpeed;
        
        private void Awake()
        {
            //m_navMeshAgent = GetComponent<NavMeshAgent>();
            m_health = maxHealth;
            m_poisonTime = new WaitForSeconds(poisonHitTime);
            m_slowDuration = new WaitForSeconds(slowDuration);
        }

        public void TakeDamage(float dmgAmount)
        {
            m_health -= dmgAmount;
            if (m_health <= 0)
                Die();
        }

        public void TakePoison(float dmgPerHit)
        {
            StartCoroutine(ProcessPoison(dmgPerHit));
        }

        public void TakeSlow(float slowFactor)
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
            yield return m_slowDuration;

            m_currentSpeed /= newSpeedFactor;
        }

        private void Die()
        {
            // Maybe we want to do something else here
            Destroy(gameObject);
        }
    }
}