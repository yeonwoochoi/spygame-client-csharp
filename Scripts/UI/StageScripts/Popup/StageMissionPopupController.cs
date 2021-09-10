using System.Collections;
using Event;
using Manager;
using StageScripts;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace UI.StageScripts.Popup
{
    public class StageMissionPopupController: BasePopupBehavior
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
            StageSceneSpawner.OpenStageMissionPopupEvent += OpenStageMissionPopup;
            okButton.onClick.AddListener(OnClickOKButton);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StageSceneSpawner.OpenStageMissionPopupEvent -= OpenStageMissionPopup;
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