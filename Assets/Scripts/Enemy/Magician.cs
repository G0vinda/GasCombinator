using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Enemy
{
    public class Magician : Enemy
    {
        [Header("MagicWandReferences")]
        [SerializeField] private GameObject magicProjectilePrefab;
        [SerializeField] private Transform wandTransform;
        [SerializeField] private Transform wandTipTransform;
        [SerializeField] private Transform wandWalkTransform;
        [SerializeField] private Transform wandShootTransform;
        
        [Header("MagicAttackValues")]
        [SerializeField] private float rangeToStartAttack;
        [SerializeField] private float pauseTimeAfterAttack;
        [SerializeField] private float wandPreparationTime;

        private Vector3 m_currentAimTarget;
        private float m_currentPauseTime;
        private bool m_isPreparingAttack;

        private Tween m_currentTween;

        private void Update()
        {
            if (!ProcessFreeze())
                return;
            
            if(m_isPreparingAttack)
                return;
            
            if (PlayerIsInShootRange())
            {
                StartCoroutine(PrepareAttack());
            }
            
            FollowPlayer();
        }

        private IEnumerator PrepareAttack()
        {
            m_isPreparingAttack = true;
            StopWalkingAnimation();
            m_currentAimTarget = PlayerTransform.position;
            SetWandToAttackPosition();

            var animationTime = wandPreparationTime;
            while (animationTime > 0)
            {
                if (FreezeTimer <= 0)
                    animationTime -= Time.deltaTime;

                yield return null;
            }
            
            m_isPreparingAttack = false;
            Shoot();
        }

        private void Shoot()
        {
            var shootDirection = m_currentAimTarget - wandTipTransform.position;
            shootDirection.y = 0;
            var projectileRotation = Quaternion.LookRotation(shootDirection);
            Instantiate(magicProjectilePrefab, wandTipTransform.position, projectileRotation);
            StartCoroutine(TimeAttackPause());
        }
        
        private bool PlayerIsInShootRange()
        {
            if (m_currentPauseTime > 0)
                return false;
            
            var distanceToPlayer = Vector3.Distance(transform.position, PlayerTransform.position);
            return distanceToPlayer < rangeToStartAttack;
        }

        private IEnumerator TimeAttackPause()
        {
            m_currentPauseTime = pauseTimeAfterAttack;
            SetWandToWalkPosition();
            var animationTime = wandPreparationTime;
            while (animationTime > 0)
            {
                if (FreezeTimer <= 0)
                    animationTime -= Time.deltaTime;

                yield return null;
            }

            StartWalkingAnimation();
            m_currentPauseTime -= wandPreparationTime;
            while (m_currentPauseTime > 0)
            {
                m_currentPauseTime -= Time.deltaTime;
                yield return null;
            }
        }
        
        private void SetWandToAttackPosition()
        {
            var wandSequence = DOTween.Sequence();
            wandSequence.Append(wandTransform.DOMove(wandShootTransform.position, wandPreparationTime));
            wandSequence.Join(wandTransform.DORotate(wandShootTransform.rotation.eulerAngles, wandPreparationTime));
            wandSequence.SetEase(Ease.InQuad);

            m_currentTween = wandSequence;
        }

        private void SetWandToWalkPosition()
        {
            var wandSequence = DOTween.Sequence();
            wandSequence.Append(wandTransform.DOMove(wandWalkTransform.position, wandPreparationTime));
            wandSequence.Join(wandTransform.DORotate(wandWalkTransform.rotation.eulerAngles, wandPreparationTime));
            wandSequence.SetEase(Ease.InQuad);
            
            m_currentTween = wandSequence;
        }

        public override void Freeze(float freezeTime)
        {
            base.Freeze(freezeTime);
            m_currentTween?.Pause();
        }

        protected override void Unfreeze()
        {
            m_currentTween?.Play();
        }
    }
}