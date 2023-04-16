using UnityEngine;

namespace Enemy
{
    public interface IEnemy
    {
        abstract void Move();
        abstract void TakeDamage(float damage);
    }
}