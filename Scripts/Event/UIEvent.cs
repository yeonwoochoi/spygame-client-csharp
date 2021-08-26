using System;
using Domain;
using Domain.StageObj;

namespace Event
{
    public class OpenSpyQnaEventArgs: EventArgs
    {
        public Spy spy;

        public OpenSpyQnaEventArgs(Spy spy)
        {
            this.spy = spy;
        }
    }

    public class SkipSpyQnaEventArgs: EventArgs
    {
        public Spy spy;

        public SkipSpyQnaEventArgs(Spy spy)
        {
            this.spy = spy;
        }
    }
    
    public class SkipItemQnaEventArgs: EventArgs
    {
        public Item item;

        public SkipItemQnaEventArgs(Item item)
        {
            this.item = item;
        }
    }

    public class OpenItemQnaEventArgs : EventArgs
    {
        public Item item;

        public OpenItemQnaEventArgs(Item item)
        {
            this.item = item;
        }
    }

    public class OpenStagePauseEventArgs : EventArgs {}

    public class OutsideClickEventArgs : EventArgs { }
}