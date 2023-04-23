using System;
using UnityEngine;

namespace Enemy
{
    public class Tank : Enemy
    {
        [SerializeField] private float dieTime;

        private void Update()
        {
            if (!ProcessFreeze())
                return;
            
            FollowPlayer();
        }

        protected override void Die()
        {
            NavMeshAgent.enabled = false;
            Destroy(gameObject, dieTime);
            enabled = false;
        }
    }
}