using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class TutorialUI : MonoBehaviour
    {
        enum State
        {
            Page1,
            Page2,
            Fading
        }
        private State state = State.Page1;
        
        [SerializeField] private Image page1;
        [SerializeField] private Image page2;

        [SerializeField] private float fadeTime;
        

        void Start()
        {
            page2.gameObject.SetActive(false);
            FadeInPage1();
        }

        public void SwitchToPage1()
        {
            Debug.Log("Switch to Page 1 called");
            if (state == State.Page1)
            {
                Debug.Log("Switch to Page 1 cancelled");
                return;
            }

            state = State.Fading;
            FadeInPage1();
        }

        public void SwitchToPage2()
        {
            Debug.Log("Switch to Page 2 called");
            if (state == State.Page2)
            {
                Debug.Log("Switch to Page 2 cancelled");
                return;
            }
            
            state = State.Fading;
            FadeOutPage(page1, () => { page1.gameObject.SetActive(false);
                state = State.Page2;
            });
        }

        public void ExitTutorial()
        {
            FadeOutPage(page2, () =>
            {
                page2.gameObject.SetActive(false);
                SceneManager.LoadScene("LevelScene");
            });
        }

        private void FadeInPage1()
        {
            page1.gameObject.SetActive(true);
            var pageColor = page1.color;
            DOVirtual.Float(0, 1, fadeTime, value =>
            {
                pageColor.a = value;
                page1.color = pageColor;
            }).SetEase(Ease.OutSine).
            OnComplete(() => 
            { 
                page2.gameObject.SetActive(true);
                state = State.Page1;
            });
        }
    
        private void FadeOutPage(Image page, TweenCallback tweenCallback)
        {
            var pageColor = page.color;
            DOVirtual.Float(1, 0, fadeTime, value =>
            {
                pageColor.a = value;
                page.color = pageColor;
            }).SetEase(Ease.InSine).OnComplete(tweenCallback);
        }
    }
}
