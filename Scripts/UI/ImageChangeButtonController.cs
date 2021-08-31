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

        #region Public Method

        public void Init(bool isDefaultImg, bool isAuto = true)
        {
            SetImage(isDefaultImg);
            if (isAuto)
            {
                GetComponent<Button>().onClick.AddListener(ChangeImage);
            }
        }

        #endregion

        #region Private Method

        private void ChangeImage()
        {
            SetImage(!isDefault);
        }

        private void SetImage(bool flag)
        {
            isDefault = flag;
            image.sprite = isDefault ? defaultSprite : changedSprite;
        }

        #endregion
    }
}