using UnityEngine;

namespace Projectile
{
    public class PoisonProjectile : Projectile
    {
        private void Update()
        {
            Move();
        }

        public override void Hit(Enemy.Enemy hitEnemy)
        {
            if (hitEnemy != null)
            {
                hitEnemy.TakePoison(damage);
                hitEnemy.TakeSlow(slowEffect);
            }

            Destroy(gameObject);
        }
    }
}