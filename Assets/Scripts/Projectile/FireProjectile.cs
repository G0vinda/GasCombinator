using UnityEngine;

namespace Projectile
{
    public class FireProjectile : Projectile
    {
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private float explosionRadius; // this radius is only for the hitbox 
        
        private void Update()
        {
            Move();
        }

        public override void Hit(Enemy.Enemy empty) // this parameter is not needed in this override
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            var colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var hitCollider in colliders)
            {
                var hitEnemy = hitCollider.GetComponent<Enemy.Enemy>();
                
                if(hitEnemy != null)
                    hitEnemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}