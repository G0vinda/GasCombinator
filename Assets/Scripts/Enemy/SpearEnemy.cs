using System.Collections;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
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
        [SerializeField] private float attackCollisionCheckRange;
        [SerializeField] private LayerMask attackCollisionCheckLayer;
        [SerializeField] private float attackRange;
        [SerializeField] private float spearPreparationTime;
        [SerializeField] private float attackTime;
        [SerializeField] private float timeBeforeNextAttack;

        private bool m_isPerformingAttack;
        private bool m_isPreparingAttack;
        private Vector3 m_currentAttackDestination;
        private float m_currentAttackPause;
        
        private Tweener m_attackTweener;
        private Sequence m_spearPrepareSequence;
        private Tween m_currentTween;

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
            {
                m_currentAttackPause -= Time.deltaTime;
                return false;   
            }

            var distanceToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);
            if (distanceToPlayer > rangeToStartAttack)
                return false;

            return IsPlayerInSight();
        }

        private bool IsPlayerInSight()
        {
            var selfPosition = transform.position;
            selfPosition.y = 1f;
            var playerPosition = PlayerTransform.position;
            playerPosition.y = 1f;

            var dirToPlayer = (playerPosition - selfPosition).normalized;
            var crossVector = Vector3.Cross(dirToPlayer, Vector3.up).normalized;

            var selfCheckPosition1 = selfPosition + crossVector * attackCollisionCheckRange;
            var selfCheckPosition2 = selfPosition - crossVector * attackCollisionCheckRange;

            if (Physics.Raycast(selfCheckPosition1, dirToPlayer, attackRange, attackCollisionCheckLayer) ||
                Physics.Raycast(selfCheckPosition2, dirToPlayer, attackRange, attackCollisionCheckLayer))
            {
                m_currentAttackPause = 0.5f;
                return false;
            }

            return true;
        }

        private IEnumerator PrepareAttack()
        {
            m_isPreparingAttack = true;
            var attackDirection = Vector3.Normalize(PlayerTransform.position - transform.position);
            transform.rotation = Quaternion.LookRotation(attackDirection);
            SetSpearToAttackPosition();
            
            m_currentAttackDestination = transform.position + attackDirection * attackRange;
            StopWalkingAnimation();

            var animationTime = spearPreparationTime;
            while (animationTime > 0)
            {
                if (FreezeTimer <= 0)
                    animationTime -= Time.deltaTime;

                yield return null;
            }
            
            m_attackTweener = transform.DOMove(m_currentAttackDestination, attackTime).SetEase(Ease.InBack).OnComplete(
                () => { StartCoroutine(TimeAttackPause());});
            m_currentTween = m_attackTweener;
            m_isPreparingAttack = false;
            m_isPerformingAttack = true;
        }

        private IEnumerator TimeAttackPause()
        {
            m_isPerformingAttack = false;
            m_attackTweener?.Kill();
            m_currentAttackPause = timeBeforeNextAttack;
            SetSpearToWalkPosition();
            if (FreezeTimer > 0)
                m_currentTween.Pause();

            var animationTime = spearPreparationTime;
            while (animationTime > 0)
            {
                if (FreezeTimer <= 0)
                    animationTime -= Time.deltaTime;

                yield return null;
            }
            
            StartWalkingAnimation();
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("collision detected");
            if(m_isPerformingAttack) 
                StartCoroutine(TimeAttackPause());
        }
        
        private void SetSpearToAttackPosition()
        {
            m_spearPrepareSequence = DOTween.Sequence();
            m_spearPrepareSequence.Append(spearTransform.DOMove(spearAttackTransform.position, spearPreparationTime));
            m_spearPrepareSequence.Join(spearTransform.DORotate(spearAttackTransform.rotation.eulerAngles, spearPreparationTime));
            m_spearPrepareSequence.SetEase(Ease.InQuad).OnComplete(() => { spearCollider.enabled = true; });

            m_currentTween = m_spearPrepareSequence;
        }

        private void SetSpearToWalkPosition()
        {
            spearCollider.enabled = false;
            
            m_spearPrepareSequence = DOTween.Sequence();
            m_spearPrepareSequence.Append(spearTransform.DOMove(spearWalkTransform.position, spearPreparationTime));
            m_spearPrepareSequence.Join(spearTransform.DORotate(spearWalkTransform.rotation.eulerAngles, spearPreparationTime));
            m_spearPrepareSequence.SetEase(Ease.InQuad);

            m_currentTween = m_spearPrepareSequence;
        }

        public override void Freeze(float freezeTime, float unfreezeSpeedMultiplier = 1.0f)
        {
            base.Freeze(freezeTime, unfreezeSpeedMultiplier);
            m_currentTween?.Pause();
        }

        protected override void Unfreeze()
        {
            m_currentTween?.Play();
            base.Unfreeze();
        }
    }
}
