using Enemy;

namespace Projectile
{
    public class FireProjectile : Projectile
    {
        private void Update()
        {
            Move();
        }

        public override void Hit(IEnemy enemy)
        {
            throw new System.NotImplementedException();
        }
    }
}