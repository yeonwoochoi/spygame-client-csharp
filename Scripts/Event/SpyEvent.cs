using System;
using Domain;
using Domain.StageObj;
using UI.Qna;

namespace Event
{

    public class CaptureSpyEventArgs : EventArgs
    {
        public Spy spy;
        public CaptureSpyType type;

        public CaptureSpyEventArgs(Spy spy, CaptureSpyType type)
        {
            this.spy = spy;
            this.type = type;
        }
    }
}