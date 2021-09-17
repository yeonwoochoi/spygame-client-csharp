using System;
using UnityEngine;

namespace Manager
{
    public class GlobalStageManager
    {
        #region Private Variable

        private bool isPaused = false;
        private bool isStageDone = false;

        #endregion

        #region Static Variable

        private static GlobalStageManager instance = null;
        public static GlobalStageManager Instance => instance ?? (instance = new GlobalStageManager());

        #endregion

        #region Getter

        public bool IsPaused()
        {
            return isPaused;
        }
        
        public bool IsStageDone()
        {
            return isStageDone;
        }
        
        #endregion


        #region Public Methods

        public void InitGame()
        {
            isPaused = false;
            isStageDone = false;
        }

        public void PauseGame()
        {
            isPaused = true;
        }

        public void ContinueGame()
        {
            isPaused = false;
        }

        public void FinishGame()
        {
            isStageDone = true;
        }

        #endregion
    }
}