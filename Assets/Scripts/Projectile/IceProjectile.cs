using UnityEngine;

namespace Projectile
{
    public class IceProjectile : Projectile
    {
        [SerializeField] private float freezeTime;
        public float unfrozenSpeedMultiplier = 1.0f;
        
        private void Update()
        {
            Move();
        }

        public override void Hit(Enemy.Enemy hitEnemy)
        {
            if (hitEnemy != null)
            {
                hitEnemy.TakeDamage(damage);
                hitEnemy.Freeze(freezeTime, unfrozenSpeedMultiplier);
            }

            Destroy(gameObject);
        }
    }
}