using Enemy;
using UnityEngine;

namespace Projectile
{
    public abstract class Projectile : MonoBehaviour
    {
        [SerializeField] private Color dragonColor;
        [SerializeField] private float speed;
        [SerializeField] private float range;
        
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

        public abstract void Hit();
    }
}
