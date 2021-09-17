using UnityEngine;

namespace Manager
{
    public class GlobalTutorialManager
    {
        #region Private Variable

        private bool isPaused = false;

        #endregion

        #region Static Variable

        private static GlobalTutorialManager instance = null;
        public static GlobalTutorialManager Instance => instance ?? (instance = new GlobalTutorialManager());

        #endregion

        #region Getter

        public bool IsPaused()
        {
            return isPaused;
        }
        
        #endregion
        

        #region Public Methods

        public void InitGame()
        {
            isPaused = false;
        }

        public void PauseGame()
        {
            isPaused = true;
        }

        public void ContinueGame()
        {
            isPaused = false;
        }

        #endregion
    }
}