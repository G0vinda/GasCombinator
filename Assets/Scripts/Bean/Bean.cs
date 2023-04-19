using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core;
using UnityEngine;
using Player;

namespace Bean
{
    public class Bean : MonoBehaviour
    {
        public enum Type
        {
            EXTRA_SHOTS,
            BONUS_WALKING_SPEED,
            SHOT_SLOW,
            SHOT_SPREAD
        }

        [SerializeField] private Type type;
        [SerializeField] private float walkingSpeedBonus;
        [SerializeField] private float shotSlow;

        public Type BeanType => type;
        public float WalkingSpeedBonus => walkingSpeedBonus;
        public float ShotSlow => shotSlow;
    }
}