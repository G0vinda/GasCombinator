using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Enemy
{
    public class SpearEnemy : Enemy
    {
        [Header("SpearReferences")] 
        [SerializeField] private Transform spearTransform;
        [SerializeField] private Collider spearCollider;
        [SerializeField] private Transform spearWalkTransform;
        [SerializeField] private Transform spearAttackTransform;
        
        [Header("SpearAttackValues")]
        [SerializeField] private float rangeToStartAttack;
        [SerializeField] private float attackRange;
        [SerializeField] private float spearPreparationTime;
        [SerializeField] private float attackTime;
        [SerializeField] private float timeBeforeNextAttack;
        [SerializeField] private float dieTime;
        
        private bool m_isPerformingAttack;
        private bool m_isPreparingAttack;
        private Vector3 m_currentAttackDestination;
        private Tweener m_attackTweener;
        private float m_currentAttackPause;

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
            m_isPreparingAttack = true;
            var attackDirection = Vector3.Normalize(PlayerTransform.position - transform.position);
            transform.rotation = Quaternion.LookRotation(attackDirection);
            SetSpearToAttackPosition();
            
            m_currentAttackDestination = transform.position + attackDirection * attackRange;
            NavMeshAgent.enabled = false;
            yield return new WaitForSeconds(spearPreparationTime);
            m_attackTweener = transform.DOMove(m_currentAttackDestination, attackTime).SetEase(Ease.InBack).OnComplete(
                () => { StartCoroutine(TimeAttackPause());});
            m_isPreparingAttack = false;
            m_isPerformingAttack = true;
        }

        private IEnumerator TimeAttackPause()
        {
            m_isPerformingAttack = false;
            m_attackTweener?.Kill();
            m_currentAttackPause = timeBeforeNextAttack;
            SetSpearToWalkPosition();
            yield return new WaitForSeconds(spearPreparationTime);

            m_currentAttackPause -= spearPreparationTime;
            NavMeshAgent.enabled = true;
            while (m_currentAttackPause > 0)
            {
                m_currentAttackPause -= Time.deltaTime;
                yield return null;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("collision detected");
            if(m_isPerformingAttack) 
                StartCoroutine(TimeAttackPause());
        }
        
        private void SetSpearToAttackPosition()
        {
            var spearSequence = DOTween.Sequence();
            spearSequence.Append(spearTransform.DOMove(spearAttackTransform.position, spearPreparationTime));
            spearSequence.Join(spearTransform.DORotate(spearAttackTransform.rotation.eulerAngles, spearPreparationTime));
            spearSequence.SetEase(Ease.InQuad).OnComplete(() => { spearCollider.enabled = true; });
        }

        private void SetSpearToWalkPosition()
        {
            spearCollider.enabled = false;
            
            var spearSequence = DOTween.Sequence();
            spearSequence.Append(spearTransform.DOMove(spearWalkTransform.position, spearPreparationTime));
            spearSequence.Join(spearTransform.DORotate(spearWalkTransform.rotation.eulerAngles, spearPreparationTime));
            spearSequence.SetEase(Ease.InQuad);
        }

        protected override void Die()
        {
            NavMeshAgent.enabled = false;
            Destroy(gameObject, dieTime);
            enabled = false;
        }
    }
}
