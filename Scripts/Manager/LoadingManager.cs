using Domain;
using StageScripts;

namespace Manager
{ 
    public enum MainSceneType
    {
        Init, Main, Select, Play
    }

    public enum LoadingType
    {
        Init, Normal
    }

    public class LoadingManager
    {
        private static LoadingManager instance = null;
        
        public MainSceneType CurrentType { get; set; }
        public MainSceneType NextType { get; set; }
        public ChapterType ChapterType { get; set; }
        public StageType StageType { get; set; }
        public LoadingType LoadingType { get; set; }

        public Stage stage;
        public Chapter chapter;
        public Qna[] qna;
        
        private LoadingManager()
        {
            CurrentType = MainSceneType.Main;
            LoadingType = LoadingType.Normal;
        }

        public static LoadingManager Instance => instance ?? (instance = new LoadingManager());
    }
}