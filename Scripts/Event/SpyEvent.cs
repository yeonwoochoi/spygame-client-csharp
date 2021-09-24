using System;
using Domain;
using Domain.StageObj;
using UI.Popup.Qna;

namespace Event
{

    public class CaptureSpyEventArgs : EventArgs
    {
        #region Public Variables

        public Spy spy;
        public CaptureSpyType type;

        #endregion

        #region Getter

        public bool IsCorrect()
        {
            var case1 = spy.isSpy && type == CaptureSpyType.Capture;
            var case2 = !spy.isSpy && type == CaptureSpyType.Release;

            return case1 || case2;
        }

        #endregion

        #region Constructor

        public CaptureSpyEventArgs(Spy spy, CaptureSpyType type)
        {
            this.spy = spy;
            this.type = type;
        }

        #endregion
    }
}