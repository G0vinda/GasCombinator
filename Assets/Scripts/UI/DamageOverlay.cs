using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DamageOverlay : MonoBehaviour
    {
        [SerializeField] private float fadeOutTime;
        
        private Image m_image;
        private Tweener m_fadeOutTweener;

        private void Awake()
        {
            m_image = GetComponent<Image>();
        }

        public void Show()
        {
            m_fadeOutTweener?.Kill();
            var imageColor = m_image.color;
            imageColor.a = 1f;
            m_image.color = imageColor;

            var transparentColor = imageColor;
            transparentColor.a = 0f;

            m_fadeOutTweener =
                DOVirtual.Color(imageColor, transparentColor, fadeOutTime, value => m_image.color = value);
        }
    }
}
