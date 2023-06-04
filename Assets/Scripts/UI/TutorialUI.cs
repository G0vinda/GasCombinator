using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
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
        FadeInPage1();
    }

    public void SwitchToPage2()
    {
        FadeOutPage(page1, () => { page1.gameObject.SetActive(false); });
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
        }).SetEase(Ease.OutSine).OnComplete(() => { page2.gameObject.SetActive(true); });
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
