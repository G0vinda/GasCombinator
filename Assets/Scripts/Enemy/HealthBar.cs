using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBarFill;
        [SerializeField] private float healthBarAnimationTime;

        private Tweener m_healthBarAnimationTween;
        private Camera m_camera;

        private void Start()
        {
            m_camera = Camera.main;
        }

        // healthAmount needs to be between 0.0 and 1.0
        public void UpdateHealth(float newHealthAmount)
        {
            m_healthBarAnimationTween?.Kill();

            DOVirtual.Float(healthBarFill.fillAmount, newHealthAmount, healthBarAnimationTime,
                value => healthBarFill.fillAmount = value).SetEase(Ease.OutCirc);
        }

        private void Update()
        {
            transform.rotation = Quaternion.LookRotation(transform.position - m_camera.transform.position);
        }
    }
}
