using UnityEngine;

namespace Enemy
{
    public class Mushroom : Enemy
    {
        protected override void Die()
        {
            Destroy(gameObject);
        }
    }
}