using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ImageChangeButtonController: MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite changedSprite;

        private bool isDefault;

        private bool IsDefault
        {
            get => isDefault;
            set
            {
                isDefault = value;
                image.sprite = isDefault ? defaultSprite : changedSprite;
            }
        }

        public void Init(bool isDefaultImg, bool isAuto = true)
        {
            IsDefault = isDefaultImg;
            if (isAuto)
            {
                GetComponent<Button>().onClick.AddListener(ChangeImage);
            }
        }

        private void ChangeImage()
        {
            IsDefault = !IsDefault;
        }
    }
}