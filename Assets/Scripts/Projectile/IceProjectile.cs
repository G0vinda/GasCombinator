using UnityEngine;

namespace Projectile
{
    public class IceProjectile : Projectile
    {
        [SerializeField] private float freezeTime;
        
        private void Update()
        {
            Move();
        }

        public override void Hit(Enemy.Enemy hitEnemy)
        {
            if (hitEnemy != null)
            {
                hitEnemy.TakeDamage(damage);
                hitEnemy.TakeSlow(slowEffect);
                hitEnemy.Freeze(freezeTime);
            }

            Destroy(gameObject);
        }
    }
}