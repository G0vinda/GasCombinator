using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core;
using UnityEngine;
using Player;
using UnityEngine.Serialization;

namespace Bean
{
    public class Bean : MonoBehaviour
    {
        public enum Type
        {
            NEUTRAL,
            RED,
            BLUE,
            GREEN
        }
        
        public enum Effect
        {
            EXTRA_SHOTS,
            BONUS_WALKING_SPEED,
            SHOT_SLOW,
            SHOT_SPREAD,
            SHOT_DAMAGE
        }

        [SerializeField] private Type type;
        [SerializeField] private Effect effect;
        [SerializeField] private float walkingSpeedBonus;
        [SerializeField] private float shotSlow;
        [FormerlySerializedAs("shotDamage")] [SerializeField] private float shotDamageMultiplier;

        public Type BeanType => type;
        public Effect BeanEfect => effect;
        public float WalkingSpeedBonus => walkingSpeedBonus;
        public float ShotSlow => shotSlow;
        public float ShotDamageMultiplier => shotDamageMultiplier;
    }
}