using System;
using Manager;
using StageScripts;
using UnityEngine;

namespace Domain
{
    #region Enums

    public enum StageType
    {
        Stage1, Stage2, Stage3, Stage4, Stage5, Stage6
    }

    public enum ChapterType
    {
        Chapter1, Chapter2, Chapter3, Chapter4, Chapter5, Chapter6
    }

    #endregion
    
    [Serializable]
    [ CreateAssetMenu( fileName = "Stage", menuName = "Scriptable Object Asset/Stage" )]
    public class Stage: ScriptableObject
    {
        #region Public Variables

        public GameObject stagePrefab;
        public Sprite mapSprite;
        public StageMissionType stageMissionType;
        public ChapterType chapterType;
        public StageType stageType;
        public int limitTime;
        public int boxCount;
        public int normalSpyCount;
        public int bossSpyCount;
        public int goalNormalSpyCount;
        public int goalBossSpyCount;
        public int score;

        #endregion

        #region Public Methods

        public string GetMissionText()
        {
            var result = $"일반 스파이 {goalNormalSpyCount}명과 보스 스파이 {goalBossSpyCount}명을 포획하시오.";
            return result;
        }

        public string GetStageInfoText()
        {
            var result = "일반 병사와 스파이를 구분해내세요.";
            return result;
        }

        #endregion
    }

    [Serializable]
    public class StageInfo
    {
        public int score;
    }
}