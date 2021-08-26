using System;
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
        [SerializeField] private Chapter[] chapters;

        [SerializeField] private Button chapter1Button;
        [SerializeField] private Button chapter2Button;
        [SerializeField] private Button chapter3Button;
        [SerializeField] private Button chapter4Button;
        [SerializeField] private Button chapter5Button;
        [SerializeField] private Button chapter6Button;
        
        private void Awake()
        {
            var manager = GlobalDataManager.Instance.Get<ChapterManager>(GlobalDataKey.CHAPTER);
            if (manager == null)
            {
                var initChapterManager = ChapterManager.Create();
                GlobalDataManager.Instance.Set(GlobalDataKey.CHAPTER, initChapterManager);
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

        private void InitScores()
        {
            var chapterManager = GlobalDataManager.Instance.Get<ChapterManager>(GlobalDataKey.CHAPTER);
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
            SetButtonController(chapter1Button, ChapterType.Chapter1);
            SetButtonController(chapter2Button, ChapterType.Chapter2);
            SetButtonController(chapter3Button, ChapterType.Chapter3);
            SetButtonController(chapter4Button, ChapterType.Chapter4);
            SetButtonController(chapter5Button, ChapterType.Chapter5);
            SetButtonController(chapter6Button, ChapterType.Chapter6);
        }

        private void SetButtonController(Button button, ChapterType chapterType)
        {
            UnlockChapterButton(GetChapter(chapterType));
            var controller = button.GetComponent<ChapterSelectPopupButtonController>();
            controller.SetChapterSelectButtons(GetChapter(chapterType));
            button.onClick.AddListener(() =>
            {
                if (controller.IsLocked) return;
                LoadChapterScene(chapterType);
            });
        }
        
        private void LoadChapterScene(ChapterType chapterType)
        {
            LoadingManager.Instance.CurrentType = MainSceneType.Main;
            LoadingManager.Instance.NextType = MainSceneType.Select;
            LoadingManager.Instance.LoadingType = LoadingType.Normal;
            LoadingManager.Instance.ChapterType = chapterType;
            LoadingManager.Instance.chapter = GetChapter(chapterType);

            StartCoroutine(StartLoadingAnimator(() =>
                {
                    nextScene = SceneNameManager.SCENE_NORMAL_LOADING;
                }, 
                () =>
                {
                    SceneManager.LoadScene(nextScene);
                }));
        }
        

        private Chapter GetChapter(ChapterType chapterType)
        {
            var result = chapters[0];
            foreach (var chapter in chapters)
            {
                if (chapter.chapterType == chapterType)
                {
                    result = chapter;
                }
            }
            return result;
        }

        private void UnlockChapterButton(Chapter chapter)
        {
            if (chapter.chapterType == ChapterType.Chapter1) chapter.isLocked = false;
            var index = (int) chapter.chapterType;
            if (index >= chapters.Length) return;
            var nextChapter = GetChapter((ChapterType) (index + 1));
            nextChapter.isLocked = !chapter.IsClear;
        }
        
        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}