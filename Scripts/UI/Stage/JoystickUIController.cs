using Domain;
using Manager;
using Manager.Data;
using UnityEngine;
using Util;

namespace UI.Stage
{
    public class JoystickUIController: MonoBehaviour
    {
        [SerializeField] private CanvasGroup cGroup;

        private void Start()
        {
            var eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
            cGroup.Visible(eControlType == EControlType.KeyBoard);
        }
    }
}