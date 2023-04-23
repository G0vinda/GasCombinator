using System;
using Cinemachine;
using Enemy;
using Player;
using UnityEngine;

namespace Projectile
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] private Color dragonColor;
        [SerializeField] private float speed;
        [SerializeField] private float range;
        [SerializeField] protected float damage;
        public float baseCriticalMultiplier = 1.5f;
        [SerializeField] protected bool isPlayerProjectile;
        [HideInInspector]
        public float slowEffect;
        public Color DragonColor => dragonColor;

        public float Damage
        {
            set => damage = value;
            get => damage;
        }
        public Bean.Bean.Type type = Bean.Bean.Type.NEUTRAL;

        private float m_traveledDistance;
        private GameObject owner;
        
        public void Init(GameObject newOwner)
        {
            owner = newOwner;
        }
        
        protected void Move()
        {
            var oldPosition = transform.position;
            transform.Translate(transform.forward * (speed * Time.deltaTime), Space.World);
            m_traveledDistance += Vector3.Distance(oldPosition, transform.position);
            
            if (m_traveledDistance >= range)
                Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject == owner)
                return;

            if (isPlayerProjectile)
            {
                if(other.gameObject.TryGetComponent<Enemy.Enemy>(out var hitEnemy))
                    Hit(hitEnemy);   
            }
            else 
            {
                if(other.gameObject.TryGetComponent<PlayerHealth>(out var playerHealth))
                    playerHealth.TakeDamage();
            }
            
            Destroy(gameObject);
        }

        public abstract void Hit(Enemy.Enemy hitEnemy);
    }
}
