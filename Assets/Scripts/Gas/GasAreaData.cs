using UnityEngine;

namespace Gas
{
    [CreateAssetMenu(fileName = "NewGasAreaData", menuName = "ScriptableObjects/GasAreaData", order = 1)]
    public class GasAreaData : ScriptableObject
    {
        public Projectile.Projectile projectilePrefab;
        public Color floorColor;
    }
}
