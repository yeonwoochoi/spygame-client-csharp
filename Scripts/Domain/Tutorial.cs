using System;
using UnityEngine;

namespace Domain
{
    [Serializable]
    public class TutorialManager
    {
        #region Public Variable

        public bool isClear;

        #endregion

        #region Static Method

        public static TutorialManager Create()
        {
            var tutorialManager = new TutorialManager
            {
                isClear = false
            };
            return tutorialManager;
        }

        #endregion
    }
}