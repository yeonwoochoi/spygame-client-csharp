using System;
using Domain;
using Domain.StageObj;
using UI.Qna;

namespace Event
{
    public class ItemGetEventArgs: EventArgs
    {
        public Item item;
        public ItemGetType type;

        public ItemGetEventArgs(Item item, ItemGetType type)
        {
            this.item = item;
            this.type = type;
        }
    }

    public class ItemUseEventArgs : EventArgs
    {
        public Item item;
        public ItemUseEventArgs(Item item)
        {
            this.item = item;
        }
    }
}