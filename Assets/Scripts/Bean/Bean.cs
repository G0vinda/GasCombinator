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
        
        [SerializeField] private Type type;
        
        public Type BeanType => type;

    }
}