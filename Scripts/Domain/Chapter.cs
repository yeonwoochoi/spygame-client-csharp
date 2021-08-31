using System;
using UnityEngine;

namespace Domain
{
    // TODO(REF)
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