using System;
using UnityEngine;

namespace Domain
{
    // TODO(REF)
    [Serializable]
    [ CreateAssetMenu( fileName = "Chapter", menuName = "Scriptable Object Asset/Chapter" )]
    public class Chapter: ScriptableObject
    {
        public Stage[] stages;
        public GameObject mapPrefab;
        public ChapterType chapterType;
        private bool isClear;

        public bool IsClear
        {
            get
            {
                isClear = true;
                foreach (var stage in stages)
                {
                    if (stage.score < 1) isClear = false;
                }
                return isClear;
            }
        }

        [HideInInspector] public bool isLocked;
    }

    [Serializable]
    public class ChapterInfo
    {
        public StageInfo[] stageInfos;

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
    }

    [Serializable]
    public class ChapterManager
    {
        public ChapterInfo[] chapterInfos;

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
    }
}