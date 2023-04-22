using System;
using System.Collections;
using DG.Tweening;
using Player;
using UnityEngine;

namespace Enemy
{
    public class SpearEnemy : Enemy
    {
        [SerializeField] private float rangeToStartAttack;
        [SerializeField] private float attackPreparationTime;
        [SerializeField] private float attackRange;
        [SerializeField] private float attackTime;
        [SerializeField] private float timeBeforeNextAttack;
        [SerializeField] private float dieTime;
        
        private bool m_isPerformingAttack;
        private bool m_isPreparingAttack;
        private Vector3 m_currentAttackDestination;
        private Tweener m_attackTweener;
        private float m_currentAttackPause;

        private int m_prepareForAttackHash;
        private int m_afterAttackHash;
        private int m_dieHash;

        private void Start()
        {
            m_prepareForAttackHash = Animator.StringToHash("PrepareForAttack");
            m_afterAttackHash = Animator.StringToHash("AfterAttack");
            m_dieHash = Animator.StringToHash("Die");
        }

        void Update()
        {
            if (!ProcessFreeze())
                return;
            
            if(m_isPreparingAttack)
                return;
            
            if (m_isPerformingAttack)
                return;
            
            
            if (PlayerIsInSpearRange())
            {
                m_isPreparingAttack = true;
                StartCoroutine(PrepareAttack());
                return;
            }
                
            FollowPlayer();
        }
        
        private bool PlayerIsInSpearRange()
        {
            if (m_currentAttackPause > 0)
                return false;
            
            var distanceToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);
            return distanceToPlayer < rangeToStartAttack;
        }

        private IEnumerator PrepareAttack()
        {
            var attackDirection = Vector3.Normalize(PlayerTransform.position - transform.position);
            transform.rotation = Quaternion.LookRotation(attackDirection);
            m_currentAttackDestination = transform.position + attackDirection * attackRange;
            NavMeshAgent.enabled = false;
            //animator.SetTrigger(m_prepareForAttackHash);
            yield return new WaitForSeconds(attackPreparationTime);
            m_attackTweener = transform.DOMove(m_currentAttackDestination, attackTime).SetEase(Ease.InBack).OnComplete(
                () => { StartCoroutine(TimeAttackPause());});
            m_isPreparingAttack = false;
            m_isPerformingAttack = true;
        }

        private IEnumerator TimeAttackPause()
        {
            NavMeshAgent.enabled = true;
            m_isPerformingAttack = false;
            m_attackTweener?.Kill();
            m_currentAttackPause = timeBeforeNextAttack;
            
            while (m_currentAttackPause > 0)
            {
                Debug.Log(DateTime.Now);
                m_currentAttackPause -= Time.deltaTime;
                yield return null;
            }
            
            Debug.Log(DateTime.Now);
        }

        private void OnCollisionEnter(Collision other)
        {
            if(m_isPerformingAttack) // TODO: Check for collision with player
                StartCoroutine(TimeAttackPause());
        }


        protected override void Die()
        {
            NavMeshAgent.enabled = false;
            //animator.SetTrigger(m_dieHash);
            Destroy(gameObject, dieTime);
            enabled = false;
        }
    }
}
