using UnityEngine;
using UnityEngine.UI;

namespace UI.Alert
{
    public class AlertButtonController: MonoBehaviour
    {
        [SerializeField] private Text buttonText;
        [SerializeField] private Button button;

        #region Getter

        public Text GetButtonText()
        {
            return buttonText;
        }

        public Button GetButton()
        {
            return button;
        }

        #endregion

        #region Setter

        public void Active(bool flag) {
            gameObject.SetActive(flag);
        }

        #endregion
    }
}