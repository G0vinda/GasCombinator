using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using Player;
using UnityEngine;

namespace Player
{
    public class Projectile : MonoBehaviour, IProjectile
    {
        public enum Type
        {
            NONE,
            FIRE,
            ICE
        }

        [SerializeField] private Color dragonColor;
        [SerializeField] private float speed;
        [SerializeField] private Type type;
        [SerializeField] private float range;
        
        public Color DragonColor => dragonColor;
        
        private float m_traveledDistance;

        void Update()
        {
            Move();
            if (m_traveledDistance >= range)
                Destroy(gameObject);
        }

        public virtual void Move()
        {
            var oldPosition = new Vector3(transform.position.x, transform.position.y, transform.position.x);
            transform.Translate(transform.forward * (speed * Time.deltaTime), Space.World);
            m_traveledDistance +=  Vector3.Distance(oldPosition, transform.position);
        }

        public virtual void Hit(IEnemy enemy)
        {

        }
    }
}
