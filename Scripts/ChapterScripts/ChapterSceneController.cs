﻿using Domain;
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
        [SerializeField] private CanvasGroup loadingCanvasGroup;
        [SerializeField] private StagePlayReadyPopupController stagePlayReadyPopupController;
        [SerializeField] private Button backButton;
        [SerializeField] private Text chapterText;
        [SerializeField] private Transform parent;

        private Chapter currentChapter;
        private ChapterMapController chapterMapController;

        protected override void Start()
        {
            base.Start();
            backButton.onClick.AddListener(OnClickBackBtn);
            SetChapterMap();
        }

        private void SetChapterMap()
        {
            currentChapter = LoadingManager.Instance.chapter;

            var currentChapterMap = Instantiate(currentChapter.mapPrefab, parent.position, Quaternion.identity);
            currentChapterMap.transform.SetParent(parent);
            currentChapterMap.GetComponent<RectTransform>().localScale = Vector3.one;
            chapterMapController = currentChapterMap.GetComponent<ChapterMapController>();
            chapterMapController.SetStageButtonEvent(OnClickStageBtn);
            chapterMapController.SetButtonScore(type => GetCurrentStage(type).score);
            chapterText.text = $"{currentChapter.chapterType}";
            loadingCanvasGroup.Visible(false);
        }
        
        private void OnClickStageBtn(StageType stageType)
        {
            stagePlayReadyPopupController.OpenStagePlayReadyPopup(GetCurrentStage(stageType), LoadStageScene);
        }

        private void LoadStageScene(StageType stageType)
        {
            LoadingManager.Instance.CurrentType = MainSceneType.Select;
            LoadingManager.Instance.NextType = MainSceneType.Play;
            LoadingManager.Instance.LoadingType = LoadingType.Normal;
            LoadingManager.Instance.ChapterType = currentChapter.chapterType;
            LoadingManager.Instance.StageType = stageType;
            LoadingManager.Instance.stage = GetCurrentStage(stageType);
            
            StartCoroutine(StartLoadingAnimator(() =>
                {
                    nextScene = SceneNameManager.SCENE_NORMAL_LOADING;
                }, 
                () =>
                {
                    SceneManager.LoadScene(nextScene);
                }));
        }

        private void OnClickBackBtn()
        {
            LoadingManager.Instance.CurrentType = MainSceneType.Select;
            LoadingManager.Instance.NextType = MainSceneType.Main;
            LoadingManager.Instance.LoadingType = LoadingType.Normal;

            StartCoroutine(StartLoadingAnimator(
                () => nextScene = SceneNameManager.SCENE_NORMAL_LOADING,
                () => SceneManager.LoadScene(nextScene)));
        }
        
        private Stage GetCurrentStage(StageType currentStageType)
        {
            // TODO : 해당되는 stage 없을 땐 default로 Chapter1 stage1 받아옴 (수정 필요)
            var result = currentChapter.stages[0];
            foreach (var stage in currentChapter.stages)
            {
                if (stage.chapterType == currentChapter.chapterType && stage.stageType == currentStageType)
                {
                    result = stage;
                }
            }
            return result;
        }
    }
}