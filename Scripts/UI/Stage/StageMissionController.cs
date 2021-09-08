﻿using System.Collections;
using Domain;
using Event;
using Manager;
using StageScripts;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.Stage
{
    public class StageMissionController: BasePopupBehavior
    {
        #region Private Variables

        [SerializeField] private CanvasGroup fakeLoadingCanvasGroup;
        [SerializeField] private Text missionText;
        [SerializeField] private Button okButton;

        #endregion

        #region Event Methods

        protected override void Start()
        {
            base.Start();
            StageSceneController.OpenStageMissionPopupEvent += OpenStageMissionPopup;
            okButton.onClick.AddListener(OnClickOKButton);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StageSceneController.OpenStageMissionPopupEvent -= OpenStageMissionPopup;
        }

        #endregion

        #region Private Methods

        private void OpenStageMissionPopup(object _, OpenStageMissionPopupEventArgs e)
        {
            fakeLoadingCanvasGroup.Visible(false);
            OnOpenPopup();
            var currentStage = ChapterManager.Instance.GetStageInfo(LoadingManager.Instance.chapterType,
                LoadingManager.Instance.stageType);
            StartCoroutine(ShowMissionText(currentStage.GetStageMissionText()));
        }

        private IEnumerator ShowMissionText(string mission)
        {
            yield return TypingComment(missionText, mission);
        }

        private void OnClickOKButton()
        {
            OnClosePopup();
        }

        #endregion
    }
}