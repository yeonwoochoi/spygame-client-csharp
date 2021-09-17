using System;
using Domain;
using Domain.StageObj;
using UnityEngine;
using UnityEngine.Events;

namespace Event
{
    /// <summary>
    /// Interaction : there is a cancel button
    /// Notice : there is no cancel button
    /// </summary>
    public enum AlertType
    {
        Interaction, Notice
    }

    public class OpenSpyQnaPopupEventArgs: EventArgs
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

    public class AlertOccurredEventArgs : EventArgs
    {
        public AlertType type;
        public string title;
        public string content;
        public string okBtnText = null;
        public string cancelBtnText = null;
        public UnityAction okHandler = null;
        public UnityAction cancelHandler = null;

        private AlertOccurredEventArgs(AlertType type, string title, string content, string okBtnText,
            string cancelBtnText, UnityAction okHandler, UnityAction cancelHandler) {
            this.type = type;
            this.title = title;
            this.content = content;
            this.okBtnText = okBtnText;
            this.cancelBtnText = cancelBtnText;
            this.okHandler = okHandler;
            this.cancelHandler = cancelHandler;
        }

        public static AlertBuilder Builder()
        {
            return new AlertBuilder();
        }
        
        public class AlertBuilder
            {
                private AlertType type;
                private string title;
                private string content;
                private string okBtnText = null;
                private string cancelBtnText = null;
                private UnityAction okHandler = null;
                private UnityAction cancelHandler = null;
        
                public AlertBuilder Type(AlertType type)
                {
                    this.type = type;
                    return this;
                }
        
                public AlertBuilder Title(string title)
                {
                    this.title = title;
                    return this;
                }
        
                public AlertBuilder Content(string content)
                {
                    this.content = content;
                    return this;
                }
        
                public AlertBuilder OkBtnText(string okBtnText)
                {
                    this.okBtnText = okBtnText;
                    return this;
                }
        
                public AlertBuilder CancelBtnText(string cancelBtnText)
                {
                    this.cancelBtnText = cancelBtnText;
                    return this;
                }
        
                public AlertBuilder OkHandler(UnityAction okHandler)
                {
                    this.okHandler = okHandler;
                    return this;
                }
        
                public AlertBuilder CancelHandler(UnityAction cancelHandler)
                {
                    this.cancelHandler = cancelHandler;
                    return this;
                }
        
                public AlertOccurredEventArgs Build() 
                    => new AlertOccurredEventArgs(type, title, content, okBtnText, cancelBtnText, okHandler, cancelHandler);
            }
    }
}