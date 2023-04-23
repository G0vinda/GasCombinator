using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bean
{
    public class BeanSpawner : MonoBehaviour
    {
        [SerializeField] private Bean[] beanPrefabs;
        [Tooltip("Spawn probability of the beans, in whole numbers")] 
        public float[] beanProbabilities;
        [SerializeField] private float minSpawnDelay;
        [SerializeField] private float maxSpawnDelay;
        [SerializeField] private Transform lowerLimit;
        [SerializeField] private Transform upperLimit;

        private void Start()
        {
            Invoke(nameof(SpawnBean), Random.Range(minSpawnDelay, maxSpawnDelay));
        }

        protected void SpawnBean()
        {
            var sumOfProbabilities = beanProbabilities.Sum();
            var randomBeanPick = Random.Range(0, sumOfProbabilities);
            
            var probabilityCounter = 0;
            while (randomBeanPick >= beanProbabilities[probabilityCounter])
            {
                randomBeanPick -= beanProbabilities[probabilityCounter];
                probabilityCounter++;
            }
            
            var beanToSpawn = beanPrefabs[probabilityCounter];
            var spawnPosition = new Vector3(Random.Range(lowerLimit.position.x, upperLimit.position.x),
                lowerLimit.position.y, Random.Range(lowerLimit.position.z, upperLimit.position.z));
            
            Instantiate(beanToSpawn, spawnPosition, Quaternion.identity);
            
            Invoke(nameof(SpawnBean), Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }
}