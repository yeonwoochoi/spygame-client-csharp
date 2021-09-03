using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Domain
{
    // TODO(DATA)
    [Serializable]
    [ CreateAssetMenu( fileName = "Chapter", menuName = "Scriptable Object Asset/Chapter" )]
    public class Chapter: ScriptableObject
    {
        #region Public Variables

        public Stage[] stages;
        public GameObject mapPrefab;
        public ChapterType chapterType;
        [HideInInspector] public bool isLocked;

        #endregion

        #region Private Variables

        private bool isClear;

        #endregion

        #region Getter

        public bool GetIsClear()
        {
            isClear = true;
            foreach (var stage in stages)
            {
                if (stage.score < 1) isClear = false;
            }
            return isClear;
        }

        #endregion
    }

    [Serializable]
    public class PseudoChapter
    {
        #region Public Variable
        
        [SerializeField] public List<PseudoChapterInfo> chapterInfos;

        #endregion

        #region Static Variables

        private static PseudoChapter instance = null;
        public static PseudoChapter Instance => instance ?? (instance = new PseudoChapter());

        #endregion
        
        #region Constructor
        private PseudoChapter() {}

        #endregion

        #region Getter
        
        public PseudoChapterInfo GetChapterInfo(ChapterType chapterType)
        {
            PseudoChapterInfo result = null;
            if (chapterInfos == null || chapterInfos.Count == 0) return result;
            foreach (var chapterInfo in chapterInfos
                .Where(chapterInfo => chapterInfo.chapterType == chapterType))
            {
                result = chapterInfo;
            }

            return result;
        }

        public PseudoStageInfo GetStageInfo(ChapterType chapterType, StageType stageType)
        {
            PseudoStageInfo result = null;
            if (chapterInfos == null || chapterInfos.Count == 0) return result;
            foreach (var chapterInfo in chapterInfos
                .Where(chapterInfo => chapterInfo.chapterType == chapterType))
            {
                if (chapterInfo.stageInfos == null || chapterInfo.stageInfos.Count == 0)
                    return result;
                foreach (var stageInfo in chapterInfo.stageInfos
                    .Where(stageInfo => stageInfo.stageType == stageType))
                {
                    result = stageInfo;
                }
            }

            return result;
        }

        #endregion

        #region Public Method

        public void SetUp(PseudoChapter chapter)
        {
            chapterInfos = chapter.chapterInfos;
        }

        #endregion

        
    }

    [Serializable]
    public class PseudoChapterInfo
    {
        #region Public Variables

        [SerializeField] public string title;
        [SerializeField] public ChapterType chapterType;
        [SerializeField] public List<PseudoStageInfo> stageInfos;

        #endregion
    }

    [Serializable]
    public class PseudoStageInfo
    {
        #region Public Variables

        [SerializeField] public StageType stageType;
        [SerializeField] public int limitTime;
        [SerializeField] public int boxCount;
        [SerializeField] public int normalSpyCount;
        [SerializeField] public int bossSpyCount;
        [SerializeField] public int goalNormalSpyCount;
        [SerializeField] public int goalBossSpyCount;

        #endregion
    }
    

    [Serializable]
    public class ChapterInfo
    {
        #region Public Variable

        public StageInfo[] stageInfos;

        #endregion

        #region Static Method

        public static ChapterInfo Create()
        {
            var stageInfos = new StageInfo[6];
            for (var i = 0; i < stageInfos.Length; i++)
            {
                stageInfos[i] = new StageInfo
                {
                    score = 0
                };
            }

            return new ChapterInfo
            {
                stageInfos = stageInfos
            };
        }

        #endregion
    }

    [Serializable]
    public class ChapterManager
    {
        #region Public Variable

        public ChapterInfo[] chapterInfos;

        #endregion

        #region Static Method

        public static ChapterManager Create()
        {
            var chapterInfos = new ChapterInfo[6];
            for (var i = 0; i < chapterInfos.Length; i++)
            {
                chapterInfos[i] = ChapterInfo.Create();
            }

            return new ChapterManager
            {
                chapterInfos = chapterInfos
            };
        }

        #endregion
    }
}