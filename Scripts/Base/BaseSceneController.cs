using System;
using System.Collections;
using Event;
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

        #region Event
        
        // 서버랑 통신하는 경우는 다 로딩 Scene에서 일어나는데 여기서 이 이벤트 호출할 일 없을듯..
        public static event EventHandler<AlertOccurredEventArgs> AlertOccurredEvent; 

        #endregion

        #region Protected Methods

        protected virtual void Start() {}

        protected IEnumerator StartLoadingAnimator(Action beforeLoading, Action afterLoading)
        {
            beforeLoading?.Invoke();
            yield return null;
            afterLoading?.Invoke();
        }

        protected void EmitAlertOccurredEvent(object _, AlertOccurredEventArgs e)
        {
            if (AlertOccurredEvent == null) return;
            foreach (var invocation in AlertOccurredEvent.GetInvocationList())
                invocation?.DynamicInvoke(this, e);
        }

        #endregion
    }
}