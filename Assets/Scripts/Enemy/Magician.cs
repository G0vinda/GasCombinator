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
        [SerializeField] private float dieTime;

        private Vector3 m_currentAimTarget;
        private float m_currentPauseTime;
        private bool m_isPreparingAttack;

        private void Update()
        {
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
            NavMeshAgent.enabled = false;
            m_currentAimTarget = PlayerTransform.position;
            SetWandToAttackPosition();

            yield return new WaitForSeconds(wandPreparationTime);
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
            yield return new WaitForSeconds(wandPreparationTime);

            NavMeshAgent.enabled = true;
            m_currentPauseTime -= wandPreparationTime;
            while (m_currentPauseTime > 0)
            {
                m_currentPauseTime -= Time.deltaTime;
                yield return null;
            }
        }
        
        private void SetWandToAttackPosition()
        {
            var spearSequence = DOTween.Sequence();
            spearSequence.Append(wandTransform.DOMove(wandShootTransform.position, wandPreparationTime));
            spearSequence.Join(wandTransform.DORotate(wandShootTransform.rotation.eulerAngles, wandPreparationTime));
            spearSequence.SetEase(Ease.InQuad);
        }

        private void SetWandToWalkPosition()
        {
            var spearSequence = DOTween.Sequence();
            spearSequence.Append(wandTransform.DOMove(wandWalkTransform.position, wandPreparationTime));
            spearSequence.Join(wandTransform.DORotate(wandWalkTransform.rotation.eulerAngles, wandPreparationTime));
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