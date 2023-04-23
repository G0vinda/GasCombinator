using System;
using UnityEngine;

namespace Projectile
{
    public class MagicProjectile : Projectile
    {
        private void Update()
        {
            Move();
        }

        public override void Hit(Enemy.Enemy hitEnemy)
        {
            // meant to be empty
        }
    }
}