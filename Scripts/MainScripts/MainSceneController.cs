using System;
using System.Data;
using System.Linq.Expressions;
using Base;
using Domain;
using Event;
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

        [SerializeField] private Chapter[] chapters;
        [SerializeField] private Button[] chapterButtons;

        private ChapterManager chapterManager;

        #endregion

        #region Event Methods

        private void Awake()
        {
            chapterManager = GlobalDataManager.Instance.Get<ChapterManager>(GlobalDataKey.CHAPTER);
            if (chapterManager == null)
            {
                chapterManager = ChapterManager.Create();
                GlobalDataManager.Instance.Set(GlobalDataKey.CHAPTER, chapterManager);
            }

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
        }

        protected override void Start()
        {
            InitScores();
            SetButtonEvent();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Quit();   
            }
        }

        #endregion

        #region Private Methods

        // TODO(Data)
        private void InitScores()
        {
            for (var chapterIndex = 0; chapterIndex < chapters.Length; chapterIndex++)
            {
                for (var stageIndex = 0; stageIndex < chapters[chapterIndex].stages.Length; stageIndex++)
                {
                    chapters[chapterIndex].stages[stageIndex].score =
                        chapterManager.chapterInfos[chapterIndex].stageInfos[stageIndex].score;
                }
            }
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
            UnlockChapterButton(GetChapter(chapterType));
            var controller = button.GetComponent<ChapterSelectPopupButtonController>();
            controller.SetChapterSelectButtons(GetChapter(chapterType));
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
            LoadingManager.Instance.chapter = GetChapter(chapterType);

            StartCoroutine(StartLoadingAnimator(() =>
                {
                    nextScene = SceneNameManager.SceneNormalLoading;
                }, 
                () =>
                {
                    SceneManager.LoadScene(nextScene);
                }));
        }
        

        private Chapter GetChapter(ChapterType chapterType)
        {
            Chapter result = null;
            foreach (var chapter in chapters)
            {
                if (chapter.chapterType == chapterType)
                {
                    result = chapter;
                }
            }

            if (result == null)
            {
                // TODO (Error) : There is no chapter data
            }
            return result;
        }

        private void UnlockChapterButton(Chapter chapter)
        {
            if (chapter.chapterType == ChapterType.Chapter1)
            {
                chapter.isLocked = false;
                return;
            }
            var index = (int) chapter.chapterType;
            if (index + 1 >= chapters.Length) return;
            var nextChapter = GetChapter((ChapterType) (index + 1));
            if (nextChapter.chapterType == ChapterType.Chapter2)
            {
                Debug.Log($"{chapter.GetIsClear()}");
            }
            nextChapter.isLocked = !chapter.GetIsClear();
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