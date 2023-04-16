using UnityEngine;

namespace Projectile
{
    public class IceProjectile : Projectile
    {
        [SerializeField] private float slowFactor;
        
        private void Update()
        {
            Move();
        }

        public override void Hit(Enemy.Enemy hitEnemy)
        {
            if (hitEnemy != null)
            {
                hitEnemy.TakeDamage(damage);
                hitEnemy.TakeSlow(slowFactor);
            }

            Destroy(gameObject);
        }
    }
}