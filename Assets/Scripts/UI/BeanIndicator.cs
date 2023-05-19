using System;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BeanIndicator : MonoBehaviour
{
    [SerializeField] private Image[] BeanIndicatorImage;
    [SerializeField] private TextMeshProUGUI[] BeanInfoTexts;
    [SerializeField] private GameObject[] BeanInfoCards;

    private bool isBeanInfoShown = false;
    
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite greenSprite;


    private void Start()
    {
        foreach (var beanInfoTexts in BeanInfoTexts)
        {
            beanInfoTexts.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isBeanInfoShown)
            {
                HideBeanInfo();
            }
            else
            {
                ShowBeanInfo();
            }
        }
    }

    private void OnEnable()
    {
        Dragon.BeansChanged += UpdateBeanUI;
    }

    private void OnDisable()
    {
        Dragon.BeansChanged -= UpdateBeanUI;
    }

    private void ShowBeanInfo()
    {
        Debug.Log("Showing Bean Info!");
        foreach (var beanInfoCard in BeanInfoCards)
        {
            beanInfoCard.transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
        }

        isBeanInfoShown = true;
    }
    
     private void HideBeanInfo()
    {
        foreach (var beanInfoCard in BeanInfoCards)
        {
            beanInfoCard.transform.DOScale(new Vector3(0f, 1f, 1f), 0.5f);
        }

        isBeanInfoShown = false;
    }
    
    private void UpdateBeanUI(List<KeyValuePair<int, string>> beans)
    {
        foreach (var image in BeanIndicatorImage)
        {
            ResetBeanImage(image);
        }
        
     

        
        foreach (var beanInfoTexts in BeanInfoTexts)
        {
            beanInfoTexts.enabled = false;
        }
        
        var counter = 0;
        foreach (var keyValuePair in beans)
        {
            switch ((Bean.Bean.Type)keyValuePair.Key)
            {
                case Bean.Bean.Type.NEUTRAL:
                    break;
                case Bean.Bean.Type.RED:
                    SetBeanImage(BeanIndicatorImage[counter], redSprite);
                    break;
                case Bean.Bean.Type.BLUE:
                    SetBeanImage(BeanIndicatorImage[counter], blueSprite);
                    break;
                case Bean.Bean.Type.GREEN:
                    SetBeanImage(BeanIndicatorImage[counter], greenSprite);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            BeanInfoTexts[counter].text = keyValuePair.Value;
            BeanInfoTexts[counter].enabled = true;
            counter++;
        }
    }

    private void SetBeanImage(Image img, Sprite sprite)
    {
        img.sprite = sprite;
        var color = img.color;
        color.a = 1;
        img.color = color;
    }

    private void ResetBeanImage(Image img)
    {
        var color = img.color;
        color.a = 0;
        img.color = color;
    }
}
