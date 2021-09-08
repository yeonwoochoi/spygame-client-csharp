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
        public ChapterType chapterType = ChapterType.Chapter1;
        public StageType stageType = StageType.Stage1;
        public LoadingType loadingType;
        
        #endregion

        #region Static Variables
        
        private static LoadingManager instance = null;

        #endregion

        #region Constructor

        private LoadingManager()
        {
            currentType = MainSceneType.Main;
            loadingType = LoadingType.Normal;
        }

        #endregion

        #region Static Method

        public static LoadingManager Instance => instance ?? (instance = new LoadingManager());

        #endregion
    }
}