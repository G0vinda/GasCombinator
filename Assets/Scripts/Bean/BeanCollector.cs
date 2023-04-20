using System;
using Player;
using UnityEngine;

namespace Bean
{
    public class BeanCollector : MonoBehaviour
    {
        [SerializeField] private Dragon dragon;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Bean>(out var bean))
            {
                dragon.ConsumeBean(bean);
            }
        }
    }
}
