using DG.Tweening;
using UnityEngine;

namespace Enemy
{
    public class EnemyInstantiate : MonoBehaviour
    {
        [SerializeField] private Transform enemyModel;
        [SerializeField] private float modelYOffset;
        [SerializeField] private float circleTime;
        [SerializeField] private float spawnTime;
        [SerializeField] private GameObject circleObject;
        [SerializeField] private Collider enemyCollider;

        private Enemy m_enemy;
        private Vector3 m_defaultModelPosition;

        private void Awake()
        {
            m_enemy = GetComponent<Enemy>();
            m_defaultModelPosition = enemyModel.transform.position;
            enemyModel.transform.position = m_defaultModelPosition + new Vector3(0, -modelYOffset, 0);
            Invoke(nameof(SpawnEnemy), circleTime);
        }

        private void SpawnEnemy()
        {
            Debug.Log("SpawnEnemyGotCalled");
            enemyModel.transform.DOMove(m_defaultModelPosition, spawnTime).SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    m_enemy.enabled = true;
                    enemyCollider.enabled = true;
                    Destroy(circleObject);
                });
        }
    }
}