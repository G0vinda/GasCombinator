using System;
using Cinemachine;
using Enemy;
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
        
        public virtual void Init(GameObject newOwner)
        {
            owner = newOwner;
        }
        
        protected virtual void Move()
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
            Enemy.Enemy hitEnemy = other.gameObject.GetComponent<Enemy.Enemy>();
            Hit(hitEnemy);
        }

        public abstract void Hit(Enemy.Enemy hitEnemy);
    }
}
