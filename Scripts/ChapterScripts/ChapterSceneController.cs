using System.Collections.Generic;
using Base;
using Domain;
using MainScripts;
using Manager;
using Manager.Data;
using UI.Popup.ChapterScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util;

namespace ChapterScripts
{
    public class ChapterSceneController: BaseSceneController
    {
        #region Private Variables

        [SerializeField] private GameObject[] chapterMapPrefabs;
        [SerializeField] private Sprite[] stageMapPreviewSprites;
        [SerializeField] private CanvasGroup loadingCanvasGroup;
        [SerializeField] private StagePlayReadyPopupController stagePlayReadyPopupController;
        [SerializeField] private Button backButton;
        [SerializeField] private Text chapterText;
        [SerializeField] private Transform parent;

        private ChapterInfo currentChapterInfo;
        private ChapterType currentChapterType;
        private ChapterButtonController chapterButtonController;
        private StageScoreManager stageScoreManager;

        private bool isSet = false;
        
        #endregion

        #region Event Method

        protected override void Start()
        {
            base.Start();
            backButton.onClick.AddListener(OnClickBackBtn);
            SetChapterMap();
        }

        #endregion

        #region Getter

        // TODO (map preview sprites) : 하드 코딩이니 서버 생기면 수정
        private Sprite GetStageMapPreviewSprites(StageType stageType)
        {
            var chapterIndex = (int) currentChapterInfo.chapterType;
            var stageIndex = (int) stageType;
            var index = chapterIndex * 6 + stageIndex;
            return stageMapPreviewSprites[index];
        }

        #endregion

        #region Private Methods

        private void SetChapterMap()
        {
            if (isSet) return;
            currentChapterType = LoadingManager.Instance.chapterType;
            currentChapterInfo = ChapterManager.Instance.GetChapterInfo(currentChapterType);

            stageScoreManager = GlobalDataManager.Instance.Get<StageScoreManager>(GlobalDataKey.STAGE_SCORE);
            
            var index = (int) currentChapterType;

            // map setting
            var currentChapterMap = Instantiate(chapterMapPrefabs[index], parent.position, Quaternion.identity);
            currentChapterMap.transform.SetParent(parent);
            currentChapterMap.GetComponent<RectTransform>().localScale = Vector3.one;
            
            // map controller setting
            chapterButtonController = currentChapterMap.GetComponent<ChapterButtonController>();
            chapterButtonController.SetStageButtonEvent(OnClickStageBtn, currentChapterType);
            chapterText.text = $"{currentChapterInfo.title}";
            
            // chapter scene setting is done
            loadingCanvasGroup.Visible(false);
            isSet = true;
        }
        
        private void OnClickStageBtn(StageType stageType)
        {
            stagePlayReadyPopupController.OpenStagePlayReadyPopup(
                GetCurrentStage(stageType),
                stageScoreManager.GetStageScore(currentChapterType, stageType),
                GetStageMapPreviewSprites(stageType),
                LoadStageScene
                );
        }

        private void LoadStageScene(StageType stageType)
        {
            LoadingManager.Instance.currentType = MainSceneType.Select;
            LoadingManager.Instance.nextType = MainSceneType.Play;
            LoadingManager.Instance.loadingType = LoadingType.Normal;
            LoadingManager.Instance.chapterType = currentChapterType;
            LoadingManager.Instance.stageType = stageType;
            
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
        
        private StageInfo GetCurrentStage(StageType stageType)
        {
            return ChapterManager.Instance.GetStageInfo(currentChapterType, stageType);
        }

        #endregion
    }
}