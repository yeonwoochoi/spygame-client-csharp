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

    // TODO()
    public class UpdateStageStateEventArgs : EventArgs
    {
        #region Public Variables

        public Stage currentStage;
        public int hp;
        public int captureNormalSpyCount;
        public int captureBossSpyCount;
        public int currentNormalSpyCount;
        public int currentBossSpyCount;

        #endregion

        #region Constructor

        public UpdateStageStateEventArgs(Stage currentStage, int hp, int captureNormalSpyCount, int captureBossSpyCount,
            int currentNormalSpyCount, int currentBossSpyCount)
        {
            this.currentStage = currentStage;
            this.hp = hp;
            this.captureNormalSpyCount = captureNormalSpyCount;
            this.captureBossSpyCount = captureBossSpyCount;
            this.currentNormalSpyCount = currentNormalSpyCount;
            this.currentBossSpyCount = currentBossSpyCount;
        }

        #endregion
    }

    public class OpenStageMissionPopupEventArgs : EventArgs
    {
        public Stage stage;
    }

    public class ExitStageEventArgs : EventArgs
    {
        public StageExitType exitType;
    }
}