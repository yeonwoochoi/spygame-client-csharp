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
        //TODO (Getter Setter)

        public MainSceneType CurrentType { get; set; }
        public MainSceneType NextType { get; set; }
        public ChapterType ChapterType { get; set; }
        public StageType StageType { get; set; }
        public LoadingType LoadingType { get; set; }

        #region Public Variables

        public Stage stage;
        public Chapter chapter;
        public Qna[] qna;

        #endregion

        #region Static Variables
        
        private static LoadingManager instance = null;

        #endregion

        #region Constructor

        private LoadingManager()
        {
            CurrentType = MainSceneType.Main;
            LoadingType = LoadingType.Normal;
        }

        #endregion

        #region Static Method

        public static LoadingManager Instance => instance ?? (instance = new LoadingManager());

        #endregion
    }
}