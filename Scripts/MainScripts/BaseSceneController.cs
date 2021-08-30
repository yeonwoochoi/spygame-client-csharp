using System;
using System.Collections;
using Manager;
using UnityEngine;

// TODO(base namespace)
namespace MainScripts
{
    public static class SceneNameManager {
        #region Static Variables

        public static readonly string SCENE_MAIN = "Main Scene";
        public static readonly string SCENE_CHAPTER = "Chapter Scene";
        public static readonly string SCENE_STAGE = "Stage Scene";
        public static readonly string SCENE_INIT_LOADING = "Init Loading Scene";
        public static readonly string SCENE_NORMAL_LOADING = "Normal Loading Scene";

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