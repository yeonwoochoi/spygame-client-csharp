using Base;
using Domain;
using MainScripts;
using Manager;
using UI.Chapter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

namespace ChapterScripts
{
    public class ChapterSceneController: BaseSceneController
    {
        #region Private Variables
        
        [SerializeField] private CanvasGroup loadingCanvasGroup;
        [SerializeField] private StagePlayReadyPopupController stagePlayReadyPopupController;
        [SerializeField] private Button backButton;
        [SerializeField] private Text chapterText;
        [SerializeField] private Transform parent;

        private Chapter currentChapter;
        private ChapterButtonController chapterButtonController;
        
        #endregion

        #region Event Method

        protected override void Start()
        {
            base.Start();
            backButton.onClick.AddListener(OnClickBackBtn);
            SetChapterMap();
        }

        #endregion

        #region Private Methods

        private void SetChapterMap()
        {
            currentChapter = LoadingManager.Instance.chapter;

            // map setting
            var currentChapterMap = Instantiate(currentChapter.mapPrefab, parent.position, Quaternion.identity);
            currentChapterMap.transform.SetParent(parent);
            currentChapterMap.GetComponent<RectTransform>().localScale = Vector3.one;
            
            // map controller setting
            chapterButtonController = currentChapterMap.GetComponent<ChapterButtonController>();
            chapterButtonController.SetStageButtonEvent(OnClickStageBtn);
            chapterButtonController.SetButtonScore(stageType => GetCurrentStage(stageType).score);
            chapterText.text = $"{currentChapter.chapterType}";
            
            // chapter scene setting is done
            loadingCanvasGroup.Visible(false);
        }
        
        private void OnClickStageBtn(StageType stageType)
        {
            stagePlayReadyPopupController.OpenStagePlayReadyPopup(GetCurrentStage(stageType), LoadStageScene);
        }

        private void LoadStageScene(StageType stageType)
        {
            LoadingManager.Instance.currentType = MainSceneType.Select;
            LoadingManager.Instance.nextType = MainSceneType.Play;
            LoadingManager.Instance.loadingType = LoadingType.Normal;
            LoadingManager.Instance.chapterType = currentChapter.chapterType;
            LoadingManager.Instance.stageType = stageType;
            LoadingManager.Instance.stage = GetCurrentStage(stageType);
            
            StartCoroutine(StartLoadingAnimator(() =>
                {
                    nextScene = SceneNameManager.SceneNormalLoading;
                }, 
                () =>
                {
                    SceneManager.LoadScene(nextScene);
                }));
        }

        private void OnClickBackBtn()
        {
            LoadingManager.Instance.currentType = MainSceneType.Select;
            LoadingManager.Instance.nextType = MainSceneType.Main;
            LoadingManager.Instance.loadingType = LoadingType.Normal;

            StartCoroutine(StartLoadingAnimator(
                () => nextScene = SceneNameManager.SceneNormalLoading,
                () => SceneManager.LoadScene(nextScene)));
        }
        
        private Stage GetCurrentStage(StageType currentStageType)
        {
            Stage result = null;
            foreach (var stage in currentChapter.stages)
            {
                if (stage.chapterType == currentChapter.chapterType && stage.stageType == currentStageType)
                {
                    result = stage;
                }
            }

            if (result == null)
            {
                // TODO (Error) : There is no stage data
            }
            return result;
        }

        #endregion
    }
}