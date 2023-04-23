using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("EnemyWaveData")]
        [SerializeField] private GameObject[] enemiesWave1;
        [SerializeField] private int[] enemiesAmountWave1;
        
        [SerializeField] private GameObject[] enemiesWave2;
        [SerializeField] private int[] enemiesAmountWave2;
        
        [Header("EnemySpawnArea")]
        [SerializeField] private Transform lowerLimit;
        [SerializeField] private Transform upperLimit;

        [SerializeField] private float pauseTimeBetweenWaves;

        private Dictionary<int, (GameObject[], int[])> m_levelEnemyData;
        private List<GameObject> m_spawnedEnemies = new ();
        private int m_currentWaveId;
        private const int MAXWaveId = 1;

        private void Awake()
        {
            m_levelEnemyData = new()
            {
                { 0, (enemiesWave1, enemiesAmountWave1) },
                { 1, (enemiesWave2, enemiesAmountWave2) }
            };
        }

        private void Start()
        {
            SpawnNextWave();
        }

        private void OnEnable()
        {
            Enemy.EnemyDied += CheckForNextWave;
        }

        private void OnDisable()
        {
            Enemy.EnemyDied -= CheckForNextWave;
        }

        private void SpawnNextWave()
        {
            var waveId = m_currentWaveId;
            for (var i = 0; i < m_levelEnemyData[waveId].Item2.Length; i++)
            {
                for (var j = 0; j < m_levelEnemyData[waveId].Item2[i]; j++)
                {
                    SpawnEnemy(m_levelEnemyData[waveId].Item1[i]);
                }  
            }
            
        }

        private void SpawnEnemy(GameObject enemyPrefab)
        {
            var spawnPosition = new Vector3(Random.Range(lowerLimit.position.x, upperLimit.position.x),
                lowerLimit.position.y, Random.Range(lowerLimit.position.z, upperLimit.position.z));
            var allowedPositionDistance = 30f;
            NavMesh.SamplePosition(spawnPosition, out var navMeshHit, allowedPositionDistance, NavMesh.AllAreas);
            spawnPosition = navMeshHit.position;
            
            m_spawnedEnemies.Add(Instantiate(enemyPrefab, spawnPosition, Quaternion.identity));
        }

        private void CheckForNextWave(GameObject destroyedEnemy)
        {
            m_spawnedEnemies.Remove(destroyedEnemy);
            if(m_spawnedEnemies.Count > 0)
                return;
            
            // All enemies have been destroyed
            m_currentWaveId++;
            if(m_currentWaveId > MAXWaveId)
                return; // Handle win here
            
            Debug.Log("Wave defeated");
            Invoke(nameof(SpawnNextWave), pauseTimeBetweenWaves);
        }
    }
}
