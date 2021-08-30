using System;
using Domain;
using Domain.StageObj;

namespace Event
{
    public class OpenSpyQnaEventArgs: EventArgs
    {
        public Spy spy;
    }

    public class SkipSpyQnaEventArgs: EventArgs
    {
        public Spy spy;
    }
    
    public class SkipItemQnaEventArgs: EventArgs
    {
        public Item item;
    }

    public class OpenItemQnaEventArgs : EventArgs
    {
        public Item item;
    }

    public class OpenStagePauseEventArgs : EventArgs {}

    public class OutsideClickEventArgs : EventArgs { }
}