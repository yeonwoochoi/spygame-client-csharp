﻿using System;
using System.Collections.Generic;
using Domain;
using StageScripts;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Event
{
    public enum StageExitType
    {
        GiveUp, GameOver, StageClear
    }
    
    public class UpdateStageStateEventArgs : EventArgs
    {
        public Stage currentStage;
        public int hp;
        public int captureNormalSpyCount;
        public int captureBossSpyCount;
        public int currentNormalSpyCount;
        public int currentBossSpyCount;

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
    }

    public class OpenStageMissionPopupEventArgs : EventArgs
    {
        public Stage stage;

        public OpenStageMissionPopupEventArgs(Stage stage)
        {
            this.stage = stage;
        }
    }

    public class ExitStageEventArgs : EventArgs
    {
        public StageExitType exitType;

        public ExitStageEventArgs(StageExitType exitType)
        {
            this.exitType = exitType;
        }
    }
}