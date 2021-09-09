using System;
using System.Collections;
using UnityEngine;

namespace Base
{
    public static class SceneNameManager {
        #region Const Variables

        public const string SceneMain = "Main Scene";
        public const string SceneChapter = "Chapter Scene";
        public const string SceneStage = "Stage Scene";
        public const string SceneTutorial = "Tutorial Scene";
        public const string SceneInitLoading = "Init Loading Scene";
        public const string SceneNormalLoading = "Normal Loading Scene";

        #endregion
    }
    
    public abstract class BaseSceneController: MonoBehaviour
    {
        #region Protected Variable

        protected static string nextScene = "";

        #endregion

        #region Protected Methods

        protected virtual void Start() {}

        protected IEnumerator StartLoadingAnimator(Action beforeLoading, Action afterLoading)
        {
            beforeLoading?.Invoke();
            yield return null;
            afterLoading?.Invoke();
        }

        #endregion
    }
}