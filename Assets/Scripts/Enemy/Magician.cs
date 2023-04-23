using System;
using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class Magician : Enemy
    {
        [SerializeField] private GameObject magicProjectilePrefab;
        [SerializeField] private float rangeToStartAttack;
        [SerializeField] private float pauseTimeAfterAttack;
        [SerializeField] private float dieTime;
        [SerializeField] private Transform wandTipTransform;

        private float m_currentPauseTime;

        private void Update()
        {
            if (PlayerIsInShootRange())
            {
                Shoot();
            }
            
            FollowPlayer();
        }

        private void Shoot()
        {
            var shootDirection = PlayerTransform.position - wandTipTransform.position;
            shootDirection.y = 0;
            var projectileRotation = Quaternion.LookRotation(shootDirection);
            Instantiate(magicProjectilePrefab, wandTipTransform.position, projectileRotation);
        }
        
        private bool PlayerIsInShootRange()
        {
            if (m_currentPauseTime > 0)
                return false;
            
            var distanceToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);
            return distanceToPlayer < rangeToStartAttack;
        }

        private IEnumerator ProcessAttackPause()
        {
            m_currentPauseTime = pauseTimeAfterAttack;

            while (m_currentPauseTime > 0)
            {
                m_currentPauseTime -= Time.deltaTime;
                yield return null;
            }
        }

        protected override void Die()
        {
            NavMeshAgent.enabled = false;
            Destroy(gameObject, dieTime);
            enabled = false;
        }
    }
}