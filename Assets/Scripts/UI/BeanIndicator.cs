using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class BeanIndicator : MonoBehaviour
    {
        [SerializeField] private Image[] BeanIndicatorImage;

        [SerializeField] private Sprite redSprite;
        [SerializeField] private Sprite blueSprite;
        [SerializeField] private Sprite greenSprite;


        private void OnEnable()
        {
            Dragon.BeansChanged += UpdateBeanUI;
        }

        private void OnDisable()
        {
            Dragon.BeansChanged -= UpdateBeanUI;
        }

        private void UpdateBeanUI(List<KeyValuePair<int, string>> beans)
        {
            foreach (var image in BeanIndicatorImage)
            {
                ResetBeanImage(image);
            }
        
            int counter = 0;
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
}
