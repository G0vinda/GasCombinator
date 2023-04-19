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
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Player.Dragon>(out var dragon))
            {
                ApplyEffect(dragon);
                dragon.RemoveBean(this);
                Destroy(gameObject);
            }
        }

        protected virtual void ApplyEffect(Player.Dragon target)
        {
            target.IncreaseHealth();
            Debug.Log("Applied Effect of " + gameObject.name + " to " + target.gameObject.name);
        }
    }
}