using System;
using Domain;
using Domain.StageObj;
using UI.Popup.Qna;

namespace Event
{
    public class ItemGetEventArgs: EventArgs
    {
        #region Public Variables

        public Item item;
        public ItemGetType type;

        #endregion

        #region Constructor

        public ItemGetEventArgs(Item item, ItemGetType type)
        {
            this.item = item;
            this.type = type;
        }

        #endregion
    }

    public class ItemUseEventArgs : EventArgs
    {
        public Item item;
    }
}