using System;
using Manager;
using StageScripts;
using UnityEngine;

namespace Domain
{
    public enum StageType
    {
        Stage1, Stage2, Stage3, Stage4, Stage5, Stage6
    }

    public enum ChapterType
    {
        Chapter1 = 1, 
        Chapter2 = 2, 
        Chapter3 = 3, 
        Chapter4 = 4, 
        Chapter5 = 5, 
        Chapter6 = 6
    }
    
    
    [Serializable]
    [ CreateAssetMenu( fileName = "Stage", menuName = "Scriptable Object Asset/Stage" )]
    public class Stage: ScriptableObject
    {
        public GameObject stagePrefab;
        public Sprite stageCaptureSprite;
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
    }

    [Serializable]
    public class StageInfo
    {
        public int score;
    }
}