using System;
using DG.Tweening;
using UnityEngine;

namespace Bean
{
    public class BeanHover : MonoBehaviour
    {
        private void Start()
        {
            transform.DOMoveY(transform.position.y + 1, 1).SetEase(Ease.OutCirc).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
