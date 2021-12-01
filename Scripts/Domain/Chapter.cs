using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Domain
{
    [Serializable]
    public class ChapterInfo
    {
        #region Public Variables

        [SerializeField] public string title;
        
        [JsonConverter(typeof(StringEnumConverter))]
        [SerializeField] public ChapterType chapterType;
        
        [SerializeField] public List<StageInfo> stageInfos;

        #endregion
    }

    [Serializable]
    public class StageInfo
    {
        #region Public Variables

        [JsonConverter(typeof(StringEnumConverter))]
        [SerializeField] public StageType stageType;
        
        [SerializeField] public int limitTime;
        [SerializeField] public int boxCount;
        [SerializeField] public int normalSpyCount;
        [SerializeField] public int bossSpyCount;
        [SerializeField] public int goalNormalSpyCount;
        [SerializeField] public int goalBossSpyCount;
        [SerializeField] public string topic;

        #endregion

        #region Getter

        public string GetStageMissionText()
        {
            return $"일반 스파이 {goalNormalSpyCount}명과\r\n보스 스파이 {goalBossSpyCount}명을\r\n색출해내세요.";
        }
        public string GetStageTopic()
        {
            return $"{topic}에 관한 문제들로 구성된 Stage입니다.";
        }

        #endregion
    }
}