using System;
using System.Collections.Generic;

namespace Domain.Network
{
    // TODO(?)
    [Serializable]
    public class Response
    {
        #region Public Variables

        public string title;
        public Qna[] content;

        #endregion
    }
}