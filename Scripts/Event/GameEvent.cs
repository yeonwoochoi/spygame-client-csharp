using System;
using System.Collections.Generic;
using Domain;
using StageScripts;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Event
{
    #region Enum

    public enum StageExitType
    {
        GiveUp, GameOver, StageClear
    }

    #endregion

    public class UpdateStageStateEventArgs : EventArgs
    {
        #region Public Variables

        public readonly int hp;
        public readonly int captureNormalSpyCount;
        public readonly int captureBossSpyCount;
        public readonly int currentNormalSpyCount;
        public readonly int currentBossSpyCount;

        #endregion

        #region Constructor

        public UpdateStageStateEventArgs(int hp, int captureNormalSpyCount, int captureBossSpyCount,
            int currentNormalSpyCount, int currentBossSpyCount)
        {
            this.hp = hp;
            this.captureNormalSpyCount = captureNormalSpyCount;
            this.captureBossSpyCount = captureBossSpyCount;
            this.currentNormalSpyCount = currentNormalSpyCount;
            this.currentBossSpyCount = currentBossSpyCount;
        }

        #endregion
    }

    public class PauseGameEventArgs : EventArgs
    {
        public bool isPaused;
    }

    public class OpenStageMissionPopupEventArgs : EventArgs { }

    public class ExitStageEventArgs : EventArgs
    {
        public StageExitType exitType;
    }

    public class ExitTutorialEventArgs : EventArgs
    {
        public bool isSuccess;
    }
}