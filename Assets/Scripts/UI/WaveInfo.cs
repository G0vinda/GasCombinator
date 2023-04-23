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
        [SerializeField] private float moveTime;
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


            infoImage.transform.DOMove(m_showPosition, moveTime).SetEase(Ease.OutCirc).SetLoops(2, LoopType.Yoyo);
        }
    }
}
