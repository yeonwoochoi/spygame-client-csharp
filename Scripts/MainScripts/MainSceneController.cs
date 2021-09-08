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

        private ChapterInfo GetChapter(ChapterType chapterType)
        {
            return ChapterManager.Instance.GetChapterInfo(chapterType);
        }
        
        private bool IsClear(ChapterType chapterType)
        {
            var isClear = true;
            var currentChapter = GetChapter(chapterType);
            foreach (var stage in currentChapter.stageInfos.Where(stage => stage.score == 0))
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
            if (ChapterManager.Instance.IsSet())
            {
                SetButtonEvent();
                isSet = true;
                return;
            }
            StartCoroutine(GetStageInfo());
        }

        private IEnumerator GetStageInfo()
        {
            var www = HttpFactory.Build(RequestUrlType.ChapterInfo);
            yield return www.SendWebRequest();
            
            NetworkManager.HandleResponse(www, out var response, out var errorResponse);
            
            if (response == null && errorResponse == null)
            {
                NetworkManager.HandleServerError();
                yield break;
            }

            if (response != null)
            {
                SetButtonEvent();
                isSet = true;
                yield break;
            }

            var code = errorResponse.GetErrorCode();
            NetworkManager.HandleError(AlertOccurredEventArgs.Builder()
                .Type(AlertType.Notice)
                .Title("No Chapter Info")
                .Content(code.message)
                .OkHandler(() => Application.Quit(0))
                .Build()
            );
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
            controller.SetChapterSelectButtons(GetChapter(chapterType), IsLocked(chapterType));
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