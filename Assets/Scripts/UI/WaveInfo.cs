using System;
using System.Collections;
using DG.Tweening;
using Enemy;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WaveInfo : MonoBehaviour
    {
        [Header("InfoSprites")] 
        [SerializeField] private Sprite[] waveInfoSprite;

        [SerializeField] private Image infoImage;
        [SerializeField] private float yOffset;
        [SerializeField] private float transitionTime;
        [SerializeField] private float showTime;

        private Vector3 m_defaultPosition;
        private Vector3 m_showPosition;

        private void OnEnable()
        {
            EnemySpawner.WaveStarts += ShowNextWaveStart;
        }

        private void OnDisable()
        {
            EnemySpawner.WaveStarts -= ShowNextWaveStart;
        }

        private void Awake()
        {
            m_defaultPosition = infoImage.transform.position;
            m_showPosition = m_defaultPosition - new Vector3(0, yOffset, 0);
        }

        private void ShowNextWaveStart(int newWaveId)
        {
            Debug.Log("Show next Wave");
            if (newWaveId == waveInfoSprite.Length)
            {
                // Handle win
                return;
            }
            
            infoImage.sprite = waveInfoSprite[newWaveId];
            infoImage.gameObject.SetActive(true);

            var fadeSequence = DOTween.Sequence();
            fadeSequence.Append(infoImage.DOColor(Color.white, transitionTime).SetOptions(true).SetEase(Ease.OutCirc));
            fadeSequence.Append(infoImage.DOColor(new Color(1, 1, 1, 0), transitionTime).SetOptions(true)
                .SetEase(Ease.InCirc).SetDelay(showTime));
        }
    }
}
