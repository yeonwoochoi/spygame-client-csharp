using System;
using System.Collections;
using Event;
using TutorialScripts;
using UI.Base;
using UI.Timer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.TutorialScripts
{
    public class TutorialDonePopupController: BasePopupBehavior
    {
        [SerializeField] private GameObject retryButton;
        [SerializeField] private GameObject exitButton;
        [SerializeField] private Text contentText;
        private bool isDone = false;

        private const string titleComment = "튜토리얼 완료";
        private const string successComment = "튜토리얼을 완료하시느라 수고 많으셨습니다.\r\n이제 본게임에 들어가서 여러 스테이지를 깨보세요.";
        private const string failureComment = "시간 초과되었습니다.\r\n튜토리얼을 다시 진행하시겠습니까?";
        
        // Tutorial Exit button 눌렀을 때 Emit 하면 됨
        public static event EventHandler<ExitTutorialEventArgs> ExitTutorialEvent;
        public static event EventHandler<PauseGameEventArgs> PauseTutorialEvent;


        protected override void Start()
        {
            base.Start();
            
            retryButton.GetComponent<Button>().onClick.AddListener(RetryTutorial);
            exitButton.GetComponent<Button>().onClick.AddListener(ExitTutorial);
            
            TimerHudController.TutorialTimeOverEvent += OpenPopup;
            TutorialStateController.TutorialDoneEvent += OpenPopup;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            TimerHudController.TutorialTimeOverEvent -= OpenPopup;
            TutorialStateController.TutorialDoneEvent -= OpenPopup;
        }

        private void OpenPopup(object _, ExitTutorialEventArgs e)
        {
            if (isDone) return;
            
            var comment = e.isSuccess ? successComment : failureComment;
            SetComments(comment);
            
            EmitPauseTutorialEvent(new PauseGameEventArgs
            {
                isPaused = e.isSuccess
            });
            
            OnOpenPopup();
        }

        private void SetComments(string comment)
        {
            titleText.text = $"{titleComment}";
            contentText.text = $"{comment}";
            retryButton.SetActive(true);
            exitButton.SetActive(true);
            isDone = true;
        }
            
        private void EmitExitTutorialEvent(ExitTutorialEventArgs e)
        {
            if (ExitTutorialEvent == null) return;
            foreach (var invocation in ExitTutorialEvent.GetInvocationList())
            {
                invocation?.DynamicInvoke(this, e);
            }
        }
        
        
        private void EmitPauseTutorialEvent(PauseGameEventArgs e)
        {
            if (PauseTutorialEvent == null) return;
            foreach (var invocation in PauseTutorialEvent.GetInvocationList())
            {
                invocation?.DynamicInvoke(this, e);
            }
        }

        private void RetryTutorial()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void ExitTutorial()
        {
            EmitExitTutorialEvent(new ExitTutorialEventArgs());
        }
    }
}