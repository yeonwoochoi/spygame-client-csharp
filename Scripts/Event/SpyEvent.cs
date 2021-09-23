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

        #region Constructor

        public CaptureSpyEventArgs(Spy spy, CaptureSpyType type)
        {
            this.spy = spy;
            this.type = type;
        }

        #endregion
    }
}