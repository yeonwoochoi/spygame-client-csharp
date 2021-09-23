using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Base;
using Domain;
using Event;
using Http;
using Manager;
using Manager.Data;
using StageScripts;
using UI.Popup.MainScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainScripts
{
    public class MainSceneController: BaseSceneController
    {
        #region Private Variables

        [SerializeField] private Button[] chapterButtons;
        private bool isSet = false;
        
        #endregion

        #region Event Methods

        private void Awake()
        {
            Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Quit();   
            }
        }

        #endregion

        #region Getter

        private ChapterInfo GetChapterInfo(ChapterType chapterType)
        {
            return ChapterManager.Instance.GetChapterInfo(chapterType);
        }
        
        private bool IsChapterClear(ChapterType chapterType)
        {
            var isClear = true;
            var stageScoreInfo = GlobalDataManager.Instance.Get<StageScoreManager>(GlobalDataKey.STAGE_SCORE).GetStageInfos(chapterType);
            foreach (var stageScore in stageScoreInfo.Where(stageScore => stageScore.score == 0))
            {
                isClear = false;
            }

            return isClear;
        }

        private bool IsChapterLocked(ChapterType chapterType)
        {
            if (chapterType == ChapterType.Chapter1) return false;
            var index = (int) chapterType;
            var prevChapter = (ChapterType) (index - 1);
            return !IsChapterClear(prevChapter);
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            if (isSet) return;
            SetButtonEvent();
            isSet = true;
        }

        private void SetButtonEvent()
        {
            for (var i = 0; i < chapterButtons.Length; i++)
            {
                SetButtonController(chapterButtons[i], (ChapterType) i);
            }
        }

        private void SetButtonController(Button button, ChapterType chapterType)
        {
            var controller = button.GetComponent<ChapterSelectPopupButtonController>();
            controller.SetChapterSelectButtons(GetChapterInfo(chapterType), IsChapterLocked(chapterType));
            button.onClick.AddListener(() =>
            {
                if (controller.GetIsLocked()) return;
                LoadChapterScene(chapterType);
            });
        }

        private void LoadChapterScene(ChapterType chapterType)
        {
            LoadingManager.Instance.currentType = MainSceneType.Main;
            LoadingManager.Instance.nextType = MainSceneType.Select;
            LoadingManager.Instance.loadingType = LoadingType.Normal;
            LoadingManager.Instance.chapterType = chapterType;

            StartCoroutine(StartLoadingAnimator(() =>
                {
                    nextScene = SceneNameManager.SceneNormalLoading;
                }, 
                () =>
                {
                    SceneManager.LoadScene(nextScene);
                }));
        }

        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion
    }
}