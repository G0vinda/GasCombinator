using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Bean
{
    public class BeanSpawner : MonoBehaviour
    {
        public Bean[] beanPrefabs;
        public float minSpawnDelay;
        public float maxSpawnDelay;
        public Transform lowerLimit;
        public Transform upperLimit;

        private void Start()
        {
            Invoke(nameof(SpawnBean), Random.Range(minSpawnDelay, maxSpawnDelay));
        }

        protected void SpawnBean()
        {
            Instantiate(beanPrefabs[Random.Range(0, beanPrefabs.Length)], new Vector3(Random.Range(lowerLimit.position.x, upperLimit.position.x),
                Random.Range(lowerLimit.position.y, upperLimit.position.y), lowerLimit.position.z), Quaternion.identity);
            Invoke(nameof(SpawnBean), Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }
}