using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject inGameUI;

        public void Toggle()
        {      
            inGameUI.SetActive(gameObject.activeSelf);
            gameObject.SetActive(!gameObject.activeSelf);

            Time.timeScale = gameObject.activeSelf ? 0 : 1;
        }

        public void GoToCredits()
        {
            SceneManager.LoadScene("CreditScene");
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene("MainMenuScene");
        }
        
        
    }
}
