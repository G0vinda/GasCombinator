using System;
using UnityEngine;

namespace Enemy
{
    public class Tank : Enemy
    {
        private void Update()
        {
            if (!ProcessFreeze())
                return;
            
            FollowPlayer();
        }
    }
}