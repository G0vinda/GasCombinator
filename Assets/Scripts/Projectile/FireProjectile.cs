using Enemy;

namespace Projectile
{
    public class FireProjectile : Projectile
    {
        private void Update()
        {
            Move();
        }

        public override void Hit()
        {
            throw new System.NotImplementedException();
        }
    }
}