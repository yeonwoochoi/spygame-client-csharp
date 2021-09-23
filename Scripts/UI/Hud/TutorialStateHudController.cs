using System;
using Domain;
using Event;
using Manager;
using Manager.Data;
using TutorialScripts;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Hud
{
    public class TutorialStateHudController: MonoBehaviour
    {
        #region Private Variables
        
        private CanvasGroup cGroup;

        private bool isSet = false;
        private bool isTutorial;

        #endregion

        #region Event Methods

        private void Start()
        {
            cGroup = GetComponent<CanvasGroup>();
            isTutorial = !GlobalDataManager.Instance.HasKey(GlobalDataKey.TUTORIAL);
            cGroup.Visible(!isTutorial);

            TutorialSceneController.StartTutorialGameEvent += ActivateStateHud;
        }

        private void OnDisable()
        {
            TutorialSceneController.StartTutorialGameEvent -= ActivateStateHud;
        }

        #endregion

        #region Private Methods

        private void ActivateStateHud(object _, StartTutorialGameEventArgs e)
        {
            if (isSet) return;
            cGroup.Visible();
            isSet = true;
        }

        #endregion
    }
}