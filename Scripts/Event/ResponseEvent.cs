using System;
using System.Collections.Generic;

namespace Event
{
    public enum DeserializeType {
        Qna, ChapterInfo, Score
    }
    
    public class ResponseOccurredEventArgs : EventArgs {
        #region Public Variables
        
        public readonly HashSet<DeserializeType> types;
        
        #endregion

        #region Constructor
        
        public ResponseOccurredEventArgs() {
            types = new HashSet<DeserializeType>();
        }
        
        #endregion

        #region Public Method
       
        public bool Contains(DeserializeType type) => types.Contains(type);
        
        #endregion
    }
}