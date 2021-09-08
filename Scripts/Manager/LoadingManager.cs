using Domain;
using StageScripts;

namespace Manager
{
    #region Enums

    public enum MainSceneType
    {
        Init, Main, Select, Play
    }

    public enum LoadingType
    {
        Init, Normal
    }

    #endregion

    public class LoadingManager
    {
        #region Public Variables

        public MainSceneType currentType;
        public MainSceneType nextType;
        public ChapterType chapterType;
        public StageType stageType;
        public LoadingType loadingType;
        
        #endregion

        #region Static Variables
        
        private static LoadingManager instance = null;

        #endregion

        #region Constructor

        private LoadingManager()
        {
            currentType = MainSceneType.Init;
            loadingType = LoadingType.Init;
        }

        #endregion

        #region Static Method

        public static LoadingManager Instance => instance ?? (instance = new LoadingManager());

        #endregion
    }
}