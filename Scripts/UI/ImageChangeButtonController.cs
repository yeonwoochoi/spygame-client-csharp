using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ImageChangeButtonController: MonoBehaviour
    {
        #region Private Variables

        [SerializeField] private Image image;
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite changedSprite;

        private bool isDefault;

        #endregion

        private bool IsDefault
        {
            get => isDefault;
            set
            {
                isDefault = value;
                image.sprite = isDefault ? defaultSprite : changedSprite;
            }
        }

        #region Public Method

        public void Init(bool isDefaultImg, bool isAuto = true)
        {
            IsDefault = isDefaultImg;
            if (isAuto)
            {
                GetComponent<Button>().onClick.AddListener(ChangeImage);
            }
        }

        #endregion

        #region Private Method

        private void ChangeImage()
        {
            IsDefault = !IsDefault;
        }

        #endregion
    }
}