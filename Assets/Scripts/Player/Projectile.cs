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

        [SerializeField] private Color dragonColor = Color.red; 
        public Color DragonColor => dragonColor;
        [SerializeField] private float speed = 8.5f;
        [SerializeField] private Type type = Type.NONE;
        [SerializeField] private float range = 600.0f;
        private float m_traveledDistance = 0.0f;
        
        private void Awake()
        {
            Destroy(gameObject, 10f);
        }

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
