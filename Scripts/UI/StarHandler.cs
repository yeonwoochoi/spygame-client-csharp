using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StarHandler: MonoBehaviour
    {
        [SerializeField] private Image left;
        [SerializeField] private Image middle;
        [SerializeField] private Image right;

        [SerializeField] private Sprite leftStarImage;
        [SerializeField] private Sprite middleStarImage;
        [SerializeField] private Sprite rightStarImage;
        
        [SerializeField] private Sprite leftStarEmptyImage;
        [SerializeField] private Sprite middleStarEmptyImage;
        [SerializeField] private Sprite rightStarEmptyImage;

        private void Start()
        {
            ResetStar();
        }

        public void ShowStars(int score)
        {
            if (score > 3) score = 3;
            if (score <= 0)
            {
                ResetStar();
                return;
            }

            if (score >= 1)
            {
                left.sprite = leftStarImage;
            }
            if (score >= 2)
            {
                middle.sprite = middleStarImage;
            }
            if (score >= 3)
            {
                right.sprite = rightStarImage;
            }
        }

        private void ResetStar()
        {
            left.sprite = leftStarEmptyImage;
            middle.sprite = middleStarEmptyImage;
            right.sprite = rightStarEmptyImage;
        }
    }
}