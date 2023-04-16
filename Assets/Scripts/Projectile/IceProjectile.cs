using UnityEngine;

namespace Projectile
{
    public class IceProjectile : Projectile
    {
        private void Update()
        {
            Move();
        }

        public override void Hit(Enemy.Enemy hitEnemy)
        {
            if (hitEnemy != null)
            {
                hitEnemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}