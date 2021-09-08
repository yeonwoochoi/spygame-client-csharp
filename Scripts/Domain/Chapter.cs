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

        #endregion

        #region Getter

        public string GetStageMissionText()
        {
            return $"Catch {goalNormalSpyCount} normal spy and {goalBossSpyCount} boss spies through interrogation.";
        }

        #endregion
    }
}