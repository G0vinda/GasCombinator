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
        
        [SerializeField] private GameObject[] enemiesWave3;
        [SerializeField] private int[] enemiesAmountWave3;
        
        [SerializeField] private GameObject[] enemiesWave4;
        [SerializeField] private int[] enemiesAmountWave4;

        [SerializeField] private GameObject[] enemiesWave5;
        [SerializeField] private int[] enemiesAmountWave5;
        
        [SerializeField] private GameObject[] enemiesWave6;
        [SerializeField] private int[] enemiesAmountWave6;

        [SerializeField] private GameObject[] enemiesWave7;
        [SerializeField] private int[] enemiesAmountWave7;
        
        [SerializeField] private GameObject[] enemiesWave8;
        [SerializeField] private int[] enemiesAmountWave8;

        [SerializeField] private GameObject[] enemiesWave9;
        [SerializeField] private int[] enemiesAmountWave9;
        
        [SerializeField] private GameObject[] enemiesWave10;
        [SerializeField] private int[] enemiesAmountWave10;

        [SerializeField] private GameObject[] enemiesWave11;
        [SerializeField] private int[] enemiesAmountWave11;
        
        [SerializeField] private GameObject[] enemiesWave12;
        [SerializeField] private int[] enemiesAmountWave12;

        [SerializeField] private GameObject[] enemiesWave13;
        [SerializeField] private int[] enemiesAmountWave13;
        
        [SerializeField] private GameObject[] enemiesWave14;
        [SerializeField] private int[] enemiesAmountWave14;

        [SerializeField] private GameObject[] enemiesWave15;
        [SerializeField] private int[] enemiesAmountWave15;
        
        [Header("EnemySpawnArea")]
        [SerializeField] private Transform lowerLimit;
        [SerializeField] private Transform upperLimit;

        [SerializeField] private float pauseTimeBetweenWaves;

        public static event Action<int> WaveStarts;

        private Dictionary<int, (GameObject[], int[])> m_levelEnemyData;
        private List<GameObject> m_spawnedEnemies = new ();
        private int m_currentWaveId;
        private const int MAXWaveId = 1;

        private void Awake()
        {
            m_levelEnemyData = new()
            {
                { 0, (enemiesWave1, enemiesAmountWave1) },
                { 1, (enemiesWave2, enemiesAmountWave2) },
                { 2, (enemiesWave3, enemiesAmountWave3) },
                { 3, (enemiesWave4, enemiesAmountWave4) },
                { 4, (enemiesWave5, enemiesAmountWave5) },
                { 5, (enemiesWave6, enemiesAmountWave6) },
                { 6, (enemiesWave7, enemiesAmountWave7) },
                { 7, (enemiesWave8, enemiesAmountWave8) },
                { 8, (enemiesWave9, enemiesAmountWave9) },
                { 9, (enemiesWave10, enemiesAmountWave10) },
                { 10, (enemiesWave11, enemiesAmountWave11) },
                { 11, (enemiesWave12, enemiesAmountWave12) },
                { 12, (enemiesWave13, enemiesAmountWave13) },
                { 13, (enemiesWave14, enemiesAmountWave14) },
                { 14, (enemiesWave15, enemiesAmountWave15) }
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
            WaveStarts?.Invoke(m_currentWaveId);
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
