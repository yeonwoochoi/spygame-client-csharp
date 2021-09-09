using Domain;
using Manager;
using Manager.Data;
using UnityEngine;
using Util;

namespace UI.StageScripts.Hud
{
    public class JoystickHudController: MonoBehaviour
    {
        #region Private Variable

        [SerializeField] private CanvasGroup cGroup;

        #endregion

        #region Event Method

        private void Start()
        {
            var eControlType = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL).eControlType;
            cGroup.Visible(eControlType == EControlType.KeyBoard);
        }

        #endregion
    }
}