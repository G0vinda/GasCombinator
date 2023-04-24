using System;
using System.Collections;
using Bean;
using TMPro;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int startLives;
        [Tooltip("Player will be invincible after hit in seconds.")]
        [SerializeField] private float invincibilityTime;
        [SerializeField] private TextMeshProUGUI liveTextElement;
        [SerializeField] private DamageOverlay damageOverlay;

        private int m_currentLives;
        private PlayerController m_playerController;
        private WaitForSeconds m_invincibilityPause;
        private bool m_isInvincible;

        public bool IncreaseHealth(int amount = 1)
        {
            m_currentLives += amount;
            if (m_currentLives > 20)
            {
                m_currentLives = 20;
            }
            UpdateLiveText();
            return true;
        }
        
        private void Awake()
        {
            m_currentLives = startLives;
            UpdateLiveText();
            m_playerController = GetComponent<PlayerController>();
            m_invincibilityPause = new WaitForSeconds(invincibilityTime);
        }

        private void OnCollisionEnter(Collision other)
        {
            if(m_isInvincible)
                return;
            
            Debug.Log("Collision entered");
            
            if (other.gameObject.GetComponent<Enemy.Enemy>() != null)
            {
                TakeDamage();
            }
        }

        public void TakeDamage(int amount = 1)
        {
            damageOverlay.Show();
            if (BlueBean.Attributes.ActivatedEffects.Contains(BlueBean.Effect.AvoidDamage))
            {
                var hit = Random.Range(0.0f, 1.0f);
                var chance = BlueBean.Attributes.AvoidDamageChancePerBean * BlueBean.Attributes.Collected;
                Debug.Log(hit + " | " + chance + " => " + (hit <= chance ? " Damage avoid success!" : "Damage avoid fail!"));
                if (hit <= chance) return;
            }
            
            m_currentLives -= amount;
            UpdateLiveText();
            if (m_currentLives == 0)
            {
                m_playerController.enabled = false;
                Time.timeScale = 0f;
            }

            m_isInvincible = true;
            StartCoroutine(InvincibleTimer());
        }

        private IEnumerator InvincibleTimer()
        {
            yield return m_invincibilityPause;
            m_isInvincible = false;
        }

        private void UpdateLiveText()
        {
            liveTextElement.text = $"Lives: {m_currentLives}";
        }
    }
}