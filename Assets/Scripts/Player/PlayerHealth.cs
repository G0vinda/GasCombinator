using System;
using System.Collections;
using Bean;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int startLives;
        [Tooltip("Player will be invincible after hit in seconds.")]
        [SerializeField] private float invincibilityTime;
        [SerializeField] private DamageOverlay damageOverlay;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float hitScreenShakeTime;
        [SerializeField] private float hitScreenShakeIntensity;
        [SerializeField] private PlayerHealthBar playerHealthBar;
        
            
        private int m_currentLives;
        private WaitForSeconds m_invincibilityPause;
        private bool m_isInvincible;

        public bool IncreaseHealth(int amount = 1)
        {
            m_currentLives += amount;
            if (m_currentLives > startLives)
            {
                m_currentLives = startLives;
            }
            
            playerHealthBar.UpdateBar((float) m_currentLives / startLives);
            return true;
        }
        
        private void Awake()
        {
            m_currentLives = startLives;
            playerHealthBar.UpdateBar((float) m_currentLives / startLives);
            m_invincibilityPause = new WaitForSeconds(invincibilityTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if(m_isInvincible)
                return;
            
            Debug.Log("Collision entered");
            
            if (other.gameObject.TryGetComponent<Enemy.Enemy>(out var enemy))
            {
                TakeDamage(enemy.damage);
            }
        }

        public void TakeDamage(int amount = 1)
        {
            damageOverlay.Show();
            var cameraNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            DOVirtual.Float(hitScreenShakeIntensity, 0f, hitScreenShakeTime,
                value => cameraNoise.m_AmplitudeGain = value).SetEase(Ease.InExpo);
            
            if (BlueBean.Attributes.ActivatedEffects.Contains(BlueBean.Effect.AvoidDamage))
            {
                var hit = Random.Range(0.0f, 1.0f);
                var chance = BlueBean.Attributes.AvoidDamageChancePerBean * BlueBean.Attributes.Collected;
                Debug.Log(hit + " | " + chance + " => " + (hit <= chance ? " Damage avoid success!" : "Damage avoid fail!"));
                if (hit <= chance) return;
            }
            
            m_currentLives -= amount;
            playerHealthBar.UpdateBar((float) m_currentLives / startLives);
            if (m_currentLives <= 0)
            {
                SceneManager.LoadScene("LoseScene");
            }

            m_isInvincible = true;
            StartCoroutine(InvincibleTimer());
        }

        private IEnumerator InvincibleTimer()
        {
            yield return m_invincibilityPause;
            m_isInvincible = false;
        }
    }
}