using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class PlayerHealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBarForeground;

        public void UpdateBar(float fillAmount)
        {
            healthBarForeground.fillAmount = fillAmount;
        }
    }
}
