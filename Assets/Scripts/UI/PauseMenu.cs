using UnityEngine;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject inGameUI;

        public void Toggle()
        {
            Debug.Log("Toggle got called");
            inGameUI.SetActive(gameObject.activeSelf);
            gameObject.SetActive(!gameObject.activeSelf);

            Time.timeScale = gameObject.activeSelf ? 0 : 1;
        }
    }
}
