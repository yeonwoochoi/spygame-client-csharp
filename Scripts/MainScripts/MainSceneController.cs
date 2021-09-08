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
using UI.Main;
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
            var soundManager = GlobalDataManager.Instance.Get<SoundManager>(GlobalDataKey.SOUND);
            if (soundManager == null)
            {
                var initSoundManager = SoundManager.Create();
                GlobalDataManager.Instance.Set(GlobalDataKey.SOUND, initSoundManager);
            }

            var eControlManager = GlobalDataManager.Instance.Get<EControlManager>(GlobalDataKey.ECONTROL);
            if (eControlManager == null)
            {
                var initEControlManager = EControlManager.Create();
                GlobalDataManager.Instance.Set(GlobalDataKey.ECONTROL, initEControlManager);
            }
            
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
        
        private bool IsClear(ChapterType chapterType)
        {
            var isClear = true;
            var stageScoreInfo = GlobalDataManager.Instance.Get<StageScoreManager>(GlobalDataKey.STAGE_SCORE).GetStageInfos(chapterType);
            foreach (var stageScore in stageScoreInfo.Where(stageScore => stageScore.score == 0))
            {
                isClear = false;
            }

            return isClear;
        }

        private bool IsLocked(ChapterType chapterType)
        {
            if (chapterType == ChapterType.Chapter1) return false;
            var index = (int) chapterType;
            var prevChapter = (ChapterType) (index - 1);
            return !IsClear(prevChapter);
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            if (isSet) return;
            SetStageScore();
            SetButtonEvent();
            isSet = true;
        }
        
        private void SetStageScore()
        {
            if (GlobalDataManager.Instance.HasKey(GlobalDataKey.STAGE_SCORE)) return;

            var scoreInfos = StageScoreManager.Create();
            GlobalDataManager.Instance.Set(GlobalDataKey.STAGE_SCORE, scoreInfos);
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
            controller.SetChapterSelectButtons(GetChapterInfo(chapterType), IsLocked(chapterType));
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