using System;
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
        
        public Color DragonColor => dragonColor;
        
        private float m_traveledDistance;

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
            Enemy.Enemy hitEnemy = other.gameObject.GetComponent<Enemy.Enemy>();
            Hit(hitEnemy);
        }

        public abstract void Hit(Enemy.Enemy hitEnemy);
    }
}
