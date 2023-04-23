using UnityEngine;

namespace Projectile
{
    public class PoisonProjectile : Projectile
    {
        public float poisonHitTime = 1;
        public int numberOfPoisonHits = 6;
        
        
        private void Update()
        {
            Move();
        }

        public override void Hit(Enemy.Enemy hitEnemy)
        {
            if (hitEnemy != null)
            {
                hitEnemy.TakePoison(damage, poisonHitTime, numberOfPoisonHits);
                hitEnemy.TakeSlow(slowEffect);
            }

            Destroy(gameObject);
        }
    }
}