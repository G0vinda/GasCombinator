using System;
using System.Collections;
using TMPro;
using UnityEngine;
<<<<<<< Updated upstream
=======
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
>>>>>>> Stashed changes

namespace Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private int startLives;
        [Tooltip("Player will be invincible after hit in seconds.")]
        [SerializeField] private float invincibilityTime;
        [SerializeField] private TextMeshProUGUI liveTextElement;
        
        private int m_currentLives;
        private PlayerController m_playerController;
        private WaitForSeconds m_invincibilityPause;
        private bool m_isInvincible;

        public bool IncreaseHealth(int amount = 1)
        {
            m_currentLives += amount;
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
                m_currentLives--;
                UpdateLiveText();
                if (m_currentLives == 0)
                {
                    m_playerController.enabled = false;
                    Time.timeScale = 0f;
                }

                m_isInvincible = true;
                StartCoroutine(InvincibleTimer());
            }
<<<<<<< Updated upstream
=======
            
            m_currentLives -= amount;
            UpdateLiveText();
            if (m_currentLives <= 0)
            {
                SceneManager.LoadScene("MainMenuScene", LoadSceneMode.Single);
            }

            m_isInvincible = true;
            StartCoroutine(InvincibleTimer());
>>>>>>> Stashed changes
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