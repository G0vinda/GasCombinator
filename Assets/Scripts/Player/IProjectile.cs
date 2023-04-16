using Enemy;
using UnityEngine;

namespace Player
{
    public interface IProjectile
    {
        abstract void Move();
        abstract void Hit(IEnemy enemy);
    }
}