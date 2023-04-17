using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnIntervalTime;
        [SerializeField] private float timeToNextWave;
        [SerializeField] private int startSpawnAmount;

        private int m_spawnAmount;
        private WaitForSeconds m_spawnIntervalPause;
        private WaitForSeconds m_nextWavePause;

        void Start()
        {
            m_spawnAmount = startSpawnAmount;
            m_spawnIntervalPause = new WaitForSeconds(spawnIntervalTime);
            m_nextWavePause = new WaitForSeconds(timeToNextWave);
            StartCoroutine(Spawn());
        }

        private IEnumerator Spawn()
        {
            while (true)
            {
                var spawnCount = 0;
                while (spawnCount < m_spawnAmount)
                {
                    Instantiate(enemyPrefab, transform.position, Quaternion.identity);
                    spawnCount++;
                    yield return m_spawnIntervalPause;
                }

                m_spawnAmount++;
                yield return m_nextWavePause;
            }
        } 
        
    }
}
